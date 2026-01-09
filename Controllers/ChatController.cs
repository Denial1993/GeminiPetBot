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

        public ChatController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost]
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
    }
}