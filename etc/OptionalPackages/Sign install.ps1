$cert = Get-ChildItem Cert:\CurrentUser\My | Where-Object {
    $_.Subject -like '*CN="Open Source Developer, David Sungaila"*' -and $_.HasPrivateKey
} | Sort-Object NotAfter -Descending | Select-Object -First 1
Set-AuthenticodeSignature -FilePath ".\install.ps1" -Certificate $cert -TimestampServer "http://time.certum.pl"
pause