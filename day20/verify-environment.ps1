# 檢查 Docker 狀態
Write-Host "Checking Docker version..." -ForegroundColor Green
docker --version

# 檢查 Docker Compose 版本
Write-Host "Checking Docker Compose version..." -ForegroundColor Green  
docker-compose --version

# 檢查 Docker 服務狀態
Write-Host "Checking Docker service status..." -ForegroundColor Green
docker system info --format "table {{.Name}}\t{{.Status}}"

# 測試容器啟動
Write-Host "Testing container startup..." -ForegroundColor Green
docker run --rm hello-world

# 檢查可用映像檔
Write-Host "Checking available images..." -ForegroundColor Green
docker images

# 檢查執行中的容器
Write-Host "Checking running containers..." -ForegroundColor Green
docker ps

# 檢查 Docker 資源使用情況
Write-Host "Checking Docker resource usage..." -ForegroundColor Green
docker system df

# 環境驗證完成
Write-Host "Environment verification completed!" -ForegroundColor Yellow