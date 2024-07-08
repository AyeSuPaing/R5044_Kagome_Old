@echo off
for /f %%f IN ('dir /b c:\logs\RealShopProductStock*.csv') do ( 
@echo off
echo Importing Order File: %%f 
@echo on
ExternalAPI.exe -setArgs -apiID:P0022_Naracamicie_ImportRealStock -target:%%f -apiType:import -fileType:csv2 -writeLog:false
@echo off)