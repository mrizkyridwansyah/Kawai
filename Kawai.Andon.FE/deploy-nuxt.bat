@echo off
setlocal enabledelayedexpansion

:: Set path project Nuxt kamu
set "PROJECT_DIR=%~dp0"

:: Path output di Nginx (ubah jika beda)
set "NGINX_HTML_DIR=D:\Data Laptop Rizky\Software\nginx-1.26.2\nginx-1.26.2\html"

:: Masuk ke direktori project
cd /d "%PROJECT_DIR%"

:: Ambil versi dari package.json
for /f "usebackq tokens=*" %%v in (`powershell -Command "(Get-Content package.json -Raw | ConvertFrom-Json).version"`) do (
    set "VERSION=%%v"
)

:: Jalankan build Nuxt
echo 📦 Building Nuxt project...
call npm run generate
IF ERRORLEVEL 1 (
    echo ❌ Failed to build Nuxt project.
    exit /b 1
)

:: Nama folder tujuan
set "DEST_FOLDER=nuxt-%VERSION%"
set "DEST_PATH=%NGINX_HTML_DIR%\%DEST_FOLDER%"

:: Hapus folder tujuan kalau sudah ada
IF EXIST "%DEST_PATH%" (
    echo 🗑️  Deleting old folder: %DEST_PATH%
    rmdir /s /q "%DEST_PATH%"
)

:: Salin hasil build
echo 📂 Copying to Nginx HTML directory: %DEST_PATH%
xcopy ".output\public\*" "%DEST_PATH%\" /E /I /Y /Q

echo.
echo ✅ Nuxt deployed to: %DEST_PATH%
endlocal
pause
