dotnet tool update --global dotnet-ef --version 7.0.11
dotnet tool restore
dotnet build
dotnet ef --startup-project ../../Sic.Chekam.Uaa.Host/ database update --context UaaDbContext
pause