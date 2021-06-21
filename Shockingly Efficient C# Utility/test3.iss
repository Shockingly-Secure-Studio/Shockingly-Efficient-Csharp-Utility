; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Shockingly Efficient C# Utility"
#define MyAppVersion "0.1"
#define MyAppPublisher "S.E.C.U. Studio"
#define MyAppURL "https://secu.studio/"
#define MyAppExeName "Shockingly Efficient C# Utility.exe"
#define MyAppAssocName MyAppName + " File"
#define MyAppAssocExt ".myp"
#define MyAppAssocKey StringChange(MyAppAssocName, " ", "") + MyAppAssocExt

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{923948F2-7C7D-4CB5-886E-052509400AC2}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
ChangesAssociations=yes
DisableProgramGroupPage=yes
LicenseFile=C:\Users\tudual\Desktop\licence.txt
;InfoBeforeFile=C:\Users\tudual\Desktop\avant intallation.txt
;InfoAfterFile=C:\Users\tudual\Desktop\apre intallation.txt
;Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
OutputDir=C:\Users\tudual\Desktop\buildexe\exe
OutputBaseFilename=Shockingly_Efficient_C#_Utility_Setup
SetupIconFile=C:\Users\tudual\Desktop\buildTest\S2.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "norwegian"; MessagesFile: "compiler:Languages\Norwegian.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "install_python"; Description: "{cm:InstallPython}" ; GroupDescription: "{cm:AdditionalLibrary}"
Name: "install_ruby"; Description: "{cm:InstallRuby}"   ; GroupDescription: "{cm:AdditionalLibrary}"
[Files]
Source: "C:\Users\tudual\Desktop\buildexe\b1\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\tudual\Desktop\buildexe\b1\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Registry]
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocExt}\OpenWithProgids"; ValueType: string; ValueName: "{#MyAppAssocKey}"; ValueData: ""; Flags: uninsdeletevalue
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}"; ValueType: string; ValueName: ""; ValueData: "{#MyAppAssocName}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},0"
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""
Root: HKA; Subkey: "Software\Classes\Applications\{#MyAppExeName}\SupportedTypes"; ValueType: string; ValueName: ".myp"; ValueData: ""

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\python-3.9.5-amd64.exe";Check: python_3_9_5_is_not_installed();Tasks: install_python;Parameters: "--install";Flags: skipifsilent  runascurrentuser
Filename: "{app}\rubyinstaller-devkit-2.7.3-1-x64.exe"; Check: ruby_2_7_3_1_is_not_installed();Tasks: install_ruby;Parameters: "--install";Flags: skipifsilent runascurrentuser 
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: postinstall skipifsilent nowait
[Code]
{Check existence of key in registry and check version string.
return true if Python is installed and version is correct}
function checkKey(regpart: integer; key: string; version: string) : Boolean;
var
  installedVersion: string;
begin
   Result :=
     { Check if key exists }
     RegKeyExists(regpart, Key) and
     { try to get version string }
     RegQueryStringValue(regpart, key, 'Version', installedVersion) and
     { check version string }
     (version = installedVersion);
end;
{ Return true if python 3.9.5 bit is not installed }
function python_3_9_5_is_not_installed() : Boolean;
var
  Key : string;
  Version: string;
begin
   { check registry }
   Key := 'Software\Python\PythonCore\3.9';
   Version := '3.9.5';
   Result :=
     { Check all user 32-bit installation}
     (not checkKey(HKLM32, Key, Version)) and
     { Check current user 32-bit installation}
     (not checkKey(HKCU32, Key, Version)) and
     { Check all user 64-bit installation}
     (not checkKey(HKLM64, Key, Version)) and
     { Check current user 64-bit installation}
     (not checkKey(HKCU64, Key, Version));
end;
{ Return true if python 3.6.8 bit is not installed }
function ruby_2_7_3_1_is_not_installed() : Boolean;
var
  Key : string;
  Version: string;
begin
   { check registry }
   Key := 'Software\RubyInstaller\MRI\2.7.3';
   Version := '2.7.3-1';
   Result :=
     { Check all user 32-bit installation}
     (not RegKeyExists(HKLM32, Key)) and
     { Check current user 32-bit installation}
     (not RegKeyExists(HKCU32, Key)) and
     { Check all user 64-bit installation}
     (not RegKeyExists(HKLM64, Key)) and
     { Check current user 64-bit installation}
     (not RegKeyExists(HKCU64, Key));
end;
[CustomMessages]
InstallPython=Install Python 3.9.5 (required)
InstallRuby=Install Ruby 2.7.3-1 (required)
AdditionalLibrary=Additional library:
french.InstallPython=Installer Python 3.9.5 (n�cessaire)
french.InstallRuby=Installer Ruby 2.7.3-1 (n�cessaire)
french.AdditionalLibrary=Logiciels � installer:


