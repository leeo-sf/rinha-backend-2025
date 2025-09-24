using RinhaBackend.Api.Config;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddDbContextConfiguration(configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

builder.Services.ConfigureAppDependencies(configuration);

builder.Services.ConfigureServices(configuration);

builder.Services.AddMediatorConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();