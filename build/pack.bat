dotnet build -c Release
del "Release\*.nupkg" /f /q
dotnet pack -c Release
dotnet nuget push -s https://www.nuget.org/ -k xxx "Release\*.nupkg"
pause