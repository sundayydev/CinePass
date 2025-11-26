using System;
using System.Collections.Generic;
using System.Text;

namespace CinePass.Shared.DTOs.Auth;

public class TokenResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
