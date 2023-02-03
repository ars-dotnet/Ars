dotnet build -c Release
del "bin\Release\*.nupkg" /f /q
dotnet pack -c Release
dotnet nuget push -s https://www.nuget.org/ -k oy2d5ehwzhlpkjtifloanvthsfff5i2ujlkc7fpxhsblli "bin\Release\*.nupkg"
pause