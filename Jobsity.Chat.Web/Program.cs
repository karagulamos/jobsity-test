using Jobsity.Chat.Core.Persistence;
using Jobsity.Chat.Core.Services;
using Jobsity.Chat.Persistence.EntityFramework;
using Jobsity.Chat.Services;
using Jobsity.Chat.Web.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddDbContextPool<ChatContext>(options => options.UseInMemoryDatabase(nameof(ChatContext)));

builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IChatRoomRepository, ChatRoomRepository>();

builder.Services.AddScoped<IStockBotService, StockBotService>();

builder.Services.AddHttpClient<IStockTickerService, StockTickerService>(p => p.BaseAddress = new Uri("https://stooq.com/q/l/"));

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    await scope.ServiceProvider
               .GetRequiredService<ChatContext>()
               .Database
               .EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.MapHub<ChatHub>("/chathub");

await app.RunAsync();
