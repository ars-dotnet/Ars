dotnet build -c Release
del "bin\Release\*.nupkg" /f /q
dotnet pack -c Release
dotnet nuget push -s https://www.nuget.org/ -k oy2lm6noo22psckvvtqamiohsywirxbulpokqbg2npcwbu "bin\Release\*.nupkg"
pause