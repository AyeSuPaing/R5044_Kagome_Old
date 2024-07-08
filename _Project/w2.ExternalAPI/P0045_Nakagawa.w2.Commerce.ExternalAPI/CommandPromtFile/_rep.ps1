param($path=1)
$dt=Get-Date
$bk=$path+"_bf"+ $dt.ToString("yyyyMMddhhmmss")
$temp=$path+".tmp"
$temp2=$path+".tmp2"
$temp3=$path+".tmp3"
$(Get-Content $path) -replace "`"","" > $temp
$(Get-Content $temp) -replace ",","`",`"" > $temp2
$sr = New-Object System.IO.StreamReader($temp2)
while (($line = $sr.ReadLine()) -ne $null)
{
  $x = "`"" +$line+"`""
  echo $x >> $temp3
}
$sr.Close()
del $temp
del $temp2
del $bk
move $path $bk
move $temp3 $path
