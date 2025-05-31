@echo off
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\makeappx.exe" pack /d "All" /p "AppPackages\SoundReaver_Music.msix"
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\signtool.exe" sign /n "Open Source Developer, David Sungaila" /tr "http://time.certum.pl" /fd certHash /td certHash /fdchw /tdchw /v "AppPackages\SoundReaver_Music.msix"
pause