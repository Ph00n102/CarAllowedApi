using System.Net.Http.Json;
using Microsoft.JSInterop;
using static HosxpUi.Services.JobNumberService;

namespace HosxpUi.Services
{
        public interface IJobNumberService
    {
        Task<string> GetNextJobNumberAsync();
        Task<string> GetLastJobNumberAsync();
        Task<string> GetJobNumberFromDatabaseAsync(int jobId);
        Task UpdateJobNumberInDatabaseAsync(int jobId, string jobNumber);
        Task<string> GetNextJobNumberPreviewAsync();
        Task<JobNumberInfo> GetJobNumberInfoAsync();
        Task<bool> ResetCounterAsync();
    }

    public class JobNumberService : IJobNumberService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IJSRuntime _jsRuntime;
        private string? _nextJobNumberPreview;
        private DateTime _lastPreviewUpdate = DateTime.MinValue;

        // ✅ ต้องมี constructor ที่รับ IHttpClientFactory
        public JobNumberService(IHttpClientFactory httpClientFactory, IJSRuntime jsRuntime)
        {
            _httpClientFactory = httpClientFactory;
            _jsRuntime = jsRuntime;
        }

        public async Task<string> GetLastJobNumberAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CarApi");
                var response = await client.GetAsync("api/JobNumber/GetLastJobNumber");
                
                if (response.IsSuccessStatusCode)
                {
                    var lastNumber = await response.Content.ReadAsStringAsync();
                    return lastNumber?.Trim('"') ?? "000000";
                }
                
