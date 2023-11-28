@echo off

del /S /Q .\bin\publish >nul 2>&1
rmdir /S /Q .\bin\publish >nul 2>&1

dotnet publish -c Release -o .\bin\publish

pause
