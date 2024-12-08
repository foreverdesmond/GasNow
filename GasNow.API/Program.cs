﻿using GasNow.Business;
using GasNow.Module;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");

builder.Services.AddDbContext<GasNowDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddSingleton<IBusinessFactory, BusinessFactory>();

builder.Services.AddScoped<GasFeeBusiness>();
builder.Services.AddScoped<PriceBussiness>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin", builder =>
//    builder.WithOrigins("http://localhost:3000") 
//    .AllowAnyMethod()
//    .AllowAnyHeader());
//});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    builder.WithOrigins(
        "http://203.211.107.7",
        "http://localhost:3000",
        "http://localhost",
        "http://127.0.0.1")
    .AllowAnyHeader()
    .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();

