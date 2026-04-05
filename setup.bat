dotnet new sln -n StudyTimerApp
dotnet sln StudyTimerApp.sln add StudyTimerApp.csproj
dotnet new classlib -n StudyTimerApp.Core -f net10.0
dotnet sln StudyTimerApp.sln add StudyTimerApp.Core/StudyTimerApp.Core.csproj
dotnet new classlib -n StudyTimerApp.Application -f net10.0
dotnet sln StudyTimerApp.sln add StudyTimerApp.Application/StudyTimerApp.Application.csproj
dotnet new classlib -n StudyTimerApp.Infrastructure -f net10.0-windows
dotnet sln StudyTimerApp.sln add StudyTimerApp.Infrastructure/StudyTimerApp.Infrastructure.csproj
dotnet add StudyTimerApp.csproj reference StudyTimerApp.Application/StudyTimerApp.Application.csproj
dotnet add StudyTimerApp.csproj reference StudyTimerApp.Infrastructure/StudyTimerApp.Infrastructure.csproj
dotnet add StudyTimerApp.Application/StudyTimerApp.Application.csproj reference StudyTimerApp.Core/StudyTimerApp.Core.csproj
dotnet add StudyTimerApp.Infrastructure/StudyTimerApp.Infrastructure.csproj reference StudyTimerApp.Core/StudyTimerApp.Core.csproj
dotnet add StudyTimerApp.Infrastructure/StudyTimerApp.Infrastructure.csproj reference StudyTimerApp.Application/StudyTimerApp.Application.csproj
