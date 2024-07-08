@echo off
REM 現在時間を取得して、 YYMMDDHHMM　の形にフォーマット
set DT=%DATE:/=%
set TM=%TIME::=%
set TM=%TM:~0,4%
set tmpNow=%DT%%TM%
set dtNow=%tmpNow: =%
@echo on

ExternalAPI.exe -setArgs -apiID:P0025_Feiler_ExportOrders        -target:Order%dtNow%.csv         -apiType:Export -fileType:csv -props:Timespan=60;PaymentStatus=0;OrderStatus=ODR;IntgFlag=7;IntgWorkingFlag=8;
ExternalAPI.exe -setArgs -apiID:P0025_Feiler_ExportOrderShipping -target:OrderShipping%dtNow%.csv -apiType:Export -fileType:csv -props:Timespan=60;PaymentStatus=0;OrderStatus=ODR;IntgFlag=7;IntgWorkingFlag=8;
ExternalAPI.exe -setArgs -apiID:P0025_Feiler_ExportOrderItems    -target:OrderItem%dtNow%.csv     -apiType:Export -fileType:csv -props:Timespan=60;PaymentStatus=0;OrderStatus=ODR;IntgFlag=7;IntgWorkingFlag=8;
