@echo off
REM ================================
REM �萔�錾�@��Test/Honban�v�ύX
REM ================================
set CURRENT_DIR=C:\inetpub\wwwroot\R5044_Kagome.Develop\Batch\ExternalAPI\bin\
set WORK_DIR=C:\test\order_stck

REM ================================
REM �t�@�C���擾��A�捞
REM ================================
REM FTP�ڑ����
REM �z�X�g��
set FTP_HOST=localhost
REM ���[�U��
set FTP_USER=ftpuser
REM �p�X���[�h
set FTP_PASSWORD=ftpuserpw
REM �_�E�����[�h��(w2�T�[�o)�̃f�B���N�g��
set FTP_DOWNLOAD_LOCATION=C://test/
REM �_�E�����[�h��(FTP�T�[�o)�̃f�B���N�g��
set FTP_DOWNLOAD_SORCE=/ecbd121/SEND/
REM �_�E�����[�h���ɔz�u���Ă���擾�\��t�@�C���ɋK����������ꍇ�͂�����ɐ��K�\����ݒ� SET����ۃn�b�g(^)�ȂǓ���L�����܂މ\��������ꍇ��""�őS�̂��͂�ł��������B
set "FTP_DOWNLOAD_FILE_REGEX=^^STOCK.*.CSV$"
REM �A�N�e�B�u���[�h�̏ꍇ��true, �p�b�V�u���[�h�̏ꍇ��false
set FTP_USE_ACTIVE=false
REM FTPS�̏ꍇ��true, �����łȂ��ꍇ��false
set FTP_ENABLE_SSL=false

set "FTP_ARGS=ftphost=%FTP_HOST%;ftpuser=%FTP_USER%;ftppassword=%FTP_PASSWORD%;downloadlocation=%FTP_DOWNLOAD_LOCATION%;downloadsorce=%FTP_DOWNLOAD_SORCE%;useactive=%FTP_USE_ACTIVE%;enablessl=%FTP_ENABLE_SSL%;fileregex=%FTP_DOWNLOAD_FILE_REGEX%"

%CURRENT_DIR%ExternalAPI.exe -setArgs -apiID:P0008_Egoist_ImportStock -target:%WORK_DIR%\NO_FILE -apiType:import -fileType:csv2 -useftpdownload:true -props:%FTP_ARGS%

