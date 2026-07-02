@echo off
setlocal

REM Copy project to a clean location
set DEST=%USERPROFILE%\Projects\truck-logistics
echo Creating project at %DEST% ...
if not exist "%USERPROFILE%\Projects" mkdir "%USERPROFILE%\Projects"
xcopy /E /I /Y "%~dp0." "%DEST%"

REM Initialize git
cd /d "%DEST%"
git init
git add .
git commit -m "Initial commit: TruckLogix - Truck Logistics App (React.js, C# .NET 8, SQL Server)"

echo.
echo =====================================================
echo  Git initialized! Project is at: %DEST%
echo  Next: Open GitHub Desktop and publish this repo.
echo =====================================================
pause
