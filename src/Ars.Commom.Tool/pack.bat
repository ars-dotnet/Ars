dotnet build -c Release
del "bin\Release\*.nupkg" /f /q
dotnet pack -c Release
dotnet nuget push -s https://www.nuget.org/ -k oy2cw2xmkvlz63sab5hazqd2rkwz7tbe7inslboyfpucai "bin\Release\*.nupkg"
pause