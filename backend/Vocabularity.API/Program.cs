using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Vocabularity.Core.Configuration;
using Vocabularity.Service.Dictionary.Implementation;
using Vocabularity.Service.Dictionary.Interfaces;
using Vocabularity.Service.Language.Implementation;
using Vocabularity.Service.Language.Interfaces;
using Vocabularity.Service.User.Implementation;
using Vocabularity.Service.User.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CosmosConfig>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddTransient<CosmosClient>(db =>
{
    var cosmosConfig = db.GetRequiredService<IOptions<CosmosConfig>>().Value;
    return new CosmosClient(cosmosConfig.EndPointUri, cosmosConfig.AuthorizationKey);
});

builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDictionaryRepository, DictionaryRepository>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapControllers();

app.Run();