rem ***********************************************************************
rem バッチ名：基幹在庫取込
rem 概要：連携された基幹在庫からw2側の注文個数を減産して在庫情報を更新する
rem ***********************************************************************

rem =======================================================================
rem 環境変数
rem 各環境に応じて適切に変更
rem =======================================================================
rem 実行ディレクトリ
set WORK_DIR=D:\Batch\P0059_Tenshodo\ExternalAPI\
rem 外部連携exe
set EXE_PATH=D:\Batch\P0059_Tenshodo\ExternalAPI\ExternalAPI.exe
rem APIID
set API_ID=P0059_Tenshodo_ImportStock
rem 取込対象ファイル配置先ディレクトリ
set INPUT_DIR=D:\FTProot\P0059_Tenshodo\LocalUser\P0059_toshiba\stock
rem =======================================================================

cd %INPUT_DIR%

if not exist "stock.csv" goto :exit
powershell %WORK_DIR%\_rep.ps1 stock.csv

cd %WORK_DIR%
rem 指定ディレクトリ内のファイル分コマンド実行
for /F %%A in ('dir /b %INPUT_DIR%\stock.csv') do %EXE_PATH% -setArgs -apiID:%API_ID% -target:%INPUT_DIR%\%%A -apiType:import -fileType:csv
