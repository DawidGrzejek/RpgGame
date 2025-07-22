netstat -ano | findstr :4200

$idNumber = read-host 'Podaj <PID> procesu do zabicia (wciśniej enter aby pominąć): '

if (![string]::IsNullOrEmpty($idNumber))
{
    Write-Host "Zabijam proces o numerze <PID>: $idNumber"
    Stop-Process $idNumber -Force
}
else
{
    Write-Host "Przechodzę do uruchomienia aplikacji Angular..."
}

clear-host
$PSStyle.OutputRendering = 'Ansi'

$asciiArt = @"
     __ __  ____ ___________   __             
  __/ // /_/ __ \ ___/__  / | / /___ _      __
 /_  _  __/ /_/ \__ \  / /  |/ / __ \ | /| / /
/_  _  __/ ____/__/ / / / /|  / /_/ / |/ |/ / 
 /_//_/ /_/   /____/ /_/_/ |_/\____/|__/|__/  
                                              
"@
Write-Output "`e[5;36m$asciiArt`e[0m";

Set-Location C:\Users\dawid\source\repos\DawidGrzejek\DesignPatterns\src\RpgGame.AngularUI\rpg-game-ui

$nodeVersion = node --version
$npmVersion = npm --version

Write-Host "Node.js version: $($nodeVersion)"
Write-Host "npm version: $($npmVersion)"

Write-Host "Starting Angular development server..."
Write-Host "Angular UI will be available at: http://localhost:4200"
Write-Host ""
Write-Host "Note: Start the Web API from Visual Studio"

npm start


# Navigate to solution root directory
#Set-Location C:\Users\dawid\source\repos\DawidGrzejek\DesignPatterns\src\RpgGame.Infrastructure

# Create initial migration for PostgreSQL
#dotnet ef migrations add InitialPostgreSQLMigration --project src/RpgGame.Infrastructure --startup-project src/RpgGame.WebApi --context GameDbContext
#dotnet ef database update  --project src/RpgGame.Infrastructure --startup-project src/RpgGame.WebApi --context GameDbContext