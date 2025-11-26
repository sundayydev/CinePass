using CinePass.Domain;
using CinePass.Domain.IRepository;
using CinePass.Domain.Models;
using CinePass.Shared.DTOs.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CinePass.Core.Services;

public class AuthService
{
    private readonly IUserRepository _userRepo; // Inject Repository thay vì DbContext
    private readonly IConnectionMultiplexer _redis;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepo, IConnectionMultiplexer redis, IConfiguration configuration)
    {
        _userRepo = userRepo;
        _redis = redis;
        _configuration = configuration;
    }

    // --- LOGIC ĐĂNG KÝ ---
    public async Task<string> RegisterAsync(RegisterDto request)
    {
        // 1. Validate Logic
        if (await _userRepo.ExistsByEmailAsync(request.Email))
            return "Email đã tồn tại.";

        if (await _userRepo.ExistsByPhoneAsync(request.PhoneNumber))
            return "Số điện thoại đã tồn tại.";

        // 2. Map DTO sang Entity
        var newUser = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Customer,
            CreatedAt = DateTime.UtcNow
        };

        // 3. Lưu xuống DB qua Repo
        await _userRepo.AddAsync(newUser);
        await _userRepo.SaveChangesAsync();

        return "Success";
    }

    // --- LOGIC ĐĂNG NHẬP ---
    public async Task<TokenResponse?> LoginAsync(LoginRequest request)
    {
        // 1. Tìm user
        var user = await _userRepo.GetByEmailAsync(request.Username); // Giả sử username là email

        // 2. Check pass
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        // 3. Sinh Token
        return await GenerateTokensAsync(user);
    }

    // --- LOGIC REFRESH TOKEN ---
    public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
    {
        var dbRedis = _redis.GetDatabase();
        var userIdStr = await dbRedis.StringGetAsync($"rt:{refreshToken}");

        if (userIdStr.IsNullOrEmpty) return null;

        int userId = int.Parse(userIdStr!);
        var user = await _userRepo.GetByIdAsync(userId);

        if (user == null) return null;

        // Xóa token cũ (Token Rotation)
        await dbRedis.KeyDeleteAsync($"rt:{refreshToken}");

        return await GenerateTokensAsync(user);
    }

    // --- HÀM PHỤ TRỢ (Private) ---
    private async Task<TokenResponse> GenerateTokensAsync(User user)
    {
        // A. Tạo Access Token (JWT)
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = creds,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

        // B. Tạo Refresh Token
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        // C. Lưu Refresh Token vào Redis (7 ngày)
        var dbRedis = _redis.GetDatabase();
        await dbRedis.StringSetAsync($"rt:{refreshToken}", user.UserID.ToString(), TimeSpan.FromDays(7));

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}