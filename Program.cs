// Program.cs
using GeminiPetBot.Services;
using System.Reflection; 

var builder = WebApplication.CreateBuilder(args);

// 1. 加入 Controller 服務
builder.Services.AddControllers();

// 2. 加入 Swagger (API 文件與測試介面)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // 取得 XML 檔案名稱 (通常是 專案名稱.xml)
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // 組合出完整路徑
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    // 告訴 Swagger 去讀它
    options.IncludeXmlComments(xmlPath);
});

// 3. ★★★ 註冊 GeminiService (這一行很重要，雖然 Service 檔案還沒建，先寫著)
builder.Services.AddHttpClient<GeminiService>();

// 註冊知識庫服務 (Singleton 代表程式啟動後只讀一次檔案，效能較好)
// AddSingleton 的意思是：「請幫我建立一個 KnowledgeService，而且整個應用程式只要這一個實體。」
// 這樣做的好處是：當你在 Upload API 呼叫 LoadKnowledge() 更新了資料後，因為大家共用同一個實體，所以 Chat API 下一次回答時，就會用到更新後的資料。
builder.Services.AddSingleton<KnowledgeService>();

var app = builder.Build();

// 設定 HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseDefaultFiles(); // 讓系統自動找 index.html
app.UseStaticFiles();  // 啟用 wwwroot 資料夾功能
app.MapControllers();

app.Run();