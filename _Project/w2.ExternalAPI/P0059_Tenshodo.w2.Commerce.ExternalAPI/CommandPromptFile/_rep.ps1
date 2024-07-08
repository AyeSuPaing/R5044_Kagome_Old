param($path=1)
$dt=Get-Date
Start-Sleep -s 10
$bk=$path+"_bf"+ $dt.ToString("yyyyMMddhhmmss")
Start-Sleep -s 10
$temp=$path+".tmp"
Start-Sleep -s 10
$temp2=$path+".tmp2"
Start-Sleep -s 10
$temp3=$path+".tmp3"
Start-Sleep -s 10
$(Get-Content $path) -replace "`"","" > $temp
$(Get-Content $temp) -replace ",","`",`"" > $temp2
$sr = New-Object System.IO.StreamReader($temp2)
while (($line = $sr.ReadLine()) -ne $null)
{
  $x = "`"" +$line+"`""
  echo $x >> $temp3
}
$sr.Close()
Start-Sleep -s 15
del $temp
del $temp2
move $path $bk
move $temp3 $path
del $bk