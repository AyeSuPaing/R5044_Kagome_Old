rem ***********************************************************************
rem �o�b�`���F�o�׊m����捞
rem �T�v�F�o�׊m����������ŘA�g���A�z���`�[�ԍ����X�V���āA�����������[�����M����
rem ***********************************************************************

rem =======================================================================
rem ���ϐ�
rem �e���ɉ����ēK�؂ɕύX
rem =======================================================================
rem ���s�f�B���N�g��
set WORK_DIR=C:\inetpub\wwwroot\V5\Batch\ExternalAPI\bin\
rem �O���A�gexe
set EXE_PATH=C:\inetpub\wwwroot\V5\Batch\ExternalAPI\bin\ExternalAPI.exe
rem APIID
set API_ID=R1054_DentalLab_ImportOrder
rem �捞�Ώۃt�@�C���z�u��f�B���N�g��
set INPUT_DIR=C:\inetpub\wwwroot\V5\_Project\w2.ExternalAPI\R1054_DentalLab.w2.Commerce.ExternalAPI\CommanPromtFile
rem =======================================================================

cd %INPUT_DIR%

if not exist "Shipping_*.csv" goto :exit

cd %WORK_DIR%
rem �w��f�B���N�g�����̃t�@�C�����R�}���h���s
for /F %%A in ('dir /b %INPUT_DIR%\Shipping_*.csv') do %EXE_PATH% -setArgs -apiID:%API_ID% -target:%INPUT_DIR%\%%A -apiType:import -fileType:csv2
