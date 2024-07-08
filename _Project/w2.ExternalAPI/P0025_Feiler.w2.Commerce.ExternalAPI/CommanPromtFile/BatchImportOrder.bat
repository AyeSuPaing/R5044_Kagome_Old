@echo off
for /f %%f IN ('dir /b c:\logs\ShipOrder*.csv') do ( 
@echo off
echo Importing Order File: %%f 
@echo on
ExternalAPI.exe -setArgs -apiID:P0025_Feiler_ImportOrder -target:%%f -apiType:import -fileType:csv
@echo off)