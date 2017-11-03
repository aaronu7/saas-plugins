

# fetch our version
$versionFile = $PSScriptRoot + "\nuget_version";
$ver = Get-Content $versionFile

$sVersionLevel = Read-Host "Current: $ver. Which version level to increment 1.2.3 (1/2/3)" 
[int]$versionLevel = [convert]::ToInt32($sVersionLevel, 10)

#$versionLevel = 1

# remove existing *.nupkg files
$prjPath = $PSScriptRoot + "\MetaIS";
get-childitem $prjPath -Filter *.nupkg | foreach ($_) {remove-item $_.fullname}


# increment the version
$verParts = $ver.split(".")
if($versionLevel -eq 1) {
    [int]$varMajor = [convert]::ToInt32($verParts[0], 10)
    $varMajor = $varMajor + 1;
    $verParts[0] = $varMajor
    $verParts[1] = 0;
    $verParts[2] = 0;
} elseif($versionLevel -eq 2) {
    [int]$varMinor = [convert]::ToInt32($verParts[1], 10)
    $varMinor = $varMinor + 1;
    $verParts[1] = $varMinor
    $verParts[2] = 0;
} elseif($versionLevel -eq 3) {
    [int]$varInc = [convert]::ToInt32($verParts[2], 10)
    $varInc = $varInc + 1;
    $verParts[2] = $varInc
}
$ver = $verParts -join '.'

# runner packer (override the version number in nuspec file)
nuget pack "$prjPath\MetaIS.csproj" -Version $ver -OutputDirectory "$prjPath";

# fetch our key from non-shared keyfile
$rootPath = (get-item $prjPath).parent.Parent.FullName;
$keyFile = "$rootPath\nuget_key.txt";
$key = Get-Content $keyFile

Write-Host($ver)
Write-Host($key)

# run pusher
$confirmation = Read-Host "Are you sure you want to push this version $ver (y/n)" 
if ($confirmation -eq 'y' -or $confirmation -eq 'Y') {
    # output the new version
    $ver | Out-File $versionFile

    nuget push "$prjPath\*.nupkg" $key -Source https://www.nuget.org/api/v2/package
}

