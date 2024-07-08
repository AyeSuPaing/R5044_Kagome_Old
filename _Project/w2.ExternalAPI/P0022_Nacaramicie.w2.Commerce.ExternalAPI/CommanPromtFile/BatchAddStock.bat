@echo off
for /f %%f IN ('dir /b STOCK*.csv') do ( 
@echo off
echo Importing Stock File: %%f 
@echo on
ExternalAPI.exe -setArgs -apiID:P0022_Naracamicie_ImportStock -target:%%f -apiType:import -fileType:csv
@echo off)