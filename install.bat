if exist %windir%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe (
    %windir%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe "%~dp0GateShell.dll" /codebase
) else (
    %windir%\Microsoft.NET\Framework\v4.0.30319\regasm.exe "%~dp0GateShell.dll" /codebase
)
