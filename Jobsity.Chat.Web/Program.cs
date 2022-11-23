using EasyNetQ;
using Jobsity.Chat.Core.Models.Options;
using Jobsity.Chat.Core.Persistence;
using Jobsity.Chat.Core.Services;
using Jobsity.Chat.Persistence.DistributedCache;
using Jobsity.Chat.Persistence.EntityFramework;
using Jobsity.Chat.Services;
using Jobsity.Chat.Services.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSingleton<ICache, DistributedMemoryCache>();

builder.Services.AddOptions<StockTickerOptions>()
    .Bind(builder.Configuration.GetSection(nameof(StockTickerOptions)))
    .ValidateDataAnnotations();

builder.Services.AddDbContextPool<ChatContext>(options => options.UseInMemoryDatabase(nameof(ChatContext)));

builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IChatRoomRepository, ChatRoomRepository>();

builder.Services.AddScoped<IStockBotService, StockBotService>();
builder.Services.AddHttpClient<IStockTickerService, StockTickerService>(p => p.BaseAddress = new Uri("https://stooq.com/q/l/"));

builder.Services.AddSingleton<IBus>(
    RabbitHutch.CreateBus(builder.Configuration.GetConnectionString("RabbitMQ"))
);

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

app.MapHub<ChatHub>("/chatHub");

await app.RunAsync();
