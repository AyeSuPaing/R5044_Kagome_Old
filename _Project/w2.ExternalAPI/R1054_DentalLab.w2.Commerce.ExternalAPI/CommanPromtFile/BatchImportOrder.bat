rem ***********************************************************************
rem バッチ名：出荷確定情報取込
rem 概要：出荷確定情報を自動で連携し、配送伝票番号を更新して、発送完了メール送信する
rem ***********************************************************************

rem =======================================================================
rem 環境変数
rem 各環境に応じて適切に変更
rem =======================================================================
rem 実行ディレクトリ
set WORK_DIR=C:\inetpub\wwwroot\V5\Batch\ExternalAPI\bin\
rem 外部連携exe
set EXE_PATH=C:\inetpub\wwwroot\V5\Batch\ExternalAPI\bin\ExternalAPI.exe
rem APIID
set API_ID=R1054_DentalLab_ImportOrder
rem 取込対象ファイル配置先ディレクトリ
set INPUT_DIR=C:\inetpub\wwwroot\V5\_Project\w2.ExternalAPI\R1054_DentalLab.w2.Commerce.ExternalAPI\CommanPromtFile
rem =======================================================================

cd %INPUT_DIR%

if not exist "Shipping_*.csv" goto :exit

cd %WORK_DIR%
rem 指定ディレクトリ内のファイル分コマンド実行
for /F %%A in ('dir /b %INPUT_DIR%\Shipping_*.csv') do %EXE_PATH% -setArgs -apiID:%API_ID% -target:%INPUT_DIR%\%%A -apiType:import -fileType:csv2
