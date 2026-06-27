using Microsoft.EntityFrameworkCore;
using Vocabularity.Core.Data;
using Vocabularity.Service.Dictionary.Implementation;
using Vocabularity.Service.Dictionary.Interfaces;
using Vocabularity.Service.Language.Implementation;
using Vocabularity.Service.Language.Interfaces;
using Vocabularity.Service.User.Implementation;
using Vocabularity.Service.User.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<VocabularityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDictionaryRepository, DictionaryRepository>();

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VocabularityDbContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => "Hello World!");
app.MapControllers();

app.Run();
