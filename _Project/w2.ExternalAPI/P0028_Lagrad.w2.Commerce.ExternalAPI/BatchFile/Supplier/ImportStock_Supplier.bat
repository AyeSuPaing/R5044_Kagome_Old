rem ***********************************************************************
rem �o�b�`���F�X�ܑ��݌ɘA�g�p�t�@�C���捞
rem �T�v�F���[�����ɂďo�͂��ꂽ�݌ɘA�g�t�@�C�����捞��
rem ���ӓ_�F�X�ܑ���CustomerResource�ɁA�O���A�g�p�̃A�Z���u��
rem        �iP0028_Lagrad.w2.Commerce.ExternalAPI�j���z�u����Ă��邱�ƁB
rem ***********************************************************************

rem =======================================================================
rem ���ϐ�
rem �e���ɉ����ēK�؂ɕύX
rem =======================================================================
rem ���s�f�B���N�g��
set WORK_DIR=C:\inetpub\wwwroot\V5.6\Batch\ExternalAPI\bin
rem �O���A�gexe
set EXE_PATH=C:\inetpub\wwwroot\V5.6\Batch\ExternalAPI\bin\ExternalAPI.exe
rem APIID
set API_ID=P0028_Lagrad_ImportStock_Supplier
rem �T�v���C�AID
set SUPPLIER_ID=P0028
rem �捞�Ώۃt�@�C���z�u��f�B���N�g��
set IMPUT_DIR=C:\inetpub\wwwroot\R5044_Kagome.Develop\Batch\ExternalAPI\bin\IMPORT_SUPPLIER\P0028
rem =======================================================================

cd %WORK_DIR%
rem �w��f�B���N�g�����̃t�@�C�����R�}���h���s
for /F %%A in ('dir /b %IMPUT_DIR%\*.csv') do %EXE_PATH% -setArgs -apiID:%API_ID% -target:%IMPUT_DIR%\%%A -apiType:import -fileType:csv

