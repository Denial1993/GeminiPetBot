using Newtonsoft.Json;

namespace GeminiPetBot.Services
{
    public class KnowledgeEntry
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public string Topic { get; set; }
        public string Content { get; set; }
        public string Source { get; set; }
    }

    public class KnowledgeService
    {
        private List<KnowledgeEntry> _knowledgeBase;
        // 設定路徑：讀取 Data 資料夾底下的 json
        private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "Data", "knowledge.json");

        public KnowledgeService()
        {
            LoadKnowledge();
        }

        public void LoadKnowledge()
        {
            // 如果檔案存在就讀取，不存在就回傳空陣列
            // 這裡要注意：VS 編譯時會把檔案複製到 bin 目錄，我們等等要設定一下
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _knowledgeBase = JsonConvert.DeserializeObject<List<KnowledgeEntry>>(json);
            }
            else
            {
                // 本機開發時有時候路徑會跑掉，這裡做個備用檢查
                var localPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "knowledge.json");
                if (File.Exists(localPath))
                {
                    var json = File.ReadAllText(localPath);
                    _knowledgeBase = JsonConvert.DeserializeObject<List<KnowledgeEntry>>(json);
                }
                else
                {
                    _knowledgeBase = new List<KnowledgeEntry>();
                }
            }
        }

        public List<KnowledgeEntry> GetAllKnowledge()
        {
            return _knowledgeBase ?? new List<KnowledgeEntry>();
        }
    }
}