                return "000000";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting last job number: {ex.Message}");
                return "000000";
            }
        }
        // ดึงเลขรันล่าสุดจาก API (Backend)
        public async Task<string> GetNextJobNumberAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CarApi");
                var response = await client.GetAsync("api/JobDistribute/GetNextJobNumber");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JobNumberResponse>();
                    if (!string.IsNullOrEmpty(result?.JobNumber))
                    {
                        // อัพเดท preview
                        _nextJobNumberPreview = result.JobNumber;
                        _lastPreviewUpdate = DateTime.Now;
                        
                        Console.WriteLine($"✅ ได้รับเลขรันใหม่จาก API: {result.JobNumber}");
                        return result.JobNumber;
                    }
                }
                
                // ถ้า API ไม่สำเร็จ ให้ใช้ fallback
                return await GetFallbackJobNumberAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting next job number from API: {ex.Message}");
                return await GetFallbackJobNumberAsync();
            }
        }

        // วิธีสำรองถ้า API ไม่ทำงาน
        private async Task<string> GetFallbackJobNumberAsync()
        {
            try
            {
                // ใช้ localStorage เป็น fallback
                return await _jsRuntime.InvokeAsync<string>("getNextJobNumberFromStorage");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fallback error: {ex.Message}");
                // สร้างเลขรันจากวันนี้
                var today = DateTime.Now.ToString("yyMMdd");
                return $"{today}001";
            }
        }

        // อัพเดทเลขรันในฐานข้อมูล
        public async Task UpdateJobNumberInDatabaseAsync(int jobId, string jobNumber)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CarApi");
                var updateData = new { JobNumber = jobNumber };
                var response = await client.PutAsJsonAsync($"api/JobRequest/UpdateJobNumber/{jobId}", updateData);
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ อัพเดทเลขรัน {jobNumber} สำหรับงาน #{jobId} ในฐานข้อมูลสำเร็จ");
                    
                    // บันทึกใน localStorage ด้วย (สำรอง)
                    try
                    {
                        await _jsRuntime.InvokeVoidAsync("saveJobNumberToStorage", jobId, jobNumber);
                    }
                    catch
                    {
                        // ถ้าไม่สำเร็จไม่เป็นไร
                    }
                }
                else
                {
                    Console.WriteLine($"❌ ไม่สามารถอัพเดทเลขรันในฐานข้อมูลได้");
                    await UpdateJobNumberFallbackAsync(jobId, jobNumber);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error updating job number in DB: {ex.Message}");
                await UpdateJobNumberFallbackAsync(jobId, jobNumber);
            }
        }

        private async Task UpdateJobNumberFallbackAsync(int jobId, string jobNumber)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("saveJobNumberToStorage", jobId, jobNumber);
                Console.WriteLine($"✅ บันทึกเลขรัน {jobNumber} สำหรับงาน #{jobId} ใน localStorage สำเร็จ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fallback save failed: {ex.Message}");
            }
        }

        // ดึงเลขรันจากฐานข้อมูล
        public async Task<string> GetJobNumberFromDatabaseAsync(int jobId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CarApi");
                var response = await client.GetAsync($"api/JobDistribute/GetJobNumber/{jobId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JobNumberResponse>();
                    if (!string.IsNullOrEmpty(result?.JobNumber))
                    {
                        return result.JobNumber;
                    }
                }
                
                // ถ้าไม่ได้จาก API ให้ลองดึงจาก localStorage
                return await GetJobNumberFallbackAsync(jobId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting job number from DB: {ex.Message}");
                return await GetJobNumberFallbackAsync(jobId);
            }
        }

        private async Task<string> GetJobNumberFallbackAsync(int jobId)
        {
            try
            {
                var jobNumber = await _jsRuntime.InvokeAsync<string>("getJobNumberFromStorage", jobId);
                return jobNumber ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        // ดึงตัวอย่างเลขรันถัดไป (สำหรับแสดง preview)
        public async Task<string> GetNextJobNumberPreviewAsync()
        {
            try
            {
                // ตรวจสอบว่า preview เก่าเกินไปหรือไม่ (เกิน 5 นาที)
                if (_nextJobNumberPreview == null || 
                    DateTime.Now.Subtract(_lastPreviewUpdate).TotalMinutes > 5)
                {
                    var client = _httpClientFactory.CreateClient("CarApi");
                    var response = await client.GetAsync("api/JobDistribute/GetNextJobNumberPreview");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<JobNumberResponse>();
                        _nextJobNumberPreview = result?.JobNumber;
                        _lastPreviewUpdate = DateTime.Now;
                    }
                    else
                    {
                        // สร้าง preview จาก localStorage
                        _nextJobNumberPreview = await _jsRuntime.InvokeAsync<string>("getNextJobNumberFromStorage");
                    }
                }
                
                return _nextJobNumberPreview ?? "000001";
            }
            catch
            {
                return "000001";
            }
        }

        // ดึงข้อมูลตัวนับ
        public async Task<JobNumberInfo> GetJobNumberInfoAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CarApi");
                var response = await client.GetAsync("api/JobDistribute/GetJobNumberInfo");
                
                if (response.IsSuccessStatusCode)
                {
                    var info = await response.Content.ReadFromJsonAsync<JobNumberInfo>();
                    return info ?? new JobNumberInfo();
                }
                
                return new JobNumberInfo
                {
                    LastResetDate = DateTime.Today,
                    CurrentCounter = 0,
                    TotalJobsToday = 0,
                    LastJobNumber = "N/A"
                };
            }
            catch
            {
                return new JobNumberInfo();
            }
        }

        // รีเซ็ตตัวนับ
        public async Task<bool> ResetCounterAsync()
        {
            try
            {
                // รีเซ็ตใน localStorage
                await _jsRuntime.InvokeVoidAsync("resetJobCounter");
                
                // ล้าง cache preview
                _nextJobNumberPreview = null;
                _lastPreviewUpdate = DateTime.MinValue;
                
                Console.WriteLine("✅ รีเซ็ตตัวนับสำเร็จ");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error resetting counter: {ex.Message}");
                return false;
            }
        }

        // DTO classes
        public class JobNumberResponse
        {
            public string JobNumber { get; set; } = string.Empty;
            public DateTime GeneratedDate { get; set; }
        }

        public class JobNumberInfo
        {
            public DateTime LastResetDate { get; set; }
            public int CurrentCounter { get; set; }
            public int NextCounter { get; set; }
            public int TotalJobsToday { get; set; }
            public string LastJobNumber { get; set; } = string.Empty;
            public string NextJobNumberPreview { get; set; } = string.Empty;
        }
    }
}