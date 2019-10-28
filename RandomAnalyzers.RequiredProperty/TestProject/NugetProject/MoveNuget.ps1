[string]$dir = Get-Location
$binDirectory = Join-Path $dir "bin/debug"
$NugetDirectory = Join-Path $dir "../nuget"

function Create-Nuget  ([String]$Folder, [String]$OutputDirectory) {
	New-Item -ItemType Directory -Force -Path $Folder
    Set-Location $Folder
    $fileName = Get-ChildItem -Filter *nupkg | Select-Object -First 1 | Select-Object -Expand Name
    nuget add $fileName -Source $NugetDirectory
    Remove-Item $fileName
    Set-Location $dir
}

Create-Nuget $binDirectory $NugetDirectory