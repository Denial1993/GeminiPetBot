using GeminiPetBot.Models;
using GeminiPetBot.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeminiPetBot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly GeminiService _geminiService;
        private readonly KnowledgeService _knowledgeService; // ★ 新增這個變數
        public ChatController(GeminiService geminiService, KnowledgeService knowledgeService)
        {
            _geminiService = geminiService;
            _knowledgeService = knowledgeService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ChatResponse), 200)] // ★ 加入這行，告訴 Swagger 回傳格式是 ChatResponse
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            // 比賽要求：必須要能運作的 Demo
            if (request == null || request.pet_profile == null)
            {
                return BadRequest("Invalid request format");
            }

            var result = await _geminiService.GetAnswerAsync(request);
            return Ok(result);
        }

        // 加入這段 Code 到 Controller 裡
        [HttpPost("upload")]
        public async Task<IActionResult> UploadKnowledge(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("請選擇檔案");

            // 確保只收 json
            if (!file.FileName.EndsWith(".json"))
                return BadRequest("只接受 .json 格式");

            var folderPath = Path.Combine(AppContext.BaseDirectory, "Data");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            // ★ 這裡就是「覆蓋舊檔」的關鍵邏輯
            var filePath = Path.Combine(folderPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create)) //同一個檔案名稱會新蓋舊。
            {
                await file.CopyToAsync(stream);
            }
            // ★★★ 關鍵補強：叫 Service 重新讀取硬碟裡的檔案 ★★★
            _knowledgeService.LoadKnowledge();

            return Ok(new { message = $"檔案 {file.FileName} 上傳成功，知識庫已更新！" });
        }
    }
}