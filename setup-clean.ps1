# Script de automação para ControleEstoqueRoupas
Write-Host "=== Iniciando setup do ControleEstoqueRoupas ===" -ForegroundColor Green

# 1. Limpar builds anteriores
Write-Host "1. Limpando builds anteriores..." -ForegroundColor Yellow
dotnet clean

# 2. Remover pastas bin e obj
Write-Host "2. Removendo pastas bin e obj..." -ForegroundColor Yellow
Remove-Item -Recurse -Force bin -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force obj -ErrorAction SilentlyContinue

# 3. Restaurar dependências
Write-Host "3. Restaurando dependências..." -ForegroundColor Yellow
dotnet restore

# 4. Remover banco e migrações anteriores
Write-Host "4. Removendo banco e migrações anteriores..." -ForegroundColor Yellow
Remove-Item -Force estoque.db -ErrorAction SilentlyContinue
dotnet ef database drop --force
Remove-Item -Recurse -Force Migrations -ErrorAction SilentlyContinue

# 5. Criar nova migração
Write-Host "5. Criando nova migração..." -ForegroundColor Yellow
dotnet ef migrations add InitialCreate

# 6. Aplicar migração ao banco
Write-Host "6. Aplicando migração ao banco..." -ForegroundColor Yellow
dotnet ef database update

# 7. Build do projeto
Write-Host "7. Fazendo build do projeto..." -ForegroundColor Yellow
dotnet build

# 8. Executar o projeto
Write-Host "8. Executando o projeto..." -ForegroundColor Green
Write-Host "Acesse: http://localhost:5000 ou https://localhost:5001" -ForegroundColor Cyan
Write-Host "Pressione Ctrl+C para parar o servidor" -ForegroundColor Cyan
dotnet run