// Program.cs
using GeminiPetBot.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. 加入 Controller 服務
builder.Services.AddControllers();

// 2. 加入 Swagger (API 文件與測試介面)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. ★★★ 註冊 GeminiService (這一行很重要，雖然 Service 檔案還沒建，先寫著)
builder.Services.AddHttpClient<GeminiService>();

// 註冊知識庫服務 (Singleton 代表程式啟動後只讀一次檔案，效能較好)
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
app.MapControllers();

app.Run();