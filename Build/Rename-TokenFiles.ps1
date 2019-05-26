param (
    [Parameter(Mandatory = $true)] [string] $targetDirectory
)

(Get-ChildItem -Path $targetDirectory -Filter '*.token' -Recurse) | ForEach-Object {
    $tokenFile = $_
    $targetFile = $tokenFile.FullName.Replace('.token', '')

    Write-Host "Processing token file $($tokenFile.FullName)"

    if (Test-Path $targetFile) {
        Write-Host "Target file exists - replacing $targetFile with $($tokenFile.FullName)"

        Move-Item -Path $tokenFile.FullName -Destination $targetFile -Force -Verbose
    }
    else {
        Write-Host "Target file does not exist"
    }
}