dotnet build -c Release
del "bin\Release\*.nupkg" /f /q
dotnet pack -c Release
dotnet nuget push -s https://www.nuget.org/ -k oy2fou5qw3jrlhph5fnqjhcj4iqtxnyu7p37xakyc6pjiy "bin\Release\*.nupkg"
pause