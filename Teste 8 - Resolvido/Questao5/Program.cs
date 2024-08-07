using FluentAssertions.Common;
using MediatR;
using Microsoft.Data.Sqlite;
using Questao5.Application.Services;
using Questao5.Infrastructure.Database.CommandStore;
using Questao5.Infrastructure.Database.QueryStore;
using Questao5.Infrastructure.Sqlite;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<ISaldoStore, SaldoStore>();
builder.Services.AddScoped<IMovimentacaoStore, MovimentacaoStore>();

// sqlite
builder.Services.AddSingleton(new DatabaseConfig { Name = builder.Configuration.GetValue<string>("DatabaseName", "Data Source=database.sqlite") });
builder.Services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();

// Adicione serviços de infraestrutura
builder.Services.AddScoped<IDbConnection>(sp => new SqliteConnection(builder.Configuration.GetConnectionString("DatabaseName")));

// Adicione serviços de repositório
builder.Services.AddScoped<IMovimentacaoStore, MovimentacaoStore>();
builder.Services.AddScoped<ISaldoStore, SaldoStore>();
builder.Services.AddScoped<IContaCorrenteStore, ContaCorrenteStore>();
builder.Services.AddScoped<IIdempotenciaStore, IdempotenciaStore>();
builder.Services.AddTransient<IIdempotenciaStore, IdempotenciaStore>();

// Adicione serviços de aplicação
//builder.Services.AddScoped<IMovimentacaoService, MovimentacaoService>();
//builder.Services.AddScoped<ISaldoService, SaldoService>();


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

// sqlite
#pragma warning disable CS8602 // Dereference of a possibly null reference.
app.Services.GetService<IDatabaseBootstrap>().Setup();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

app.Run();

// Informações úteis:
// Tipos do Sqlite - https://www.sqlite.org/datatype3.html


