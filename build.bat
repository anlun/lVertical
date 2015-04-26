@echo off

cls
powershell -Command "(New-Object Net.WebClient).DownloadFile('http://nuget.org/nuget.exe', 'NuGet.exe')"
"NuGet.exe" "Install" "FAKE" "-OutputDirectory" "tools" "-ExcludeVersion"
pause