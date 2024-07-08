@echo off
REM Constant declaration

set CURRENT_DIR=C:\inetpub\wwwroot\R5044_Kagome.Develop\Batch\ExternalAPI\bin\
set WORK_DIR=C:\_w2cManager\Logs\ExternalAPI\Order\

if not exist %WORK_DIR% mkdir %WORK_DIR%

set year=%date:~0,4%
set month=%date:~5,2%
set date=%date:~8,2%
set hour=%time:~0,2%
if "%hour:~0,1%" == " " set hour=0%hour:~1,1%
set minute=%time:~3,2%
set seconds=%time:~6,2%

set DATE_TIME=%year%%month%%date%%hour%%minute%%seconds%

set TARGET_FILE=%WORK_DIR%order%DATE_TIME%_IDutf8

%CURRENT_DIR%ExternalAPI.exe -setArgs -apiID:TempoStar_ExportOrder -target:%TARGET_FILE%.csv -apiType:Export -fileType:csv -props: