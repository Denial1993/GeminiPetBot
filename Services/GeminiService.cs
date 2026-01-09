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
        private readonly string _endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent";
        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            // 2. 從設定檔讀取 Key
            var keyFromConfig = configuration["Gemini:ApiKey"];

            if (string.IsNullOrEmpty(keyFromConfig))
            {
                throw new Exception("嚴重錯誤: appsettings.json 裡面的 API Key 是空的！");
            }

            // 3. ★關鍵修正★：強制刪除 Key 前後的空白或換行，避免網址壞掉
            _apiKey = keyFromConfig.Trim();
        }

        public async Task<ChatResponse> GetAnswerAsync(ChatRequest request)
        {
            // 4. 準備 Prompt
            var systemPrompt = $@"
你是一個專業的寵物醫療助手。
使用者有一隻 {request.pet_profile.age} 歲的 {request.pet_profile.species}，體重 {request.pet_profile.weight} 公斤。
使用者問題：{request.message}

請以 JSON 格式回答，不要使用 Markdown code block。
格式必須包含：answer, risk_level (low/medium/high), suggested_next_actions (array of strings)。
如果不知道，請回答不知道。
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

            // 5. 組合網址並印出 (方便除錯，如果還有錯可以看到網址長怎樣)
            var finalUrl = $"{_endpoint}?key={_apiKey}";

            // 發送請求
            var response = await _httpClient.PostAsync(finalUrl, jsonContent);

            // 6. ★除錯核心★：如果失敗，讀取 Google 罵我們的內容
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // 這裡會顯示 400, 404, 500 等具體錯誤
                throw new Exception($"Google API 報錯! Code: {response.StatusCode}, URL: {finalUrl}, 詳細: {errorContent}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            dynamic googleResponse = JsonConvert.DeserializeObject(responseString);

            // 7. 取出結果
            try
            {
                string rawText = googleResponse.candidates[0].content.parts[0].text;
                // 清理 Markdown
                rawText = rawText.Replace("```json", "").Replace("```", "").Trim();
                return JsonConvert.DeserializeObject<ChatResponse>(rawText);
            }
            catch
            {
                // 如果 AI 沒回傳標準 JSON，回傳原始文字
                return new ChatResponse
                {
                    answer = "解析錯誤，但 AI 有回應：" + responseString,
                    risk_level = "low"
                };
            }
        }
    }
}