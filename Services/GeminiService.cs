using GeminiPetBot.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace GeminiPetBot.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        // private readonly string _endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent";
        private readonly string _endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent";
        // ★ 新增：注入知識庫服務
        private readonly KnowledgeService _knowledgeService;

        public GeminiService(HttpClient httpClient, IConfiguration configuration, KnowledgeService knowledgeService)
        {
            _httpClient = httpClient;
            _knowledgeService = knowledgeService; // ★ 存起來

            var keyFromConfig = configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(keyFromConfig))
            {
                throw new Exception("嚴重錯誤: appsettings.json 裡面的 API Key 是空的！");
            }
            _apiKey = keyFromConfig.Trim(); 
        }

        public async Task<ChatResponse> GetAnswerAsync(ChatRequest request)
        {
            // 1. 從知識庫撈出所有資料
            var allKnowledge = _knowledgeService.GetAllKnowledge();
            var knowledgeJson = JsonConvert.SerializeObject(allKnowledge);

            // 2. 組合 Prompt (這就是 RAG 的精隨：把資料餵給 AI)
            var systemPrompt = $@"
你是一個專業的寵物醫療助手。請根據以下的「已知知識庫」來回答使用者的問題。

【嚴格規則】
1. 只能依據「已知知識庫」的內容回答。
2. 若知識庫無相關資訊，請委婉回答：「抱歉，目前的資料庫中尚未收錄此問題的相關資訊，建議您諮詢專業獸醫師以獲得準確建議。」(語氣需親切專業)。
3. 如果是高風險症狀（如：呼吸急促、中毒），Risk Level 必須設為 'high' 並建議就醫。
4. 必須在 'citations' 欄位中列出你參考的 'source' (例如：內部公告 PDF 第 5 頁)。

【已知知識庫】
{knowledgeJson}

【使用者情境】
寵物：{request.pet_profile.age} 歲的 {request.pet_profile.species}，體重 {request.pet_profile.weight} 公斤。
問題：{request.message}

【輸出格式 (JSON Only)】
請直接回傳 JSON，不要 markdown 標記：
{{
    ""answer"": ""你的回答"",
    ""citations"": [""來源1"", ""來源2""],
    ""risk_level"": ""low | medium | high"",
    ""suggested_next_actions"": [""建議行動1"", ""建議行動2""]
}}
";

            var googlePayload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = systemPrompt } }
                    }
                }
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(googlePayload), Encoding.UTF8, "application/json");
            var finalUrl = $"{_endpoint}?key={_apiKey}";
            
            var response = await _httpClient.PostAsync(finalUrl, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Google API 報錯! Code: {response.StatusCode}, 詳細: {errorContent}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            dynamic googleResponse = JsonConvert.DeserializeObject(responseString);
            
            try 
            {
                 string rawText = googleResponse.candidates[0].content.parts[0].text;
                 // 清理 Markdown
                 rawText = rawText.Replace("```json", "").Replace("```", "").Trim();
                 return JsonConvert.DeserializeObject<ChatResponse>(rawText);
            }
            catch
            {
                return new ChatResponse 
                { 
                    answer = "解析錯誤，但 AI 有回應：" + responseString, 
                    risk_level = "low"
                };
            }
        }
    }
}