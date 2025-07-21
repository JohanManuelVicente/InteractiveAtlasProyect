using InteractiveAtlas.Domain.Entities;
using InteractiveAtlas.Infrastucture.Data;
using InteractiveAtlas.Infrastucture.Repository;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InteractiveAtlasDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MainConnection")));

// Add services to the container.
////Use the same instance
//builder.Services.AddSingleton<ProvinceRepository>();
//// create new instance for each repository
//builder.Services.AddTransient<ProvinceRepository>();
//create the nwe instance for each repository


builder.Services.AddScoped<ProvinceRepository>();
builder.Services.AddScoped<TypicalProductsRepository>();
builder.Services.AddScoped<TouristAttractionRepository>();
builder.Services.AddScoped<QuizQuestionRepository>();
builder.Services.AddScoped<QuizAnswerRepository>();



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
    
    ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
