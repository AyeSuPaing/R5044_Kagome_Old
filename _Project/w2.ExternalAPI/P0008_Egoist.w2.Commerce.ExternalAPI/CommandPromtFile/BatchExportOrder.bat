@echo off
REM �萔�錾�@��Test/Honban�v�ύX
set CURRENT_DIR=C:\inetpub\wwwroot\R5044_Kagome.Develop\Batch\ExternalAPI\bin\
set WORK_DIR=C:\test\order_stck\
set LOG_DIR=%WORK_DIR%Log\
set CMP_DIR=%WORK_DIR%Complete\
if not exist %CMP_DIR% mkdir %CMP_DIR%
if not exist %LOG_DIR% mkdir %LOG_DIR%

REM API�R�}���h���� ���ݎ���
set ARG_PROPS=Timespan=90;OrderStatus=ODR,ODR_CNSL;ReturnExchangeKbn=00;
set DT=%DATE:/=%
set TM=%TIME::=%
set TM=%TM: =0%
set TM=%TM:~0,6%
set tmpNow=%DT%%TM%
set DATE_TIME=%tmpNow: =%

REM �O���A�g�o�b�`���s
set TARGET_FILE=%WORK_DIR%ORDERFIX%DATE_TIME%
%CURRENT_DIR%ExternalAPI.exe -setArgs -apiID:P0008_Egoist_ExportOrderItems -target:%TARGET_FILE%.txt -apiType:Export -fileType:csv2 -props:%ARG_PROPS%

move %TARGET_FILE%.txt %CMP_DIR%