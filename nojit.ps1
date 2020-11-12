# run this code as admin - it will create config files for each assembly so you dont have to debug jit in dnspy
# to get the modules.txt content perform the following:
# 1. open dnspy and attach to process
# 2. Debug->Windows->Modules
# 3. Highlight all modules and right click -> copy
# 4. Paste your clipboard buffer into modules.txt
$content0 = Get-Content "modules.txt"
$content1 = $content0.split(':')
$myArray = New-Object System.Collections.ArrayList
for ($i=0; $i -lt $content1.length; $i++) {
    if ($content1[$i].StartsWith("\")) {
	    $line = -join("c:", "", $content1[$i]) 
		$myArray.Add($line) | out-null
	}
}

$nojit = "[.NET Framework Debugging Control]
GenerateTrackingInfo=1
AllowOptimize=0"

foreach($line in $myArray){
    $config = $line -replace ".dll", ".ini"
	Set-Content -Path $config -Value $nojit   
}