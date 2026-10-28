@echo off
echo ========================================================
echo   Starting AuraMart E-Commerce Store (.NET 8.0 MVC)...
echo ========================================================
echo Opening browser at http://localhost:5220 ...
start http://localhost:5220
echo.
echo Running Web Application... Press Ctrl+C to stop.
dotnet run --launch-profile http
pause
