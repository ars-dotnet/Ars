dotnet build -c Release
del "bin\Release\*.nupkg" /f /q
dotnet pack -c Release
dotnet nuget push -s https://www.nuget.org/ -k oy2cbre423ratr5znzmqjyls6oymwkyunkiajo34oioq5u "bin\Release\*.nupkg"
pause