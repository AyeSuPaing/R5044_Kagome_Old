if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CreateUserAccessDataSp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[w2_CreateUserAccessDataSp]
GO
/*
=========================================================================================================
  Module      : 日次ユーザアクセスデータ作成プロシージャ(w2_CreateUserAccessDataSp.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE PROCEDURE [dbo].[w2_CreateUserAccessDataSp] (
					@TARGET_DATE varchar(10)) AS

	---------------------------------------
	-- 変数定義
	---------------------------------------
	DECLARE @TOTAL_PAGEVIEWS bigint
	DECLARE @TOTAL_USERS bigint
	DECLARE @TOTAL_SESSIONS bigint
	DECLARE @TOTAL_NEW_USERS bigint
	DECLARE @TOTAL_PAGEVIEWS_MOBILE bigint
	DECLARE @TOTAL_SESSIONS_MOBILE bigint
	DECLARE @DEPT_ID varchar(30)

	---------------------------------------
	-- dept_idカーソル定義
	---------------------------------------
	DECLARE CUR_DEPTID CURSOR FOR
	SELECT	DISTINCT dept_id
	  FROM	w2_AccessLogAccount

	---------------------------------------
	-- 日付分割
	---------------------------------------
	DECLARE @TGT_YEAR varchar(4)
	DECLARE @TGT_MONTH varchar(2)
	DECLARE @TGT_DAY varchar(2)
	SET @TGT_YEAR = SUBSTRING(@TARGET_DATE, 0, 5)
	SET @TGT_MONTH = SUBSTRING(@TARGET_DATE, 6, 2)
	SET @TGT_DAY = SUBSTRING(@TARGET_DATE, 9, 2)

	---------------------------------------
	-- カーソル開く・最終行までループ
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
		--レコード削除（デリートインサートのため）
		---------------------------------------
		DELETE	w2_DispAccessAnalysis
		 WHERE	dept_id = @DEPT_ID
		   AND	tgt_year = @TGT_YEAR
		   AND	tgt_month = @TGT_MONTH
		   AND	tgt_day = @TGT_DAY

		---------------------------------------
		-- ＰＣ：ページビュー数取得
		---------------------------------------
		SELECT	@TOTAL_PAGEVIEWS = COUNT(*)
		  FROM	w2_AccessLog
		 WHERE	access_date = @TARGET_DATE
		   AND	dept_id = @DEPT_ID
		   AND	user_agent_kbn = 'PC'

		---------------------------------------
		-- ＰＣ：ユーザー数取得
		---------------------------------------
		SELECT	@TOTAL_USERS = COUNT(DISTINCT access_user_id)
		  FROM	w2_AccessLog
		 WHERE	access_date = @TARGET_DATE
		   AND	dept_id = @DEPT_ID
		   AND	user_agent_kbn = 'PC'
		
		---------------------------------------
		-- ＰＣ：訪問者数（セッション数）取得
		---------------------------------------
		SELECT	@TOTAL_SESSIONS = COUNT(DISTINCT session_id)
		  FROM	w2_AccessLog
		 WHERE	access_date = @TARGET_DATE
 		   AND	dept_id = @DEPT_ID
		   AND	user_agent_kbn = 'PC'

		---------------------------------------
		-- ＰＣ：新規訪問ユーザー数取得
		---------------------------------------
		SELECT	@TOTAL_NEW_USERS = COUNT(DISTINCT access_user_id)
		  FROM	w2_AccessLog
		 WHERE	access_date = @TARGET_DATE
 		   AND	dept_id = @DEPT_ID
 		   AND	access_interval = -1	-- 初回アクセス
		   AND	user_agent_kbn = 'PC'

		---------------------------------------
		-- ＰＣ：レコード挿入
		---------------------------------------
		-- インサート
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
		-- モバイル：ページビュー数取得
		---------------------------------------
		SELECT	@TOTAL_PAGEVIEWS_MOBILE = COUNT(*)
		  FROM	w2_AccessLogMobile
		 WHERE	access_date = @TARGET_DATE
		   AND	dept_id = @DEPT_ID

		---------------------------------------
		-- モバイル：訪問者数（セッション数）取得
		---------------------------------------
		SELECT	@TOTAL_SESSIONS_MOBILE = COUNT(DISTINCT session_id)
		  FROM	w2_AccessLogMobile
		 WHERE	access_date = @TARGET_DATE
 		   AND	dept_id = @DEPT_ID
			
		---------------------------------------
		-- モバイル：レコード挿入
		---------------------------------------
		-- インサート
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
			'1',	-- モバイル
			@TOTAL_PAGEVIEWS_MOBILE,
			@TOTAL_SESSIONS_MOBILE
			)

		---------------------------------------
		-- スマートフォン：ページビュー数取得
		---------------------------------------
		SELECT	@TOTAL_PAGEVIEWS = COUNT(*)
		  FROM	w2_AccessLog
		 WHERE	access_date = @TARGET_DATE
		   AND	dept_id = @DEPT_ID
		   AND	user_agent_kbn = 'SP'

		---------------------------------------
		-- スマートフォン：ユーザー数取得
		---------------------------------------
		SELECT	@TOTAL_USERS = COUNT(DISTINCT access_user_id)
		  FROM	w2_AccessLog
		 WHERE	access_date = @TARGET_DATE
		   AND	dept_id = @DEPT_ID
		   AND	user_agent_kbn = 'SP'
		
		---------------------------------------
		-- スマートフォン：訪問者数（セッション数）取得
		---------------------------------------
		SELECT	@TOTAL_SESSIONS = COUNT(DISTINCT session_id)
		  FROM	w2_AccessLog
		 WHERE	access_date = @TARGET_DATE
 		   AND	dept_id = @DEPT_ID
		   AND	user_agent_kbn = 'SP'

		---------------------------------------
		-- スマートフォン：新規訪問ユーザー数取得
		---------------------------------------
		SELECT	@TOTAL_NEW_USERS = COUNT(DISTINCT access_user_id)
		  FROM	w2_AccessLog
		 WHERE	access_date = @TARGET_DATE
 		   AND	dept_id = @DEPT_ID
 		   AND	access_interval = -1	-- 初回アクセス
		   AND	user_agent_kbn = 'SP'

		---------------------------------------
		-- スマートフォン：レコード挿入
		---------------------------------------
		-- インサート
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
			'2',	-- スマートフォン
			@TOTAL_PAGEVIEWS,
			@TOTAL_USERS,
			@TOTAL_SESSIONS,
			@TOTAL_NEW_USERS
			)
	END

	---------------------------------------
	-- カーソル閉じる
	---------------------------------------
	CLOSE CUR_DEPTID
	DEALLOCATE CUR_DEPTID
GO