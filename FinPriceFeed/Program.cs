using FinPriceFeed.Configuration;
using FinPriceFeed.Core.Configuration;
using FinPriceFeed.ExternalClients;
using FinPriceFeed.ExternalProviders;
using FinPriceFeed.ExternalProviders.TwelveData.Service;
using FinPriceFeed.Service;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.Get<Settings>();
var selectedProvider = settings.ExternalProviderSettings.Selected;
builder.Configuration.Bind(settings);
builder.Services.AddSingleton(settings);
builder.Services.AddSingleton(settings.ExternalProviderSettings);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.DefaultBufferSize = 64 * 1024;
    options.JsonSerializerOptions.WriteIndented = false;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

});
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.AddSwaggerGen(options =>
{
    options.DescribeAllParametersInCamelCase();
    options.IgnoreObsoleteActions();
    options.IgnoreObsoleteProperties();
    options.SchemaFilter<EnumSchemaFilter>();
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddExternalProviders(settings);
builder.Services.AddSignalR();
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IFinInstrumentService, FinInstrumentService>();
builder.Services.AddSingleton<IExternalProviderWebSocketService, TwelveDataWebSocketService>();
builder.Services.AddSingleton<ILivePriceWebSocketService, LivePriceWebSocketService>();

var app = builder.Build();

app.UseCors("AllowAll");  
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseResponseCompression();
app.UseWebSockets();

app.MapHub<LivePricesHub>("/livePricesHub");
app.MapControllers();

app.Run();