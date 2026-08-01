# Builds all Masged deployables, zips each output, and copies archives to ./publish
# Usage: .\publish-all.ps1
# Requires: dotnet SDK, Node.js/npm, Flutter (in PATH)

$ErrorActionPreference = 'Stop'

$Root = $PSScriptRoot
$PublishDir = Join-Path $Root 'publish'
$StagingDir = Join-Path $Root '_publish-staging'
$DotNetPublishProfile = 'FolderProfile'

# Windows PowerShell 5.1 Join-Path accepts only two path segments.
function Join-Many {
    param(
        [Parameter(Mandatory)]
        [string]$Root,
        [Parameter(Mandatory)]
        [string[]]$Parts
    )
    $path = $Root
    foreach ($part in $Parts) {
        $path = Join-Path $path $part
    }
    return $path
}

function Get-DotNetPublishOutput {
    param([Parameter(Mandatory)][string]$ProjectName)

    $base = Join-Many $Root @($ProjectName, 'bin', 'Release', 'net8.0')
    $candidates = @(
        (Join-Many $base @('win-x64', 'publish')),
        (Join-Many $base @('publish'))
    )
    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) {
            return $candidate
        }
    }
    throw "Could not find dotnet publish output under $ProjectName\bin\Release\net8.0\"
}

function Write-Step([string]$Message) {
    Write-Host ''
    Write-Host "==> $Message" -ForegroundColor Cyan
}

function Ensure-Command([string]$Name) {
    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        throw "Required command not found in PATH: $Name"
    }
}

function Zip-FolderContents {
    param(
        [Parameter(Mandatory)]
        [string]$SourceFolder,
        [Parameter(Mandatory)]
        [string]$ZipPath
    )

    if (-not (Test-Path $SourceFolder)) {
        throw "Build output folder not found: $SourceFolder"
    }

    $items = Get-ChildItem -Path $SourceFolder -Force
    if ($items.Count -eq 0) {
        throw "Build output folder is empty: $SourceFolder"
    }

    if (Test-Path $ZipPath) {
        Remove-Item $ZipPath -Force
    }

    $parent = Split-Path $ZipPath -Parent
    if (-not (Test-Path $parent)) {
        New-Item -ItemType Directory -Path $parent -Force | Out-Null
    }

    Compress-Archive -Path (Join-Path $SourceFolder '*') -DestinationPath $ZipPath -CompressionLevel Optimal
    Write-Host "Created: $ZipPath"
}

function Sync-QcfFonts {
    param(
        [Parameter(Mandatory)]
        [string]$DestinationRoot
    )

    $source = Join-Many $Root @('ParentApp', 'assets', 'fonts', 'v2woff')
    $dest = Join-Path $DestinationRoot 'static\qcf-fonts'

    if (-not (Test-Path $source)) {
        throw "QCF font source not found: $source"
    }

    Write-Host "Syncing QCF fonts -> $dest"
    New-Item -ItemType Directory -Path $dest -Force | Out-Null

    # /MIR keeps publish folder aligned; exit codes 0-7 are success for robocopy.
    & robocopy $source $dest '*.woff' /MIR /NFL /NDL /NJH /NJS /NC /NS | Out-Null
    if ($LASTEXITCODE -ge 8) {
        throw "robocopy failed syncing QCF fonts (exit $LASTEXITCODE)"
    }

    $count = (Get-ChildItem $dest -Filter '*.woff' -File).Count
    Write-Host "QCF fonts synced: $count files"
}

# Server-owned files must never travel inside the package, so that extracting an
# update over a live site cannot clobber its config, credentials or runtime data.
# A fresh install copies appsettings.example.json to appsettings.json and edits it.
function Protect-PublishOutput {
    param(
        [Parameter(Mandatory)]
        [string]$PublishFolder
    )

    $appsettings = Join-Path $PublishFolder 'appsettings.json'
    if (Test-Path $appsettings) {
        $example = Join-Path $PublishFolder 'appsettings.example.json'
        if (Test-Path $example) { Remove-Item $example -Force }
        Rename-Item -Path $appsettings -NewName 'appsettings.example.json'
        Write-Host 'Packaged config as template: appsettings.example.json'
    }

    $excluded = @(
        'appsettings.Development.json',
        'appsettings.Production.json',
        'firebase-service-account.json',
        '_buildcheck',
        'Uploads',
        'FilesManager',
        'Logs'
    )

    foreach ($name in $excluded) {
        $path = Join-Path $PublishFolder $name
        if (Test-Path $path) {
            Remove-Item $path -Recurse -Force
            Write-Host "Excluded from package: $name"
        }
    }
}

function Publish-DotNetApi {
    param(
        [string]$ProjectName,
        [string]$CsprojRelativePath
    )

    Write-Step "Publishing $ProjectName (profile: $DotNetPublishProfile)"
    $csproj = Join-Path $Root $CsprojRelativePath

    if ($ProjectName -eq 'AdminAPI') {
        $adminApiRoot = Join-Path $Root 'AdminAPI'
        Sync-QcfFonts -DestinationRoot $adminApiRoot
    }

    dotnet publish $csproj -c Release "/p:PublishProfile=$DotNetPublishProfile"
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed for $ProjectName" }

    $output = Get-DotNetPublishOutput -ProjectName $ProjectName
    Write-Host "Publish output: $output"

    if ($ProjectName -eq 'AdminAPI') {
        Sync-QcfFonts -DestinationRoot $output
    }

    Protect-PublishOutput -PublishFolder $output

    $zip = Join-Path $StagingDir "$ProjectName.zip"
    Zip-FolderContents -SourceFolder $output -ZipPath $zip
}

