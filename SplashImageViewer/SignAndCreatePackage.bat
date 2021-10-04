@echo off

set "cert=C:\GitSources\CodeSign\Certificates\Illuminati_Software_Inc_Code_Sign.p12"
set "timestamp=http://timestamp.digicert.com"

set "bin=.\bin"
set "src=%bin%\Release"

signtool.exe sign /fd sha256 /a /f "%cert%" "%src%\Splash.exe"
signtool.exe timestamp /tr "%timestamp%" /td sha256 "%src%\Splash.exe"

signtool.exe sign /fd sha256 /a /f "%cert%" "%src%\Splash.dll"
signtool.exe timestamp /tr "%timestamp%" /td sha256 "%src%\Splash.dll"

signtool.exe sign /fd sha256 /a /f "%cert%" "%src%\ApplicationUpdater.dll"
signtool.exe timestamp /tr "%timestamp%" /td sha256 "%src%\ApplicationUpdater.dll"

signtool.exe sign /fd sha256 /a /f "%cert%" "%src%\ref\Splash.dll"
signtool.exe timestamp /tr "%timestamp%" /td sha256 "%src%\ref\Splash.dll"

signtool.exe sign /fd sha256 /a /f "%cert%" "%src%\en\Splash.resources.dll"
signtool.exe timestamp /tr "%timestamp%" /td sha256 "%src%\en\Splash.resources.dll"

signtool.exe sign /fd sha256 /a /f "%cert%" "%src%\ru\Splash.resources.dll"
signtool.exe timestamp /tr "%timestamp%" /td sha256 "%src%\ru\Splash.resources.dll"

7za.exe a -mx0 -tzip "%bin%\Splash.zip" "%src%\*"
7za.exe h -scrcSHA256 "%bin%\Splash.zip" > "%bin%\sha256.txt"

pause
