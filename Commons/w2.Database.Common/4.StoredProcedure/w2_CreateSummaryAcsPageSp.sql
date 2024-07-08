if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CreateSummaryAcsPageSp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[w2_CreateSummaryAcsPageSp]
GO
/*
=========================================================================================================
  Module      : アクセスページサマリ作成プロシージャ(w2_CreateSummaryAcsPageSp.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE PROCEDURE [dbo].[w2_CreateSummaryAcsPageSp] (
					@TARGET_DATE varchar(10)) AS

	---------------------------------------
	-- 定数設定
	---------------------------------------
	DECLARE @SUMMARY_KBN nvarchar(30)
	SET @SUMMARY_KBN = 'acspage'

	---------------------------------------
	-- 変数定義
	---------------------------------------
	DECLARE @DEPT_ID nvarchar(30)
	DECLARE @TGT_YEAR nvarchar(4)
	DECLARE @TGT_MONTH nvarchar(2)
	DECLARE @TGT_DAY nvarchar(2)
	DECLARE @SCORING_SALE_TOP_URL NVARCHAR(MAX)
	DECLARE @SCORING_SALE_QUESTION_URL NVARCHAR(MAX)
	DECLARE @SCORING_SALE_RESULT_URL NVARCHAR(MAX)
	DECLARE @SEPARATOR NVarChar(1)
	
	DECLARE @tmp_table TABLE(
		[value_name] [nvarchar] (1000) NOT NULL DEFAULT (N''),
		[counts] [bigint] NOT NULL DEFAULT (0)
	)

	---------------------------------------
	-- dept_idカーソル定義
	---------------------------------------
	DECLARE CUR_DEPTID CURSOR FOR
	SELECT	DISTINCT dept_id
	  FROM	w2_AccessLogAccount

	---------------------------------------
	-- 日付分割
	---------------------------------------
	SET @TGT_YEAR = SUBSTRING(@TARGET_DATE, 0, 5)
	SET @TGT_MONTH = SUBSTRING(@TARGET_DATE, 6, 2)
	SET @TGT_DAY = SUBSTRING(@TARGET_DATE, 9, 2)
	SET @SCORING_SALE_TOP_URL = '/ScoringSale/Top.aspx'
	SET @SCORING_SALE_QUESTION_URL = '/ScoringSale/Question.aspx'
	SET @SCORING_SALE_RESULT_URL = '/ScoringSale/ResultPage.aspx'
	SET @SEPARATOR = '?'

	---------------------------------------
	-- 識別IDカーソル開く・最終行までループ
	---------------------------------------
	OPEN CUR_DEPTID

	WHILE (1=1)
	BEGIN
		---------------------------------------
		-- dept_id取得
		---------------------------------------
		FETCH NEXT FROM	CUR_DEPTID
		  INTO	@DEPT_ID

		-- 終端なら抜ける
		IF @@FETCH_STATUS != 0
		BEGIN
			BREAK
		END
		
		---------------------------------------
		-- アクセスページサマリ作成（PC）
		---------------------------------------
		SET @SUMMARY_KBN = 'acspage'
		
		-- 初期化
		DELETE FROM @tmp_table
		
		-- アクセスページ数取得・仮テーブルに格納
		INSERT
		  INTO	@tmp_table
				(
					value_name,
					counts
				)
				SELECT
					CASE WHEN
							(
								RIGHT(url_page, LEN(@SCORING_SALE_TOP_URL)) = @SCORING_SALE_TOP_URL
								OR
								RIGHT(url_page, LEN(@SCORING_SALE_QUESTION_URL)) = @SCORING_SALE_QUESTION_URL
								OR
								RIGHT(url_page, LEN(@SCORING_SALE_RESULT_URL)) = @SCORING_SALE_RESULT_URL
							)
							THEN a.url_page + @SEPARATOR + a.url_param
							ELSE a.url_page
						END,
						SUM(a.count) AS count
				FROM	(
						SELECT
							CASE WHEN
								(
									RIGHT(url_page, LEN(@SCORING_SALE_TOP_URL)) = @SCORING_SALE_TOP_URL
									OR
									RIGHT(url_page, LEN(@SCORING_SALE_QUESTION_URL)) = @SCORING_SALE_QUESTION_URL
									OR
									RIGHT(url_page, LEN(@SCORING_SALE_RESULT_URL)) = @SCORING_SALE_RESULT_URL
								)
								THEN url_page + @SEPARATOR + url_param
								ELSE url_page
							END AS url_page,
								url_param,
								count(*) AS count
						 FROM	w2_AccessLog
						WHERE	dept_id = @DEPT_ID
						  AND	access_date = @TARGET_DATE
						  AND	user_agent_kbn = 'PC'
						GROUP BY
							CASE WHEN
								(
									RIGHT(url_page, LEN(@SCORING_SALE_TOP_URL)) = @SCORING_SALE_TOP_URL
									OR
									RIGHT(url_page, LEN(@SCORING_SALE_QUESTION_URL)) = @SCORING_SALE_QUESTION_URL
									OR
									RIGHT(url_page, LEN(@SCORING_SALE_RESULT_URL)) = @SCORING_SALE_RESULT_URL
								)
								THEN url_page + @SEPARATOR + url_param
								ELSE url_page
								END,
							url_param
					) a
				GROUP BY
					CASE WHEN
							(
								RIGHT(url_page, LEN(@SCORING_SALE_TOP_URL)) = @SCORING_SALE_TOP_URL
								OR
								RIGHT(url_page, LEN(@SCORING_SALE_QUESTION_URL)) = @SCORING_SALE_QUESTION_URL
								OR
								RIGHT(url_page, LEN(@SCORING_SALE_RESULT_URL)) = @SCORING_SALE_RESULT_URL
							)
							THEN url_page + @SEPARATOR + url_param
							ELSE url_page
						END
				ORDER BY count DESC
		
		-- デリート
		DELETE
		  FROM	w2_DispSummaryAnalysis
		 WHERE	dept_id = @DEPT_ID
		   AND	summary_kbn = @SUMMARY_KBN
		   AND	tgt_year = @TGT_YEAR
		   AND	tgt_month = @TGT_MONTH
		   AND	tgt_day = @TGT_DAY
		
		-- インサート
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
		-- アクセスページサマリ作成（スマートフォン）
		---------------------------------------
		SET @SUMMARY_KBN = 'acspage_sp'
		
		-- 初期化
		DELETE FROM @tmp_table
		
		-- アクセスページ数取得・仮テーブルに格納
		INSERT
		  INTO	@tmp_table
				(
					value_name,
					counts
				)
		SELECT
					CASE WHEN
							(
								RIGHT(url_page, LEN(@SCORING_SALE_TOP_URL)) = @SCORING_SALE_TOP_URL
								OR
								RIGHT(url_page, LEN(@SCORING_SALE_QUESTION_URL)) = @SCORING_SALE_QUESTION_URL
								OR
								RIGHT(url_page, LEN(@SCORING_SALE_RESULT_URL)) = @SCORING_SALE_RESULT_URL
							)
							THEN a.url_page + @SEPARATOR + a.url_param
							ELSE a.url_page
						 END,
						SUM(a.count) AS count
				FROM	(
						SELECT
							CASE WHEN
								(
									RIGHT(url_page, LEN(@SCORING_SALE_TOP_URL)) = @SCORING_SALE_TOP_URL
									OR
									RIGHT(url_page, LEN(@SCORING_SALE_QUESTION_URL)) = @SCORING_SALE_QUESTION_URL
									OR
									RIGHT(url_page, LEN(@SCORING_SALE_RESULT_URL)) = @SCORING_SALE_RESULT_URL
								)
								THEN url_page + @SEPARATOR + url_param
								ELSE url_page
							END AS url_page,
								url_param,
								count(*) AS count
						  FROM	w2_AccessLog
						 WHERE	dept_id = @DEPT_ID
						   AND	access_date = @TARGET_DATE
						   AND	user_agent_kbn = 'SP'
						GROUP BY
							CASE WHEN
								(
									RIGHT(url_page, LEN(@SCORING_SALE_TOP_URL)) = @SCORING_SALE_TOP_URL
									OR
									RIGHT(url_page, LEN(@SCORING_SALE_QUESTION_URL)) = @SCORING_SALE_QUESTION_URL
									OR
									RIGHT(url_page, LEN(@SCORING_SALE_RESULT_URL)) = @SCORING_SALE_RESULT_URL
								)
								THEN url_page + @SEPARATOR + url_param
								ELSE url_page
								END,
							url_param
					) a
				GROUP BY
					CASE WHEN
							(
								RIGHT(url_page, LEN(@SCORING_SALE_TOP_URL)) = @SCORING_SALE_TOP_URL
								OR
								RIGHT(url_page, LEN(@SCORING_SALE_QUESTION_URL)) = @SCORING_SALE_QUESTION_URL
								OR
								RIGHT(url_page, LEN(@SCORING_SALE_RESULT_URL)) = @SCORING_SALE_RESULT_URL
							)
							THEN url_page + @SEPARATOR + url_param
							ELSE url_page
						 END
				ORDER BY count DESC
		
		-- デリート
		DELETE
		  FROM	w2_DispSummaryAnalysis
		 WHERE	dept_id = @DEPT_ID
		   AND	summary_kbn = @SUMMARY_KBN
		   AND	tgt_year = @TGT_YEAR
		   AND	tgt_month = @TGT_MONTH
		   AND	tgt_day = @TGT_DAY
		
		-- インサート
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
	-- カーソル閉じる
	---------------------------------------
	CLOSE CUR_DEPTID
	DEALLOCATE CUR_DEPTID
GO