function Build-NpmUi {
    param(
        [string]$ProjectName,
        [string]$FolderName
    )

    Write-Step "Building $ProjectName (npm run build)"
    $projectDir = Join-Path $Root $FolderName
    Push-Location $projectDir
    try {
        npm run build
        if ($LASTEXITCODE -ne 0) { throw "npm run build failed for $ProjectName" }
    }
    finally {
        Pop-Location
    }

    $output = Join-Path $projectDir 'dist'
    $zip = Join-Path $StagingDir "$ProjectName.zip"
    Zip-FolderContents -SourceFolder $output -ZipPath $zip
}

function Build-FlutterWeb {
    param(
        [string]$ProjectName,
        [string]$FolderName
    )

    Write-Step "Building $ProjectName (web build without Quran fonts)"
    $projectDir = Join-Path $Root $FolderName
    $webBuildScript = Join-Path $projectDir 'tool\web_build.ps1'
    Push-Location $projectDir
    try {
        & $webBuildScript
        if ($LASTEXITCODE -ne 0) { throw "web build failed for $ProjectName" }
    }
    finally {
        Pop-Location
    }

    $output = Join-Many $projectDir @('build', 'web')
    $zip = Join-Path $StagingDir "$ProjectName.zip"
    Zip-FolderContents -SourceFolder $output -ZipPath $zip
}

function Build-FlutterAndroid {
    Write-Step 'Building ParentApp Android App Bundle (Google Play)'
    $playDeployScript = Join-Many $Root @('ParentApp', 'tool', 'play_deploy.ps1')
    $playOutput = Join-Path $PublishDir 'google-play'

    & $playDeployScript -OutputDir $playOutput
    if ($LASTEXITCODE -ne 0) { throw 'play_deploy.ps1 failed' }

    $zip = Join-Path $StagingDir 'ParentApp-Android.zip'
    if (Test-Path $zip) { Remove-Item $zip -Force }
    Compress-Archive -Path (Join-Path $playOutput '*') -DestinationPath $zip -CompressionLevel Optimal
    Write-Host "Created: $zip"
}

# --- prerequisites ---
Ensure-Command dotnet
Ensure-Command npm
Ensure-Command flutter

if (Test-Path $PublishDir) {
    Remove-Item $PublishDir -Recurse -Force
}
if (Test-Path $StagingDir) {
    Remove-Item $StagingDir -Recurse -Force
}
New-Item -ItemType Directory -Path $PublishDir -Force | Out-Null
New-Item -ItemType Directory -Path $StagingDir -Force | Out-Null

$started = Get-Date
Write-Host "Masged publish-all started at $($started.ToString('yyyy-MM-dd HH:mm:ss'))"
Write-Host "Root: $Root"

try {
    Publish-DotNetApi -ProjectName 'AdminAPI' -CsprojRelativePath 'AdminAPI\AdminAPI.csproj'
    Build-NpmUi -ProjectName 'AdminPanelUI' -FolderName 'AdminPanelUI'
    Publish-DotNetApi -ProjectName 'MasgedParentMobileAPI' -CsprojRelativePath 'MasgedParentMobileAPI\MasgedParentMobileAPI.csproj'
    Build-FlutterWeb -ProjectName 'ParentApp' -FolderName 'ParentApp'
    Build-FlutterAndroid
    Build-NpmUi -ProjectName 'PublicWebsiteUI' -FolderName 'PublicWebsiteUI'

    Write-Step 'Copying zip archives to publish folder'
    Copy-Item -Path (Join-Path $StagingDir '*.zip') -Destination $PublishDir -Force

    $elapsed = (Get-Date) - $started
    Write-Host ''
    Write-Host 'All builds completed successfully.' -ForegroundColor Green
    Write-Host "Output folder: $PublishDir"
    Write-Host "Elapsed: $($elapsed.ToString('hh\:mm\:ss'))"
    Get-ChildItem $PublishDir -Filter '*.zip' | ForEach-Object {
        $sizeMb = [math]::Round($_.Length / 1MB, 2)
        Write-Host "  $($_.Name) ($sizeMb MB)"
    }
    $playDir = Join-Path $PublishDir 'google-play'
    if (Test-Path $playDir) {
        Write-Host ''
        Write-Host 'Google Play artifacts:' -ForegroundColor Green
        Get-ChildItem $playDir | ForEach-Object {
            $sizeMb = if ($_.PSIsContainer) { '' } else { " ($([math]::Round($_.Length / 1MB, 2)) MB)" }
            Write-Host "  google-play\$($_.Name)$sizeMb"
        }
    }
}
finally {
    if (Test-Path $StagingDir) {
        Remove-Item $StagingDir -Recurse -Force
    }
}
