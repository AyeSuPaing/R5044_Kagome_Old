 if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CreateSummarySrchEngineSp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[w2_CreateSummarySrchEngineSp]
GO
/*
=========================================================================================================
  Module      : �����G���W���쐬�v���V�[�W��(w2_CreateSummarySrchEngineSp.sql)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE PROCEDURE [dbo].[w2_CreateSummarySrchEngineSp] (
					@TARGET_DATE varchar(10)) AS

	---------------------------------------
	-- �ϐ���`
	---------------------------------------
	DECLARE @DEPT_ID nvarchar(30)
	DECLARE @SUMMARY_KBN nvarchar(30)
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
		
		---------------------------------------
		-- �e�����G���W�����T�}���쐬�iPC�j
		---------------------------------------
		SET @SUMMARY_KBN = 'srchengine'
		
		-- ������
		DELETE FROM @tmp_table
		
		-- �e�����G���W�����擾�E���e�[�u���Ɋi�[
		INSERT
		  INTO	@tmp_table
				(
					value_name,
					counts
				)
				SELECT	a.search_engine,
						SUM(a.count) AS count
				FROM	(
						SELECT 	search_engine,
								count(*) AS count
						  FROM	w2_AccessLog
						 WHERE	dept_id = @DEPT_ID
						   AND	access_date = @TARGET_DATE
						   AND	search_engine <> ''
						   AND	user_agent_kbn = 'PC'
						GROUP BY search_engine, url_domain
					) a
				GROUP BY search_engine
				ORDER BY count DESC
		
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
		
		---------------------------------------
		-- �e�����G���W�����T�}���쐬�i�X�}�[�g�t�H���j
		---------------------------------------
		SET @SUMMARY_KBN = 'srchengine_sp'
		
		-- ������
		DELETE FROM @tmp_table
		
		-- �e�����G���W�����擾�E���e�[�u���Ɋi�[
		INSERT
		  INTO	@tmp_table
				(
					value_name,
					counts
				)
				SELECT	a.search_engine,
						SUM(a.count) AS count
				FROM	(
						SELECT 	search_engine,
								count(*) AS count
						  FROM	w2_AccessLog
						 WHERE	dept_id = @DEPT_ID
						   AND	access_date = @TARGET_DATE
						   AND	search_engine <> ''
						   AND	user_agent_kbn = 'SP'
						GROUP BY search_engine, url_domain
					) a
				GROUP BY search_engine
				ORDER BY count DESC
		
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