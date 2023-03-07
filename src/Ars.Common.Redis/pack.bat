dotnet build -c Release
del "bin\Release\*.nupkg" /f /q
dotnet pack -c Release
dotnet nuget push -s https://www.nuget.org/ -k oy2bnyejtxplnhkls23abzfxqmpkpofexdiumxkfir2kbe "bin\Release\*.nupkg"
pause