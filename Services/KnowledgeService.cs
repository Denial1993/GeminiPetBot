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
            _knowledgeBase = new List<KnowledgeEntry>();

            // 取得資料夾路徑
            var dataPath = Path.Combine(AppContext.BaseDirectory, "Data");
            if (!Directory.Exists(dataPath)) return;

            // ★ 修改點：讀取該資料夾下 "所有" .json 檔案
            var files = Directory.GetFiles(dataPath, "*.json");

            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var entries = JsonConvert.DeserializeObject<List<KnowledgeEntry>>(json);
                    if (entries != null)
                    {
                        _knowledgeBase.AddRange(entries);
                    }
                }
                catch
                {
                    // 略過格式錯誤的檔案，避免 crash
                }
            }
        }
        public List<KnowledgeEntry> GetAllKnowledge()
        {
            return _knowledgeBase ?? new List<KnowledgeEntry>();
        }
    }
}