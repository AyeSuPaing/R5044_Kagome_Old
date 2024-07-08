 if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CreateSummaryMobileNameSp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[w2_CreateSummaryMobileNameSp]
GO
/*
=========================================================================================================
  Module      : ���o�C���@�햼�T�}���쐬�v���V�[�W��(w2_CreateSummaryMobileNameSp.sql)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE PROCEDURE [dbo].[w2_CreateSummaryMobileNameSp] (
					@TARGET_DATE varchar(10)) AS

	---------------------------------------
	-- �萔�ݒ�
	---------------------------------------
	DECLARE @SUMMARY_KBN nvarchar(30)
	SET @SUMMARY_KBN = 'mobile_modelname'

	---------------------------------------
	-- �ϐ���`
	---------------------------------------
	DECLARE @DEPT_ID nvarchar(30)
	DECLARE @TGT_YEAR nvarchar(4)
	DECLARE @TGT_MONTH nvarchar(2)
	DECLARE @TGT_DAY nvarchar(2)
	
	DECLARE @tmp_table TABLE(
		[value_name] [nvarchar] (1000) NOT NULL DEFAULT (N''),
		[counts] [bigint] NOT NULL DEFAULT (0)
	)

	---------------------------------------
	-- dept_id�J�[�\����`
	---------------------------------------
	DECLARE CUR_DEPTID CURSOR FOR
	SELECT	DISTINCT dept_id
	  FROM	w2_AccessLogAccount

	---------------------------------------
	-- ���t����
	---------------------------------------
	SET @TGT_YEAR = SUBSTRING(@TARGET_DATE, 0, 5)
	SET @TGT_MONTH = SUBSTRING(@TARGET_DATE, 6, 2)
	SET @TGT_DAY = SUBSTRING(@TARGET_DATE, 9, 2)

	---------------------------------------
	-- ����ID�J�[�\���J���E�ŏI�s�܂Ń��[�v
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
		
		-- ������
		DELETE FROM @tmp_table
		
		---------------------------------------
		-- �A�N�Z�X�y�[�W���擾�E���e�[�u���Ɋi�[
		---------------------------------------
		INSERT
		  INTO	@tmp_table
				(
					value_name,
					counts
				)
				SELECT	a.mobile_model_name,
						SUM(a.count) AS count
				FROM	(
						SELECT 	mobile_model_name,
								count(*) AS count
						  FROM	w2_AccessLogMobile
						 WHERE	dept_id = @DEPT_ID
						   AND	access_date = @TARGET_DATE
						GROUP BY mobile_model_name
					) a
				GROUP BY mobile_model_name
				ORDER BY count DESC
				
		---------------------------------------
		-- �f���[�g�E�C���T�[�g
		---------------------------------------
		-- �f���[�g
		DELETE
		  FROM	w2_DispSummaryAnalysis
		 WHERE	dept_id = @DEPT_ID
		   AND	summary_kbn = @SUMMARY_KBN
		   AND	tgt_year = @TGT_YEAR
		   AND	tgt_month = @TGT_MONTH
		   AND	tgt_day = @TGT_DAY
		
		-- �C���T�[�g
		INSERT
		  INTO	w2_DispSummaryAnalysis
				(
				dept_id,
				summary_kbn,
				tgt_year,
				tgt_month,
				tgt_day,
				value_name,
				counts
				)
				SELECT	@DEPT_ID,
						@SUMMARY_KBN,
						@TGT_YEAR,
						@TGT_MONTH,
						@TGT_DAY,
						value_name,
						counts
				  FROM	@tmp_table
	END

	---------------------------------------
	-- �J�[�\������
	---------------------------------------
	CLOSE CUR_DEPTID
	DEALLOCATE CUR_DEPTID
GO