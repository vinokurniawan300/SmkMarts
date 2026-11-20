; Inno Setup script untuk SMK MART
; Cara kompilasi lokal (Inno Setup 6):
;   1. dotnet publish src\SmkMart\SmkMart.csproj -c Release -r win-x64 --self-contained true -o publish
;   2. Buka file ini di Inno Setup Compiler lalu tekan Ctrl+F9
;      atau: ISCC.exe /DMyAppVersion=1.0.1 installer\SmkMart.iss
; Hasil: installer-output\SmkMart-Setup-<versi>.exe

#ifndef MyAppVersion
  #define MyAppVersion "1.0.1"
#endif
#define MyAppName "SMK MART"
#define MyAppExe "SmkMart.exe"

[Setup]
AppId={{7B1E2C54-3A8D-4F6B-9D21-5C0A8E9F4B73}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher=SMK MART
DefaultDirName={autopf}\SmkMart
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=..\installer-output
OutputBaseFilename=SmkMart-Setup-{#MyAppVersion}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
SetupIconFile=..\src\SmkMart\Assets\SmkMart.ico
UninstallDisplayIcon={app}\{#MyAppExe}

[Languages]
; Inno Setup 6 tidak membawa bahasa Indonesia, jadi dipakai bahasa Inggris bawaan.
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Buat pintasan di Desktop"; GroupDescription: "Pintasan:"

[Files]
Source: "..\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExe}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExe}"; Description: "Jalankan {#MyAppName}"; Flags: nowait postinstall skipifsilent
