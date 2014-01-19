@echo OFF

if %1.=="." (
    echo Target dir unspecified
    PAUSE
    goto :Exit
)

if not exist %1 mkdir %1

@echo ON

xcopy /Y "%~dp0App\bin\Release\GateDiff.exe" %1
xcopy /Y "%~dp0App\bin\Release\GateDiff.exe.config" %1

xcopy /Y "%~dp0Extension\bin\Release\GateShell.dll" %1
xcopy /Y "%~dp0Extension\bin\Release\SharpShell.dll" %1

xcopy /Y "%~dp0install.bat" %1
xcopy /Y "%~dp0uninstall.bat" %1

:Exit