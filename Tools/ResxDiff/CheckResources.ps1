write-host "##teamcity[message text='Comparing resx files']"
$toolPath = "Tools\ResxDiff\ResxDiff.exe"
$targetDirectory = "Code\Omnipaste\Properties"
$mainResxFileName = "Resources.resx"
$mainResxFile = Get-ChildItem "$targetDirectory\$mainResxFileName"
$resxFiles = Get-ChildItem "$targetDirectory\*.resx"

$exitCode = 0
foreach ($resxFile in $resxFiles)
  {
  if($resxFile.Name -ne $mainResxFile.Name)
  {
    $output = cmd /c $toolPath -m $mainResxFile $resxFile 2`>`&1
    if($output)
    {
      write-host "##teamcity[message text='Found Missing Translation in $($resxFile.Name)' errorDetails='$output' status='ERROR']"
      $exitCode = 1
    }
  }
}

exit $exitCode