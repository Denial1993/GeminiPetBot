using GeminiPetBot.Models;

namespace GeminiPetBot.Models
{
    /// <summary>
    /// 使用者傳送的聊天請求
    /// </summary>
    public class ChatRequest
    {
        /// <summary>
        /// 使用者 ID (例如: user_123)
        /// </summary>
        public string user_id { get; set; }

        /// <summary>
        /// 寵物基本資料
        /// </summary>
        public PetProfile pet_profile { get; set; }

        /// <summary>
        /// 使用者的問題 (例如: 狗可以吃葡萄嗎?)
        /// </summary>
        public string message { get; set; }
    }

    /// <summary>
    /// 寵物詳細資料
    /// </summary>
    public class PetProfile
    {
        /// <summary>
        /// 寵物種類 (只能填 dog 或 cat)
        /// </summary>
        public string species { get; set; } 

        /// <summary>
        /// 寵物年齡 (歲)
        /// </summary>
        public int age { get; set; }

        /// <summary>
        /// 寵物體重 (公斤)
        /// </summary>
        public double weight { get; set; }
    }

    /// <summary>
    /// AI 回傳的答案格式
    /// </summary>
    public class ChatResponse
    {
        /// <summary>
        /// AI 的回答內容
        /// </summary>
        public string answer { get; set; }

        /// <summary>
        /// 引用來源清單 (例如: 內部公告 PDF 第 5 頁)
        /// </summary>
        public List<string> citations { get; set; } = new List<string>();

        /// <summary>
        /// 風險等級 (low | medium | high)
        /// </summary>
        public string risk_level { get; set; }

        /// <summary>
        /// 建議的後續行動 (例如: 立即就醫)
        /// </summary>
        public List<string> suggested_next_actions { get; set; } = new List<string>();
    }
    
    // (Google 用的 Request 物件不用給 Swagger 看，可以不用加註解)
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