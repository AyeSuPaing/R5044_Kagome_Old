rem ***********************************************************************
rem �o�b�`���F�X�ܑ��݌ɘA�g�p�t�@�C���o��
rem �T�v�F�A�g�t�@�C�����o�͂��A���[�����փt�@�C���𑗐M����
rem       �捞�ɂ��񂵂Ă̓��[���ɂčs��
rem ���ӓ_�F�X�ܑ���CustomerResource�ɁA�O���A�g�p�̃A�Z���u��
rem        �iP0028_Lagrad.w2.Commerce.ExternalAPI�j���z�u����Ă��邱�ƁB
rem ***********************************************************************

rem =======================================================================
rem ���ϐ�
rem �e���ɉ����ēK�؂ɕύX
rem =======================================================================
rem ���s�f�B���N�g��
set WORK_DIR=C:\inetpub\wwwroot\R5044_Kagome.Develop\Batch\P0008_Egoist\ExternalAPI
rem �O���A�gexe
set EXE_PATH=C:\inetpub\wwwroot\R5044_Kagome.Develop\Batch\P0008_Egoist\ExternalAPI\ExternalAPI.exe
rem APIID
set API_ID=P0028_Lagrad_ExportStock_Supplier
rem �T�v���C�AID
set SUPPLIER_ID=EGO
rem �t�@�C���o�͐�f�B���N�g��
set OUTPUT_DIR=%WORK_DIR%\EXPORT\%SUPPLIER_ID%
rem �t�@�C����
set OUTPUT_FILE=expstock.csv
rem ���[�����ɔz�u����ꏊ
set MALL_DIR=C:\inetpub\wwwroot\R5044_Kagome.Develop\Batch\P0028_Lagrad\ExternalAPI\IMPORT\EGO
rem �݌Ɏ擾���̉ߋ����ԁi�����j1�w��ŉߋ�����ŕϓ�������
set EXP_TERM=1
rem =======================================================================

rem ���s�f�B���N�g���ړ�
cd %WORK_DIR%

rem ���łɏo�̓t�@�C��������ꍇ�͏����Ă����i�㏑�������j
del %OUTPUT_DIR%\%OUTPUT_FILE%

rem -target�Ŏw�肷��t�@�C�����͌Œ��OK
%EXE_PATH% -setArgs -apiID:%API_ID% -target:%OUTPUT_DIR%\%OUTPUT_FILE% -apiType:export -fileType:csv -props:supplier=%SUPPLIER_ID%;term=%EXP_TERM%

rem �t�@�C��Move
move /y %OUTPUT_DIR%\%OUTPUT_FILE% %MALL_DIR%\%OUTPUT_FILE%

