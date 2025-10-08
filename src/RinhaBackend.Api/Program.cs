using RinhaBackend.Api.Infra;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddDbContextConfiguration(configuration);

builder.Services.AddSwaggerGen();

builder.Services.ConfigureAppDependencies(configuration);

builder.Services.ConfigureServices(configuration);

builder.Services.ConfigureWorkerServices();

var app = builder.Build();

app.AddEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();