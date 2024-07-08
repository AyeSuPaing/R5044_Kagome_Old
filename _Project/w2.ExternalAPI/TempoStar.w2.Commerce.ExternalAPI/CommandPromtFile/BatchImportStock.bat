@echo off
REM Constant declaration

set CURRENT_DIR=C:\inetpub\wwwroot\R5044_Kagome.Develop\Batch\ExternalAPI\bin\
set WORK_DIR=C:\_w2cManager\Logs\ExternalAPI\Stock\

if not exist %WORK_DIR% mkdir %WORK_DIR%

REM FTP connection information
REM Host name
set FTP_HOST=ftp.tempostar.net

REM Username
set FTP_USER=4335_0

REM Password
set FTP_PASSWORD=zkgPw43350

REM Directory of download destination (w2 server)
set FTP_DOWNLOAD_LOCATION=%WORK_DIR%

REM Shop code
set SHOP_CODE=20356

REM Directory of download source (FTP server)
set FTP_DOWNLOAD_SOURCE=/netstock/%SHOP_CODE%/

REM FTP download file regex
set "FTP_DOWNLOAD_FILE_REGEX="

REM True for active mode, false for passive mode
set FTP_USE_ACTIVE=false

REM True for FTPS, false otherwise
set FTP_ENABLE_SSL=true

set "FTP_ARGS=ftphost=%FTP_HOST%;ftpuser=%FTP_USER%;ftppassword=%FTP_PASSWORD%;downloadlocation=%FTP_DOWNLOAD_LOCATION%;downloadsorce=%FTP_DOWNLOAD_SOURCE%;useactive=%FTP_USE_ACTIVE%;enablessl=%FTP_ENABLE_SSL%;fileregex=%FTP_DOWNLOAD_FILE_REGEX%"

%CURRENT_DIR%ExternalAPI.exe -setArgs -apiID:TempoStar_ImportStock -target:%WORK_DIR%\NO_FILE -apiType:import -fileType:csv -useftpdownload:true -props:%FTP_ARGS%