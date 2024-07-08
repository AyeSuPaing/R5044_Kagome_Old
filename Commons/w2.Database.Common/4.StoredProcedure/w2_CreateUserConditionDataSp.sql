if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CreateUserConditionDataSp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[w2_CreateUserConditionDataSp]
GO
/*
=========================================================================================================
  Module      : 日次ユーザ状況データ作成プロシージャ(w2_CreateUserConditionDataSp.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE PROCEDURE [dbo].[w2_CreateUserConditionDataSp] (
					@TARGET_DATE varchar(10)) AS

	---------------------------------------
	-- 変数定義
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
	-- dept_idカーソル定義
	---------------------------------------
	DECLARE CUR_DEPTID CURSOR FOR
	SELECT	DISTINCT dept_id
	  FROM	w2_AccessLogAccount

	---------------------------------------
	-- 休眠ユーザのしきい値？計算
	---------------------------------------
	-- 30月前
	SET @DATE_LAST_MONTH = DATEADD(dd, -30, @TARGET_DATE)
	-- 60月前
	SET @DATE_LAST_MONTH2 = DATEADD(dd, -60, @TARGET_DATE)

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
		-- 潜在ユーザ集計
		---------------------------------------
		-- 新規獲得潜在ユーザ数取得
		--   新規獲得ユーザは即認知顧客になった人も含めるため、
		--   user_idが空かの判断は行わない。
		SELECT	@POTENTIAL_NEW = COUNT(*)
		  FROM	w2_AccessUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	YEAR(first_acc_date) = YEAR(@TARGET_DATE)
		   AND	MONTH(first_acc_date) = MONTH(@TARGET_DATE)
		   AND	DAY(first_acc_date) = DAY(@TARGET_DATE)

		-- 潜在ユーザ数取得
		SELECT	@POTENTIAL_ALL = COUNT(*)
		  FROM	w2_AccessUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	user_id = ''

		-- 潜在アクティヴユーザ数取得（過去３０日以内に最終アクセス日をもつユーザ）
		SELECT	@POTENTIAL_ACTIVE = COUNT(*)
		  FROM	w2_AccessUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	user_id = ''
		   AND	last_acc_date >= @DATE_LAST_MONTH

		-- 潜在休眠ユーザ数取得（３１日～６０日前以内に最終アクセス日をもつユーザ）
		SELECT	@POTENTIAL_UNACTIVE1 = COUNT(*)
		  FROM	w2_AccessUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	user_id = ''
		   AND	last_acc_date < @DATE_LAST_MONTH
		   AND	last_acc_date >= @DATE_LAST_MONTH2

		-- 潜在休眠ユーザ数取得（６１日前以前にに最終アクセス日をもつユーザ）
		SELECT	@POTENTIAL_UNACTIVE2 = COUNT(*)
		  FROM	w2_AccessUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	user_id = ''
		   AND	last_acc_date < @DATE_LAST_MONTH2

		---------------------------------------
		-- 認知顧客集計（※退会ユーザは含めない）
		---------------------------------------
		-- 新規獲得認知ユーザ数取得（一ヶ月間）
		SELECT	@RECOGNIZE_NEW = COUNT(*)
		  FROM	w2_AccessRecUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	YEAR(recognized_date) = YEAR(@TARGET_DATE)
		   AND	MONTH(recognized_date) = MONTH(@TARGET_DATE)
		   AND	DAY(recognized_date) = DAY(@TARGET_DATE)
		   AND	leave_date IS NULL

		-- 認知顧客数取得	（退会ユーザを含める）
		SELECT	@RECOGNIZE_ALL = COUNT(*)
		  FROM	w2_AccessRecUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	leave_date IS NULL

		-- 認知アクティヴ顧客数取得（過去３０日以内に最終アクセス日をもつユーザ）
		SELECT	@RECOGNIZE_ACTIVE = COUNT(*)
		  FROM	w2_AccessRecUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	last_acc_date >= @DATE_LAST_MONTH
		   AND	leave_date IS NULL

		-- 認知休眠顧客数取得（３１日～６０日前以内に最終アクセス日をもつユーザ）
		SELECT	@RECOGNIZE_UNACTIVE1 = COUNT(*)
		  FROM	w2_AccessRecUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	last_acc_date< @DATE_LAST_MONTH
		   AND	last_acc_date >= @DATE_LAST_MONTH2
		   AND	leave_date IS NULL
		
		-- 認知休眠顧客数取得（６１日前以前にに最終アクセス日をもつユーザ）
		SELECT	@RECOGNIZE_UNACTIVE2 = COUNT(*)
		  FROM	w2_AccessRecUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	last_acc_date < @DATE_LAST_MONTH2
		   AND	leave_date IS NULL

		---------------------------------------
		-- 退会顧客集計
		---------------------------------------
		-- 退会顧客新規獲得数取得
		SELECT	@LEAVE_NEW = COUNT(*)
		  FROM	w2_AccessRecUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	leave_date IS NOT NULL
		   AND	YEAR(leave_date) = YEAR(@TARGET_DATE)
		   AND	MONTH(leave_date) = MONTH(@TARGET_DATE)
		   AND	DAY(leave_date) = DAY(@TARGET_DATE)

		-- 退会顧客数取得
		SELECT	@LEAVE_ALL = COUNT(*)
		  FROM	w2_AccessRecUserMaster
		 WHERE	dept_id = @DEPT_ID
		   AND	leave_date IS NOT NULL

		---------------------------------------
		-- レコード挿入
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
	-- カーソル閉じる
	---------------------------------------
	CLOSE CUR_DEPTID
	DEALLOCATE CUR_DEPTID
GO