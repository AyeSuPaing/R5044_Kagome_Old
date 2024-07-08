@echo off
for /f %%f IN ('dir /b C:\Logs\Dtp\*.csv') do ( 
@echo off
echo Importing RealStock File: %%f 
@echo on
ExternalAPI.exe -setArgs -apiID:P0025_Feiler_ImportRealStock -target:C:\Logs\Dtp\%%f -apiType:import -fileType:csv
@echo off)