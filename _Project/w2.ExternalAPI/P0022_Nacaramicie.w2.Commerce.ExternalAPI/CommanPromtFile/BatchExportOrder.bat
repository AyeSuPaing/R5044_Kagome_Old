@echo off
REM ���ݎ��Ԃ��擾���āA YYMMDDHHMM�@�̌`�Ƀt�H�[�}�b�g
set DT=%DATE:/=%
set TM=%TIME::=%
set TM=%TM:~0,4%
set dtNow=%DT%%TM%
@echo on

ExternalAPI.exe -setArgs -apiID:P0022_Naracamicie_ExportOrders        -target:Order%dtNow%.csv         -apiType:Export -fileType:csv -props:PaymentStatus=0;OrderStatus=ODR;IntgFlag=4;IntgWorkingFlag=5;
ExternalAPI.exe -setArgs -apiID:P0022_Naracamicie_ExportOrderShipping -target:OrderShipping%dtNow%.csv -apiType:Export -fileType:csv -props:PaymentStatus=0;OrderStatus=ODR;IntgFlag=4;IntgWorkingFlag=5;
ExternalAPI.exe -setArgs -apiID:P0022_Naracamicie_ExportOrderItems    -target:OrderItem%dtNow%.csv     -apiType:Export -fileType:csv -props:PaymentStatus=0;OrderStatus=ODR;IntgFlag=4;IntgWorkingFlag=5;
