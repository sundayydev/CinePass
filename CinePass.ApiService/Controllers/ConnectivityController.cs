using CinePass.Core.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Dùng cho Entity Framework (Postgres)
using Minio;                         // Dùng cho MinIO
using StackExchange.Redis;           // Dùng cho Redis
using System.Diagnostics;

namespace CinePass.ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectivityController : ControllerBase
    {
        // 1. Inject các service client đã được đăng ký trong Program.cs
        // Lưu ý: Thay 'AppDbContext' bằng tên DbContext thực tế của bạn
        private readonly AppDbContext _context;
        private readonly IConnectionMultiplexer _redis;
        private readonly IMinioClient _minioClient;
        private readonly ILogger<ConnectivityController> _logger;

        public ConnectivityController(
            AppDbContext dbContext,
            IConnectionMultiplexer redis,
            IMinioClient minioClient,
            ILogger<ConnectivityController> logger)
        {
            _context = dbContext;
            _redis = redis;
            _minioClient = minioClient;
            _logger = logger;
        }

        /// <summary>
        /// API kiểm tra tình trạng kết nối tới tất cả các dịch vụ hạ tầng
        /// GET: api/connectivity/health-check
        /// </summary>
        [HttpGet("health-check")]
        public async Task<IActionResult> CheckHealth()
        {
            var statusReport = new Dictionary<string, object>();
            var stopWatch = new Stopwatch();

            // --- 1. Kiểm tra PostgreSQL ---
            stopWatch.Start();
            try
            {
                // Thử mở kết nối tới DB (hoặc chạy query đơn giản)
                bool canConnect = await _context.Database.CanConnectAsync();
                stopWatch.Stop();

                statusReport.Add("Postgres", new
                {
                    Status = canConnect ? "Healthy" : "Unhealthy",
                    Latency = $"{stopWatch.ElapsedMilliseconds}ms"
                });
            }
            catch (Exception ex)
            {
                statusReport.Add("Postgres", new { Status = "Error", Message = ex.Message });
                _logger.LogError(ex, "Lỗi kết nối Postgres");
            }

            // --- 2. Kiểm tra Redis (Cache) ---
            stopWatch.Restart();
            try
            {
                var db = _redis.GetDatabase();
                // Lệnh Ping trả về thời gian phản hồi
                var latency = await db.PingAsync();
                stopWatch.Stop();

                statusReport.Add("Redis", new
                {
                    Status = "Healthy",
                    Latency = $"{latency.TotalMilliseconds}ms"
                });
            }
            catch (Exception ex)
            {
                statusReport.Add("Redis", new { Status = "Error", Message = ex.Message });
                _logger.LogError(ex, "Lỗi kết nối Redis");
            }

            // --- 3. Kiểm tra MinIO (Storage) ---
            stopWatch.Restart();
            try
            {
                // Thử liệt kê các bucket để xem kết nối có thông không
                // SỬA ĐỔI: Sử dụng cú pháp tương thích với MinIO SDK phiên bản cũ (< 5.0)
                // Nếu bạn update lên bản mới (5.0+), hàm này sẽ cần tham số 'new ListBucketsArgs()'

                var buckets = await _minioClient.ListBucketsAsync();

                stopWatch.Stop();

                statusReport.Add("MinIO", new
                {
                    Status = "Healthy",
                    Latency = $"{stopWatch.ElapsedMilliseconds}ms",
                    BucketCount = buckets.Buckets.Count
                });
            }
            catch (Exception ex)
            {
                // MinIO thường ném exception nếu không kết nối được
                statusReport.Add("MinIO", new { Status = "Error", Message = ex.Message });
                _logger.LogError(ex, "Lỗi kết nối MinIO");
            }

            // Tổng hợp kết quả
            return Ok(new
            {
                Timestamp = DateTime.UtcNow,
                Services = statusReport
            });
        }
    }
}