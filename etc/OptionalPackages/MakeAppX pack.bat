@echo off
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\makeappx.exe" pack /d "Drowned Abbey" /p "AppPackages\SoundReaver_Music_DrownedAbbey.msix"
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\makeappx.exe" pack /d "Human Citadel" /p "AppPackages\SoundReaver_Music_HumanCitadel.msix"
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\makeappx.exe" pack /d "Necropolis" /p "AppPackages\SoundReaver_Music_Necropolis.msix"
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\makeappx.exe" pack /d "Raziel's Stronghold" /p "AppPackages\SoundReaver_Music_RazielsStronghold.msix"
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\makeappx.exe" pack /d "Ruined City" /p "AppPackages\SoundReaver_Music_RuinedCity.msix"
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\makeappx.exe" pack /d "Sarafan Tomb" /p "AppPackages\SoundReaver_Music_SarafanTomb.msix"
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\makeappx.exe" pack /d "Silenced Cathedral" /p "AppPackages\SoundReaver_Music_SilencedCathedral.msix"
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\makeappx.exe" pack /d "The Lighthouse" /p "AppPackages\SoundReaver_Music_TheLighthouse.msix"
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\makeappx.exe" pack /d "Underworld" /p "AppPackages\SoundReaver_Music_Underworld.msix"
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\signtool.exe" sign /n "Open Source Developer, David Sungaila" /tr "http://time.certum.pl" /fd certHash /td certHash /fdchw /tdchw /v "AppPackages\SoundReaver_Music_DrownedAbbey.msix" "AppPackages\SoundReaver_Music_HumanCitadel.msix" "AppPackages\SoundReaver_Music_Necropolis.msix" "AppPackages\SoundReaver_Music_RazielsStronghold.msix" "AppPackages\SoundReaver_Music_RuinedCity.msix" "AppPackages\SoundReaver_Music_SarafanTomb.msix" "AppPackages\SoundReaver_Music_SilencedCathedral.msix" "AppPackages\SoundReaver_Music_TheLighthouse.msix" "AppPackages\SoundReaver_Music_Underworld.msix"
pause