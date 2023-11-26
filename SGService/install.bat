@echo off
REM SET THIS (settings file in THIS folder):
set parameterfile=SGServiceSettings.json
REM ===========================================
echo - Service parameter file= %~dp0\%parameterfile%
echo.
echo - Create eventlog source SystemsGardenService
rem %~dp0\ConsoleApp1.exe
echo Wait a little...
timeout 3 > NUL
echo - Stop and delete the service...
sc delete TestService
sc stop SGService
sc delete SGService
timeout 2 > NUL
echo - Create and start service SGService...
sc create SGService Start="delayed-auto" DisplayName= "Systems Garden Service" binPath="%~dp0\bin\Release\SGService.exe %~dp0 %parameterfile%"
sc start SGService
echo.
echo Service parameter file= %~dp0\%parameterfile%
echo ready! Press NewLine
pause
