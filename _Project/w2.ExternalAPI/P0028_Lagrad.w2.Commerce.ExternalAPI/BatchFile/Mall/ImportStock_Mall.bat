rem ***********************************************************************
rem �o�b�`���F���[�����݌Ɏ捞
rem �T�v�F�X�ܖ��ɏo�͂��ꂽ�A�g�t�@�C�������[�����Ŏ捞��
rem ���ӓ_�F���[������CustomerResource�ɁA�O���A�g�p�̃A�Z���u��
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
set API_ID=P0028_Lagrad_ImportStock_Mall
rem �ݒ�t�@�C���p�X
set SETTING_FILE=C:\inetpub\wwwroot\R5044_Kagome.Develop\_Project\w2.ExternalAPI\P0028_Lagrad.w2.Commerce.ExternalAPI\BatchFile\Mall\Settings.txt
rem =======================================================================

rem ���s�f�B���N�g���ړ�
cd %WORK_DIR%

rem Setting�̃T�v���C�A���݌Ƀt�@�C�����捞��
for /f "delims=, usebackq tokens=1-3 usebackq" %%i in ("%SETTING_FILE%") do ( 
rem %%i�T�v���C�A
%EXE_PATH% -setArgs -apiID:%API_ID% -target:%%k -apiType:import -fileType:csv -props:supplier=%%i
)

