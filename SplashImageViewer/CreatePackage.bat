@echo off

set "timestamp=http://timestamp.digicert.com"
set "publishDir=.\bin\publish"

del /S /Q %publishDir% >nul 2>&1
rmdir /S /Q %publishDir% >nul 2>&1
del /S /Q ".\bin\Splash.zip" >nul 2>&1

dotnet publish -c Release -v m -o "%publishDir%"

if exist "%publishDir%" (
    signtool.exe sign /fd sha256 /a /f %CodesignCertPath% "%publishDir%\Splash.exe"
    signtool.exe timestamp /tr "%timestamp%" /td sha256 "%publishDir%\Splash.exe"
    signtool.exe sign /fd sha256 /a /f %CodesignCertPath% "%publishDir%\Splash.dll"
    signtool.exe timestamp /tr "%timestamp%" /td sha256 "%publishDir%\Splash.dll"
    signtool.exe sign /fd sha256 /a /f %CodesignCertPath% "%publishDir%\en\Splash.resources.dll"
    signtool.exe timestamp /tr "%timestamp%" /td sha256 "%publishDir%\en\Splash.resources.dll"
    signtool.exe sign /fd sha256 /a /f %CodesignCertPath% "%publishDir%\ru\Splash.resources.dll"
    signtool.exe timestamp /tr "%timestamp%" /td sha256 "%publishDir%\ru\Splash.resources.dll"
    7za.exe a -mx0 -tzip ".\bin\Splash.zip" "%publishDir%\*" -xr!*.pdb
    7za.exe h -scrcSHA256 ".\bin\Splash.zip" > ".\bin\sha256.txt"
)

pause
