Param(
  [string]$version
)

$path = '.\CallMeMaybe\CallMeMaybe.nuspec'
$xml = [xml](Get-Content $path)
$xml.package.metadata.version = $version
$xml.Save($path)