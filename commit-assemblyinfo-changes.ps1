# only need to commit assembly info changes when build is NOT for a pull-request
if ($env:appveyor_pull_request_number)
{
    'Skip committing assembly info changes as this is a PR build...' | Write-Host -ForegroundColor White
}
else
{
    # updated assembly info files   
    git add "source\Windows.Storage.Streams\Properties\AssemblyInfo.cs"
    git commit -m "Update assembly info file for v$env:GitVersion_NuGetVersionV2"
    git push origin --porcelain -q > $null
    
    'Updated assembly info...' | Write-Host -ForegroundColor White -NoNewline
    'OK' | Write-Host -ForegroundColor Green

    # this assembly does not have native implementation, no updates requried in that repo
}
