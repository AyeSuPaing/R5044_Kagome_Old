@echo off
REM �萔�錾
set CURRENT_DIR=C:\inetpub\wwwroot\R5044_Kagome.Develop\Batch\ExternalAPI\bin\
set WORK_KBN=order_stck
set WORK_DIR=C:\test\%WORK_KBN%\
set CMP_DIR=%WORK_DIR%Complete\

REM API�R�}���h����
set ARG_PROPS=OrderStatus=ODR;OutputCsvFlag=1;Timespan=60;

REM ���ݎ���
set DT=%DATE:/=%
set TM=%TIME::=%
set TM=%TM: =0%
set TM=%TM:~0,6%
set DATE_TIME=%DT%%TM%


set TARGET_FILE=%WORK_DIR%\%DATE_TIME%%WORK_KBN:order=%

%CURRENT_DIR%ExternalAPI.exe -setArgs -apiID:P0045_Nakagawa_ExportOrderItems -target:%TARGET_FILE%.work -apiType:Export -fileType:csv -props:%ARG_PROPS% -writeLog:true

move %TARGET_FILE%.work %CMP_DIR%