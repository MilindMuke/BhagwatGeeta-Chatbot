using BhagwatGitaChatbot.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IAwsComprehendService, LocalNlpService>();
builder.Services.AddScoped<IVerseService, VerseService>();
builder.Services.AddScoped<IResponseGenerator, ResponseGenerator>();
builder.Services.AddScoped<IKeywordContextService>(provider =>
    new KeywordContextService("LocalData/bhagavad_gita_keywords.json"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
