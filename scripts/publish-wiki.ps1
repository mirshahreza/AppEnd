param(
    [string]$WikiRepoUrl = "https://github.com/mirshahreza/AppEnd.wiki.git",
    [string]$SourceDir = "wiki",
    [string]$TargetDir = ".wiki-publish"
)

$ErrorActionPreference = 'Stop'

function Exec {
    param(
        [Parameter(Mandatory)] [string] $cmd,
        [Parameter(Mandatory)] [string[]] $args
    )
    & $cmd @args
    if ($LASTEXITCODE -ne 0) {
        $joined = ($args -join ' ')
        throw "Command failed: $cmd $joined"
    }
}

if (-not (Test-Path $SourceDir)) {
    throw "Source directory '$SourceDir' not found. Ensure wiki/ exists at repo root."
}

# Ensure git exists
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    throw "git is not installed or not in PATH."
}

# Clean target dir
if (Test-Path $TargetDir) {
    Remove-Item -Recurse -Force $TargetDir
}

# Clone wiki repo
Exec 'git' @('clone', $WikiRepoUrl, $TargetDir)

# Mirror files from wiki/ into the cloned wiki repo
$robocopy = Get-Command robocopy -ErrorAction SilentlyContinue
if ($robocopy) {
    # /MIR mirrors, /XD excludes .git
    robocopy $SourceDir $TargetDir *.* /MIR /XD .git | Out-Null
} else {
    # Fallback: delete everything except .git then copy
    Get-ChildItem -Force $TargetDir | Where-Object { $_.Name -ne '.git' } | Remove-Item -Recurse -Force
    Copy-Item -Path (Join-Path $SourceDir '*') -Destination $TargetDir -Recurse -Force
}

# Commit and push if there are changes
$status = & git -C $TargetDir status --porcelain
if ([string]::IsNullOrWhiteSpace($status)) {
    Write-Host "No changes to publish."
    exit 0
}

Exec 'git' @('-C', $TargetDir, 'add', '-A')
$timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
Exec 'git' @('-C', $TargetDir, 'commit', '-m', "Publish wiki from repo wiki/ $timestamp")
Exec 'git' @('-C', $TargetDir, 'push')

Write-Host "Wiki published successfully to $WikiRepoUrl"
