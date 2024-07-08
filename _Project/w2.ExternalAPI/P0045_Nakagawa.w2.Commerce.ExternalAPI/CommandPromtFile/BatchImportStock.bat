@echo off
@echo on

rem ���ɂ���ăp�X�ς���
set web_root_path=C:\test
set batch_root_path=C:\inetpub\wwwroot\R5044_Kagome.Develop\Batch

cd /D %web_root_path%\_w2cManager\nakagawaftp\stock

if not exist "stock.csv" goto :exit

powershell %batch_root_path%\ExternalAPI\_rep.ps1 stock.csv

cd %batch_root_path%\ExternalAPI

ExternalAPI.exe -setArgs -apiID:P0045_Nakagawa_ImportStock -target:%web_root_path%\_w2cManager\nakagawaftp\stock\stock.csv -apiType:import -fileType:csv -writeLog:false

:exit
