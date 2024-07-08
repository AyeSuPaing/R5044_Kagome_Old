if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CreateUserConditionDataSp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[w2_CreateUserConditionDataSp]
GO
/*
=========================================================================================================
  Module      : �������[�U�󋵃f�[�^�쐬�v���V�[�W��(w2_CreateUserConditionDataSp.sql)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE PROCEDURE [dbo].[w2_CreateUserConditionDataSp] (
					@TARGET_DATE varchar(10)) AS

	---------------------------------------
	-- �ϐ���`
	---------------------------------------
	DECLARE @POTENTIAL_NEW BIGINT
	DECLARE @POTENTIAL_ALL BIGINT
	DECLARE @POTENTIAL_ACTIVE BIGINT
	DECLARE @POTENTIAL_UNACTIVE1 BIGINT
	DECLARE @POTENTIAL_UNACTIVE2 BIGINT
	DECLARE @RECOGNIZE_NEW BIGINT
	DECLARE @RECOGNIZE_ALL BIGINT
	DECLARE @RECOGNIZE_ACTIVE BIGINT
	DECLARE @RECOGNIZE_UNACTIVE1 BIGINT
	DECLARE @RECOGNIZE_UNACTIVE2 BIGINT
	DECLARE @LEAVE_NEW BIGINT
	DECLARE @LEAVE_ALL BIGINT
	DECLARE @DATE_LAST_MONTH DATETIME
	DECLARE @DATE_LAST_MONTH2 DATETIME
	
	DECLARE @DEPT_ID varchar(30)

	---------------------------------------
	-- dept_id�J�[�\����`
	---------------------------------------
	DECLARE CUR_DEPTID CURSOR FOR
	SELECT	DISTINCT dept_id
	  FROM	w2_AccessLogAccount

	---------------------------------------
	-- �x�����[�U�̂������l�H�v�Z
	---------------------------------------
	-- 30���O
	SET @DATE_LAST_MONTH = DATEADD(dd, -30, @TARGET_DATE)
	-- 60���O
	SET @DATE_LAST_MONTH2 = DATEADD(dd, -60, @TARGET_DATE)

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
		-- ���݃��[�U�W�v
		---------------------------------------
		-- �V�K�l�����݃��[�U���擾
		--   �V�K�l�����[�U�͑��F�m�ڋq�ɂȂ����l���܂߂邽�߁A
		--   user_id���󂩂̔��f�͍s��Ȃ��B
		SELECT	@POTENTIAL_NEW = COUNT(*)
		  FROM	w2_AccessUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	YEAR(first_acc_date) = YEAR(@TARGET_DATE)
		   AND	MONTH(first_acc_date) = MONTH(@TARGET_DATE)
		   AND	DAY(first_acc_date) = DAY(@TARGET_DATE)

		-- ���݃��[�U���擾
		SELECT	@POTENTIAL_ALL = COUNT(*)
		  FROM	w2_AccessUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	user_id = ''

		-- ���݃A�N�e�B�����[�U���擾�i�ߋ��R�O���ȓ��ɍŏI�A�N�Z�X���������[�U�j
		SELECT	@POTENTIAL_ACTIVE = COUNT(*)
		  FROM	w2_AccessUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	user_id = ''
		   AND	last_acc_date >= @DATE_LAST_MONTH

		-- ���݋x�����[�U���擾�i�R�P���`�U�O���O�ȓ��ɍŏI�A�N�Z�X���������[�U�j
		SELECT	@POTENTIAL_UNACTIVE1 = COUNT(*)
		  FROM	w2_AccessUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	user_id = ''
		   AND	last_acc_date < @DATE_LAST_MONTH
		   AND	last_acc_date >= @DATE_LAST_MONTH2

		-- ���݋x�����[�U���擾�i�U�P���O�ȑO�ɂɍŏI�A�N�Z�X���������[�U�j
		SELECT	@POTENTIAL_UNACTIVE2 = COUNT(*)
		  FROM	w2_AccessUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	user_id = ''
		   AND	last_acc_date < @DATE_LAST_MONTH2

		---------------------------------------
		-- �F�m�ڋq�W�v�i���މ�[�U�͊܂߂Ȃ��j
		---------------------------------------
		-- �V�K�l���F�m���[�U���擾�i�ꃖ���ԁj
		SELECT	@RECOGNIZE_NEW = COUNT(*)
		  FROM	w2_AccessRecUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	YEAR(recognized_date) = YEAR(@TARGET_DATE)
		   AND	MONTH(recognized_date) = MONTH(@TARGET_DATE)
		   AND	DAY(recognized_date) = DAY(@TARGET_DATE)
		   AND	leave_date IS NULL

		-- �F�m�ڋq���擾	�i�މ�[�U���܂߂�j
		SELECT	@RECOGNIZE_ALL = COUNT(*)
		  FROM	w2_AccessRecUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	leave_date IS NULL

		-- �F�m�A�N�e�B���ڋq���擾�i�ߋ��R�O���ȓ��ɍŏI�A�N�Z�X���������[�U�j
		SELECT	@RECOGNIZE_ACTIVE = COUNT(*)
		  FROM	w2_AccessRecUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	last_acc_date >= @DATE_LAST_MONTH
		   AND	leave_date IS NULL

		-- �F�m�x���ڋq���擾�i�R�P���`�U�O���O�ȓ��ɍŏI�A�N�Z�X���������[�U�j
		SELECT	@RECOGNIZE_UNACTIVE1 = COUNT(*)
		  FROM	w2_AccessRecUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	last_acc_date< @DATE_LAST_MONTH
		   AND	last_acc_date >= @DATE_LAST_MONTH2
		   AND	leave_date IS NULL
		
		-- �F�m�x���ڋq���擾�i�U�P���O�ȑO�ɂɍŏI�A�N�Z�X���������[�U�j
		SELECT	@RECOGNIZE_UNACTIVE2 = COUNT(*)
		  FROM	w2_AccessRecUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	last_acc_date < @DATE_LAST_MONTH2
		   AND	leave_date IS NULL

		---------------------------------------
		-- �މ�ڋq�W�v
		---------------------------------------
		-- �މ�ڋq�V�K�l�����擾
		SELECT	@LEAVE_NEW = COUNT(*)
		  FROM	w2_AccessRecUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	leave_date IS NOT NULL
		   AND	YEAR(leave_date) = YEAR(@TARGET_DATE)
		   AND	MONTH(leave_date) = MONTH(@TARGET_DATE)
		   AND	DAY(leave_date) = DAY(@TARGET_DATE)

		-- �މ�ڋq���擾
		SELECT	@LEAVE_ALL = COUNT(*)
		  FROM	w2_AccessRecUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	leave_date IS NOT NULL

		---------------------------------------
		-- ���R�[�h�}��
		---------------------------------------
		-- DEETE/INSERT
		DELETE
		  FROM	w2_DispUserAnalysis
		 WHERE	dept_id = @DEPT_ID
		   AND	tgt_year = YEAR(@TARGET_DATE)
		   AND	tgt_month = MONTH(@TARGET_DATE)
		   AND	tgt_day = DAY(@TARGET_DATE)

		INSERT w2_DispUserAnalysis
			(
			dept_id,
			tgt_year,
			tgt_month,
			tgt_day,
			potential_new,
			potential_all,
			potential_active,
			potential_unactive1,
			potential_unactive2,
			recognize_new,
			recognize_all,
			recognize_active,
			recognize_unactive1,
			recognize_unactive2,
			leave_new,
			leave_all
			)
		VALUES
			(
			@DEPT_ID,
			YEAR(@TARGET_DATE),
			RIGHT('00' + CONVERT(varchar,MONTH(@TARGET_DATE)), 2),
			RIGHT('00' + CONVERT(varchar,DAY(@TARGET_DATE)), 2),
			@POTENTIAL_NEW,
			@POTENTIAL_ALL,
			@POTENTIAL_ACTIVE,
			@POTENTIAL_UNACTIVE1,
			@POTENTIAL_UNACTIVE2,
			@RECOGNIZE_NEW,
			@RECOGNIZE_ALL,
			@RECOGNIZE_ACTIVE,
			@RECOGNIZE_UNACTIVE1,
			@RECOGNIZE_UNACTIVE2,
			@LEAVE_NEW,
			@LEAVE_ALL
			)
	END
	
	---------------------------------------
	-- �J�[�\������
	---------------------------------------
	CLOSE CUR_DEPTID
	DEALLOCATE CUR_DEPTID
GO