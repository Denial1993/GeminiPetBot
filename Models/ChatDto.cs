namespace GeminiPetBot.Models
{
    // 請求格式 (對應 PDF page 3)
    public class ChatRequest
    {
        public string user_id { get; set; }
        public PetProfile pet_profile { get; set; }
        public string message { get; set; }
    }

    public class PetProfile
    {
        public string species { get; set; } // dog | cat
        public int age { get; set; }
        public double weight { get; set; }
    }

    // 回應格式 (對應 PDF page 4)
    public class ChatResponse
    {
        public string answer { get; set; }
        public List<string> citations { get; set; } = new List<string>();
        public string risk_level { get; set; } // low | medium | high
        public List<string> suggested_next_actions { get; set; } = new List<string>();
    }
    
    // Google Gemini API 的 Request 格式 (這是給 Google看的)
    public class GoogleGeminiRequest
    {
        public List<Content> contents { get; set; }
    }
    public class Content
    {
        public List<Part> parts { get; set; }
    }
    public class Part
    {
        public string text { get; set; }
    }
}