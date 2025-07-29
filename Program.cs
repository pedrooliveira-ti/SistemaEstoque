using ControleEstoqueRoupas.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco baseada no ambiente
var connectionString = builder.Environment.IsDevelopment() 
    ? "Data Source=estoque.db"
    : Environment.GetEnvironmentVariable("DATABASE_URL") ?? "Data Source=estoque.db";

builder.Services.AddDbContext<EstoqueContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Auto-migração para produção
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EstoqueContext>();
    context.Database.EnsureCreated();
}

// Configurações para produção
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Produtos}/{action=Index}/{id?}");

app.Run();


