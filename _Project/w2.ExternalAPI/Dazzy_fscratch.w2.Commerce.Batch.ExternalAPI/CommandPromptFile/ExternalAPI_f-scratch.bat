set YYYYMMDD=%DATE:/=%

ExternalAPI.exe Dazzy_fscratch_0005_ExportStocks D:\Batch\P0043_DRW\ExternalAPI\f-scratch\DRW_Stock_%YYYYMMDD%.csv D:\Batch\P0043_DRW\ExternalAPI\f-scratch\Active\active_0005.csv 1 csv
ExternalAPI.exe Dazzy_fscratch_0012_ExportOrders D:\Batch\P0043_DRW\ExternalAPI\f-scratch\DRW_Order_%YYYYMMDD%.csv D:\Batch\P0043_DRW\ExternalAPI\f-scratch\Active\active_0012.csv 1 csv
ExternalAPI.exe Dazzy_fscratch_0020_ExportOrderShippings D:\Batch\P0043_DRW\ExternalAPI\f-scratch\DRW_OrderShipping_%YYYYMMDD%.csv D:\Batch\P0043_DRW\ExternalAPI\f-scratch\Active\active_0020.csv 1 csv
ExternalAPI.exe Dazzy_fscratch_0028_ExportOrderItems D:\Batch\P0043_DRW\ExternalAPI\f-scratch\DRW_OrderItem_%YYYYMMDD%.csv D:\Batch\P0043_DRW\ExternalAPI\f-scratch\Active\active_0028.csv 1 csv