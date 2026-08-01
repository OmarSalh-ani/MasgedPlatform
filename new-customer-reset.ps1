# Scrubs a COPY of this project so it can safely become a new customer's project.
# Removes the previous customer's credentials, data and build outputs.
#
# Usage (run inside the copied folder):
#   .\new-customer-reset.ps1            # asks for confirmation
#   .\new-customer-reset.ps1 -DryRun    # show what would be removed, change nothing
#   .\new-customer-reset.ps1 -Force     # no confirmation prompt

param(
    [switch]$DryRun,
    [switch]$Force
)

$ErrorActionPreference = 'Stop'
$Root = $PSScriptRoot

if (-not $DryRun -and -not $Force) {
    Write-Host 'This will DELETE the previous customer''s credentials, uploads and build outputs' -ForegroundColor Yellow
    Write-Host "from: $Root" -ForegroundColor Yellow
    Write-Host 'Run it only inside a COPY made for a new customer, never in the original project.' -ForegroundColor Yellow
    $answer = Read-Host 'Type YES to continue'
    if ($answer -cne 'YES') {
        Write-Host 'Aborted.'
        exit 1
    }
}

function Remove-Target {
    param([string]$RelativePath, [string]$Reason)
    $path = Join-Path $Root $RelativePath
    if (-not (Test-Path $path)) { return }
    if ($DryRun) {
        Write-Host "[dry-run] would remove: $RelativePath  ($Reason)"
    }
    else {
        Remove-Item $path -Recurse -Force
        Write-Host "Removed: $RelativePath  ($Reason)"
    }
}

function Reset-EnvFile {
    param([string]$RelativePath, [string]$Contents)
    $path = Join-Path $Root $RelativePath
    if ($DryRun) {
        Write-Host "[dry-run] would reset: $RelativePath (template values)"
        return
    }
    Set-Content -Path $path -Value $Contents -Encoding UTF8
    Write-Host "Reset to template: $RelativePath"
}

function Clear-WasenderToken {
    param([string]$RelativePath)
    $path = Join-Path $Root $RelativePath
    if (-not (Test-Path $path)) { return }
    $content = Get-Content $path -Raw
    if ($content -notmatch '"ApiToken":\s*"[^"]+"') { return }
    if ($DryRun) {
        Write-Host "[dry-run] would blank Wasender ApiToken in: $RelativePath"
        return
    }
    $content = $content -replace '"ApiToken":\s*"[^"]*"', '"ApiToken": ""'
    $content = $content -replace '"SessionApiKey":\s*"[^"]*"', '"SessionApiKey": ""'
    Set-Content -Path $path -Value $content -Encoding UTF8 -NoNewline
    Write-Host "Blanked Wasender ApiToken/SessionApiKey in: $RelativePath"
}

Write-Host ''
Write-Host '=== Credentials of the previous customer ===' -ForegroundColor Cyan
Remove-Target 'AdminAPI\firebase-service-account.json'              'Firebase push credentials'
Remove-Target 'AdminAPI\appsettings.Production.json'                'previous customer production config'
Remove-Target 'MasgedParentMobileAPI\firebase-service-account.json' 'Firebase push credentials'
Remove-Target 'ParentApp\android\app\google-services.json'          'Firebase Android config'
Remove-Target 'ParentApp\ios\Runner\GoogleService-Info.plist'       'Firebase iOS config'
Remove-Target 'ParentApp\lib\firebase_options.dart'                 'Firebase keys — regenerate with flutterfire configure'
Remove-Target 'ParentApp\android\key.properties'                    'Android signing config'
Remove-Target 'ParentApp\android\upload-keystore.jks'               'Android signing keystore'
Remove-Target 'ParentApp\codemagic.yaml'                            'customer CI config — copy codemagic.yaml.example instead'
Clear-WasenderToken 'AdminAPI\appsettings.Development.json'
Clear-WasenderToken 'MasgedParentMobileAPI\appsettings.Development.json'

Write-Host ''
Write-Host '=== Data of the previous customer ===' -ForegroundColor Cyan
Remove-Target 'AdminAPI\Uploads'      'uploaded images/files'
Remove-Target 'AdminAPI\FilesManager' 'uploaded documents'
Remove-Target 'AdminAPI\Logs'         'runtime logs'

Write-Host ''
Write-Host '=== Build outputs (contain copies of the files above) ===' -ForegroundColor Cyan
Remove-Target 'publish'                       'packaged zips'
Remove-Target '_publish-staging'              'packaging staging'
Remove-Target 'AdminAPI\bin'                  'build output'
Remove-Target 'AdminAPI\obj'                  'build output'
Remove-Target 'AdminAPI\_buildcheck'          'build output'
Remove-Target 'MasgedParentMobileAPI\bin'     'build output'
Remove-Target 'MasgedParentMobileAPI\obj'     'build output'
Remove-Target 'Masged.WhatsApp\bin'           'build output'
Remove-Target 'Masged.WhatsApp\obj'           'build output'
Remove-Target 'AdminPanelUI\dist'             'UI bundle built for the old domain'
Remove-Target 'PublicWebsiteUI\dist'          'UI bundle built for the old domain'
Remove-Target 'ParentApp\build'               'Flutter build output'

Write-Host ''
Write-Host '=== UI .env files -> template values ===' -ForegroundColor Cyan
Reset-EnvFile 'AdminPanelUI\.env' @'
VITE_API_BASE_URL=https://admin.customer.com/api
VITE_UPLOADS_BASE_URL=https://admin.customer.com
VITE_PUBLIC_SITE_URL=https://customer.com
'@
Reset-EnvFile 'PublicWebsiteUI\.env' @'
VITE_API_BASE_URL=https://admin.customer.com/api
VITE_UPLOADS_BASE_URL=https://admin.customer.com
# VITE_APP_STORE_URL=https://apps.apple.com/...
# VITE_GOOGLE_PLAY_URL=https://play.google.com/store/apps/...
'@

Write-Host ''
Write-Host 'Done. Manual steps for the new customer:' -ForegroundColor Green
Write-Host '  1. AdminAPI\appsettings.json: connection string, Jwt:Key, StudentQr key, Cors,'
Write-Host '     PublicSite:BaseUrl, Registration numbers, Deployment:Domain, Firebase:ProjectId'
Write-Host '  2. MasgedParentMobileAPI\appsettings.json: connection string + its own unique keys'
Write-Host '  3. UI .env files: replace customer.com with the real domain BEFORE building'
Write-Host '  4. ParentApp: new applicationId/bundle id, then flutterfire configure'
Write-Host '     (regenerates firebase_options.dart) + new google-services.json / GoogleService-Info.plist'
Write-Host '  5. ParentApp icons: .\ParentApp\tool\generate_store_icons.ps1 -LogoPath <logo.png>'
Write-Host '  6. Codemagic: copy ParentApp\codemagic.yaml.example to codemagic.yaml and fill CUSTOMER_* values'
Write-Host '  7. New Android keystore + key.properties for the new customer'
Write-Host '  8. Fallback branding still shows the OLD customer until replaced:'
Write-Host '     - AdminPanelUI\public\assets\images\logo.png + pwa-icon-192/512.png + favicon.svg'
Write-Host '     - PublicWebsiteUI\public\assets\images\logo.png + favicon.svg'
Write-Host '     - DEFAULT_MASGED_NAME in AdminPanelUI + PublicWebsiteUI src\lib\masgedBrandingDefaults.ts'
Write-Host '     - PublicWebsiteUI\src\content\privacyPolicyContent.ts (names the old mosque as data controller)'
Write-Host 'Full guide: DEPLOYMENT.md'
