using Differ.CustomBinders;
using Differ.CustomBinders.Deserializer;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CustomContentTypeProvider>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<CustomContentTypeBinder>();
builder.Services.AddSingleton<IBase64JsonDeserializer, Base64JsonToObjectDeserializer>();
builder.Services.Configure<MvcOptions>(opt => opt.ModelBinderProviders.Add(new CustomContentTypeProvider()));


builder.Services.AddControllers();
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