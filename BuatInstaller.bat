@echo off
setlocal
cd /d "%~dp0"
title Buat Installer SMK MART

set VERSI=1.0.1
if not "%~1"=="" set VERSI=%~1

where dotnet >nul 2>nul
if errorlevel 1 (
  echo [GAGAL] .NET SDK belum terpasang. Pasang dari https://dotnet.microsoft.com/download
  goto :akhir
)

set ISCC=
for %%P in ("%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe" "%ProgramFiles%\Inno Setup 6\ISCC.exe" "%LocalAppData%\Programs\Inno Setup 6\ISCC.exe") do (
  if exist %%P set ISCC=%%~P
)

if "%ISCC%"=="" (
  echo Inno Setup 6 belum ada, memasang lewat winget...
  winget install --id JRSoftware.InnoSetup -e --silent --accept-package-agreements --accept-source-agreements
  for %%P in ("%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe" "%ProgramFiles%\Inno Setup 6\ISCC.exe" "%LocalAppData%\Programs\Inno Setup 6\ISCC.exe") do (
    if exist %%P set ISCC=%%~P
  )
)
if "%ISCC%"=="" (
  echo [GAGAL] Inno Setup 6 tidak ditemukan. Pasang dari https://jrsoftware.org/isdl.php lalu jalankan lagi.
  goto :akhir
)

echo.
echo === [1/2] Publish aplikasi (versi %VERSI%) ===
if exist publish rmdir /s /q publish
dotnet publish src\SmkMart\SmkMart.csproj -c Release -r win-x64 --self-contained true -p:Version=%VERSI% -o publish
if errorlevel 1 (
  echo [GAGAL] Publish gagal.
  goto :akhir
)

echo.
echo === [2/2] Membuat installer ===
"%ISCC%" /DMyAppVersion=%VERSI% installer\SmkMart.iss
if errorlevel 1 (
  echo [GAGAL] Kompilasi installer gagal.
  goto :akhir
)

echo.
echo SELESAI! Installer ada di folder installer-output
start "" "%~dp0installer-output"

:akhir
echo.
pause
