Write-Host "========================================================" -ForegroundColor Green
Write-Host "  Starting AuraMart E-Commerce Store (.NET 8.0 MVC)..." -ForegroundColor Green
Write-Host "========================================================" -ForegroundColor Green
Write-Host "Opening browser at http://localhost:5220 ..." -ForegroundColor Cyan
Start-Process "http://localhost:5220"
Write-Host "`nRunning Web Application... Press Ctrl+C to stop.`n" -ForegroundColor Yellow
dotnet run --launch-profile http
