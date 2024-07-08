rem ***********************************************************************
rem �o�b�`���F�o�׎󒍍X�V
rem �T�v�F�o�׊m�肵���󒍂������ŘA�g���A�z���`�[�ԍ����X�V���āA�o�׊������[�����M����
rem ***********************************************************************

rem =======================================================================
rem ���ϐ�
rem �e���ɉ����ēK�؂ɕύX
rem =======================================================================
rem ���s�f�B���N�g��
set WORK_DIR=C:\inetpub\wwwroot\R5044_Kagome.Develop\Batch\ExternalAPI\bin\
rem �O���A�gexe
set EXE_PATH=C:\inetpub\wwwroot\R5044_Kagome.Develop\Batch\ExternalAPI\bin\ExternalAPI.exe
rem APIID
set API_ID=R1182_Nanoegg_UpdateShippedOrder
rem �捞�Ώۃt�@�C���z�u��f�B���N�g��
set INPUT_DIR=C:\inetpub\wwwroot\R5044_Kagome.Develop\_Project\w2.ExternalAPI\P1182_Nanoegg.w2.Commerce.Batch.ExternalAPI\CommandPromtFile
rem =======================================================================

cd %INPUT_DIR%

if not exist "import_*.csv" goto :exit

cd %WORK_DIR%
rem �w��f�B���N�g�����̃t�@�C�����R�}���h���s
for /F %%A in ('dir /b %INPUT_DIR%\import_*.csv') do %EXE_PATH% -setArgs -apiID:%API_ID% -target:%INPUT_DIR%\%%A -apiType:import -fileType:csv2
