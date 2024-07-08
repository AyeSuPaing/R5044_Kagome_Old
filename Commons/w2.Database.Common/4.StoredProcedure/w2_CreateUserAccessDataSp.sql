if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CreateUserAccessDataSp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[w2_CreateUserAccessDataSp]
GO
/*
=========================================================================================================
  Module      : �������[�U�A�N�Z�X�f�[�^�쐬�v���V�[�W��(w2_CreateUserAccessDataSp.sql)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE PROCEDURE [dbo].[w2_CreateUserAccessDataSp] (
					@TARGET_DATE varchar(10)) AS

	---------------------------------------
	-- �ϐ���`
	---------------------------------------
	DECLARE @TOTAL_PAGEVIEWS bigint
	DECLARE @TOTAL_USERS bigint
	DECLARE @TOTAL_SESSIONS bigint
	DECLARE @TOTAL_NEW_USERS bigint
	DECLARE @TOTAL_PAGEVIEWS_MOBILE bigint
	DECLARE @TOTAL_SESSIONS_MOBILE bigint
	DECLARE @DEPT_ID varchar(30)

	---------------------------------------
	-- dept_id�J�[�\����`
	---------------------------------------
	DECLARE CUR_DEPTID CURSOR FOR
	SELECT	DISTINCT dept_id
	  FROM	w2_AccessLogAccount

	---------------------------------------
	-- ���t����
	---------------------------------------
	DECLARE @TGT_YEAR varchar(4)
	DECLARE @TGT_MONTH varchar(2)
	DECLARE @TGT_DAY varchar(2)
	SET @TGT_YEAR = SUBSTRING(@TARGET_DATE, 0, 5)
	SET @TGT_MONTH = SUBSTRING(@TARGET_DATE, 6, 2)
	SET @TGT_DAY = SUBSTRING(@TARGET_DATE, 9, 2)

	---------------------------------------
	-- �J�[�\���J���E�ŏI�s�܂Ń��[�v
	---------------------------------------
	OPEN CUR_DEPTID

	WHILE (1=1)
	BEGIN
		---------------------------------------
		-- dept_id�擾
		---------------------------------------
		FETCH NEXT FROM	CUR_DEPTID
		  INTO	@DEPT_ID

		-- �I�[�Ȃ甲����
		IF @@FETCH_STATUS != 0
		BEGIN
			BREAK
		END
		
		---------------------------------------
		--���R�[�h�폜�i�f���[�g�C���T�[�g�̂��߁j
		---------------------------------------
		DELETE	w2_DispAccessAnalysis
		 WHERE	dept_id = @DEPT_ID
		   AND	tgt_year = @TGT_YEAR
		   AND	tgt_month = @TGT_MONTH
		   AND	tgt_day = @TGT_DAY

		---------------------------------------
		-- �o�b�F�y�[�W�r���[���擾
		---------------------------------------
		SELECT	@TOTAL_PAGEVIEWS = COUNT(*)
		  FROM	w2_AccessLog
		 WHERE	access_date = @TARGET_DATE
		   AND	dept_id = @DEPT_ID
		   AND	user_agent_kbn = 'PC'

		---------------------------------------
		-- �o�b�F���[�U�[���擾
		---------------------------------------
		SELECT	@TOTAL_USERS = COUNT(DISTINCT access_user_id)
		  FROM	w2_AccessLog
		 WHERE	access_date = @TARGET_DATE
		   AND	dept_id = @DEPT_ID
		   AND	user_agent_kbn = 'PC'
		
		---------------------------------------
		-- �o�b�F�K��Ґ��i�Z�b�V�������j�擾
		---------------------------------------
		SELECT	@TOTAL_SESSIONS = COUNT(DISTINCT session_id)
		  FROM	w2_AccessLog
		 WHERE	access_date = @TARGET_DATE
 		   AND	dept_id = @DEPT_ID
		   AND	user_agent_kbn = 'PC'

		---------------------------------------
		-- �o�b�F�V�K�K�⃆�[�U�[���擾
		---------------------------------------
		SELECT	@TOTAL_NEW_USERS = COUNT(DISTINCT access_user_id)
		  FROM	w2_AccessLog
		 WHERE	access_date = @TARGET_DATE
 		   AND	dept_id = @DEPT_ID
 		   AND	access_interval = -1	-- ����A�N�Z�X
		   AND	user_agent_kbn = 'PC'

		---------------------------------------
		-- �o�b�F���R�[�h�}��
		---------------------------------------
		-- �C���T�[�g
		INSERT w2_DispAccessAnalysis
			(
			dept_id,
			tgt_year,
			tgt_month,
			tgt_day,
			access_kbn,
			total_page_views,
			total_users,
			total_sessions,
			total_new_users
			)
		VALUES
			(
			@DEPT_ID,
			@TGT_YEAR,
			@TGT_MONTH,
			@TGT_DAY,
			'0',	-- PC
			@TOTAL_PAGEVIEWS,
			@TOTAL_USERS,
			@TOTAL_SESSIONS,
			@TOTAL_NEW_USERS
			)
			
		---------------------------------------
		-- ���o�C���F�y�[�W�r���[���擾
		---------------------------------------
		SELECT	@TOTAL_PAGEVIEWS_MOBILE = COUNT(*)
		  FROM	w2_AccessLogMobile
		 WHERE	access_date = @TARGET_DATE
		   AND	dept_id = @DEPT_ID

		---------------------------------------
		-- ���o�C���F�K��Ґ��i�Z�b�V�������j�擾
		---------------------------------------
		SELECT	@TOTAL_SESSIONS_MOBILE = COUNT(DISTINCT session_id)
		  FROM	w2_AccessLogMobile
		 WHERE	access_date = @TARGET_DATE
 		   AND	dept_id = @DEPT_ID
			
		---------------------------------------
		-- ���o�C���F���R�[�h�}��
		---------------------------------------
		-- �C���T�[�g
		INSERT w2_DispAccessAnalysis
			(
			dept_id,
			tgt_year,
			tgt_month,
			tgt_day,
			access_kbn,
			total_page_views,
			total_sessions
			)
		VALUES
			(
			@DEPT_ID,
			@TGT_YEAR,
			@TGT_MONTH,
			@TGT_DAY,
			'1',	-- ���o�C��
			@TOTAL_PAGEVIEWS_MOBILE,
			@TOTAL_SESSIONS_MOBILE
			)

		---------------------------------------
		-- �X�}�[�g�t�H���F�y�[�W�r���[���擾
		---------------------------------------
		SELECT	@TOTAL_PAGEVIEWS = COUNT(*)
		  FROM	w2_AccessLog
		 WHERE	access_date = @TARGET_DATE
		   AND	dept_id = @DEPT_ID
		   AND	user_agent_kbn = 'SP'

		---------------------------------------
		-- �X�}�[�g�t�H���F���[�U�[���擾
		---------------------------------------
		SELECT	@TOTAL_USERS = COUNT(DISTINCT access_user_id)
		  FROM	w2_AccessLog
		 WHERE	access_date = @TARGET_DATE
		   AND	dept_id = @DEPT_ID
		   AND	user_agent_kbn = 'SP'
		
		---------------------------------------
		-- �X�}�[�g�t�H���F�K��Ґ��i�Z�b�V�������j�擾
		---------------------------------------
		SELECT	@TOTAL_SESSIONS = COUNT(DISTINCT session_id)
		  FROM	w2_AccessLog
		 WHERE	access_date = @TARGET_DATE
 		   AND	dept_id = @DEPT_ID
		   AND	user_agent_kbn = 'SP'

		---------------------------------------
		-- �X�}�[�g�t�H���F�V�K�K�⃆�[�U�[���擾
		---------------------------------------
		SELECT	@TOTAL_NEW_USERS = COUNT(DISTINCT access_user_id)
		  FROM	w2_AccessLog
		 WHERE	access_date = @TARGET_DATE
 		   AND	dept_id = @DEPT_ID
 		   AND	access_interval = -1	-- ����A�N�Z�X
		   AND	user_agent_kbn = 'SP'

		---------------------------------------
		-- �X�}�[�g�t�H���F���R�[�h�}��
		---------------------------------------
		-- �C���T�[�g
		INSERT w2_DispAccessAnalysis
			(
			dept_id,
			tgt_year,
			tgt_month,
			tgt_day,
			access_kbn,
			total_page_views,
			total_users,
			total_sessions,
			total_new_users
			)
		VALUES
			(
			@DEPT_ID,
			@TGT_YEAR,
			@TGT_MONTH,
			@TGT_DAY,
			'2',	-- �X�}�[�g�t�H��
			@TOTAL_PAGEVIEWS,
			@TOTAL_USERS,
			@TOTAL_SESSIONS,
			@TOTAL_NEW_USERS
			)
	END

	---------------------------------------
	-- �J�[�\������
	---------------------------------------
	CLOSE CUR_DEPTID
	DEALLOCATE CUR_DEPTID
GO