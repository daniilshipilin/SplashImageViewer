@echo off

set "cert=%CodesignCertPath%"
set "timestamp=http://timestamp.digicert.com"

set "bin=.\bin"
set "src=%bin%\Release\publish"

signtool.exe sign /fd sha256 /a /f "%cert%" "%src%\Splash.exe"
signtool.exe timestamp /tr "%timestamp%" /td sha256 "%src%\Splash.exe"

signtool.exe sign /fd sha256 /a /f "%cert%" "%src%\Splash.dll"
signtool.exe timestamp /tr "%timestamp%" /td sha256 "%src%\Splash.dll"

signtool.exe sign /fd sha256 /a /f "%cert%" "%src%\en\Splash.resources.dll"
signtool.exe timestamp /tr "%timestamp%" /td sha256 "%src%\en\Splash.resources.dll"

signtool.exe sign /fd sha256 /a /f "%cert%" "%src%\ru\Splash.resources.dll"
signtool.exe timestamp /tr "%timestamp%" /td sha256 "%src%\ru\Splash.resources.dll"

del /S /Q "%bin%\Splash.zip" >nul 2>&1
7za.exe a -mx0 -tzip "%bin%\Splash.zip" "%src%\*"
7za.exe h -scrcSHA256 "%bin%\Splash.zip" > "%bin%\sha256.txt"

pause
