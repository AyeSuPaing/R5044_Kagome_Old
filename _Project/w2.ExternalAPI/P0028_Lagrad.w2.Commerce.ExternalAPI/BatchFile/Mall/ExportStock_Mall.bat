rem ***********************************************************************
rem �o�b�`���F���[�����݌ɘA�g�p�t�@�C���o��
rem �T�v�F�X�ܖ��ɘA�g�t�@�C�����o�͂��A�X�܂փt�@�C���𑗐M����
rem       �捞�ɂ��񂵂Ă͊e�X�܂ɂčs��
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
set API_ID=P0028_Lagrad_ExportStock_Mall
rem �t�@�C���o�͐�f�B���N�g��
set OUTPUT_DIR=%WORK_DIR%\EXPORT_MALL
rem �ݒ�t�@�C���p�X
set SETTING_FILE=C:\inetpub\wwwroot\R5044_Kagome.Develop\_Project\w2.ExternalAPI\P0028_Lagrad.w2.Commerce.ExternalAPI\BatchFile\Mall\Settings.txt
rem �݌Ɏ擾���̉ߋ����ԁi�����j1�w��ŉߋ�����ŕϓ�������
set EXP_TERM=1
rem =======================================================================

rem ���s�f�B���N�g���ړ�
cd %WORK_DIR%

rem �t�@�C����
set date_tmp=%date:/=%
set time_tmp=%time: =0%
set yyyy=%date_tmp:~0,4%
set yy=%date_tmp:~2,2%
set mm=%date_tmp:~4,2%
set dd=%date_tmp:~6,2%
set hh=%time_tmp:~0,2%
set mi=%time_tmp:~3,2%
set ss=%time_tmp:~6,2%
set sss=%time_tmp:~9,2%
set datetime=%yyyy%%mm%%dd%_%hh%%mi%%ss%%sss%

rem Setting�̃T�v���C�A���݌Ƀt�@�C�����o�́A�X�ܑ��փR�s�[
for /f "delims=, usebackq tokens=1-2 usebackq" %%i in ("%SETTING_FILE%") do ( 
rem %%i�T�v���C�A
%EXE_PATH% -setArgs -apiID:%API_ID% -target:%OUTPUT_DIR%\%%i\expstock_%%i_%datetime%.csv -apiType:export -fileType:csv -props:supplier=%%i;term=%EXP_TERM%
move %OUTPUT_DIR%\%%i\expstock_%%i_%datetime%.csv %%j\expstock_%%i_%datetime%.csv
)

