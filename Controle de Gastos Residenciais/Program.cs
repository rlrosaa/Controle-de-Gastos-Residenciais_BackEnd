using Microsoft.EntityFrameworkCore;
using ControleGastosResiduais.Infrastructure.Data;
using ControleGastosResiduais.Infrastructure.Repositories;
using ControleGastosResiduais.Domain.Interfaces;
using ControleGastosResiduais.Domain.Services;
using ControleGastosResiduais.Application.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger
builder.Services.AddSwaggerGen();

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configuração do DbContext com SQLite
builder.Services.AddDbContext<GastosResiduaisDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro de repositórios
builder.Services.AddScoped<IPessoaRepository, PessoaRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();

// Registro de serviços de domínio
builder.Services.AddScoped<TransacaoService>();

// Registro de serviços de aplicação
builder.Services.AddScoped<PessoaAppService>();
builder.Services.AddScoped<CategoriaAppService>();
builder.Services.AddScoped<TransacaoAppService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    
    // Configuração do Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aplicar a política de CORS
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
