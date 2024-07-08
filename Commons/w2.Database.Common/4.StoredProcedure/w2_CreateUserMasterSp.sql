if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CreateUserMasterSp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[w2_CreateUserMasterSp]
GO
/*
=========================================================================================================
  Module      : ユーザマスタ作成プロシージャ(w2_CreateUserMasterSp.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE PROCEDURE [dbo].[w2_CreateUserMasterSp] (@TARGET_DATE varchar(10)) AS
	---------------------------------------
	-- 定数定義
	---------------------------------------
	DECLARE @KBN_ACTION_LOGIN varchar(20)
	DECLARE @KBN_ACTION_LEAVE varchar(20)
	SET @KBN_ACTION_LOGIN = '__login'
	SET @KBN_ACTION_LEAVE = '__leave'

	---------------------------------------
	-- 変数定義
	---------------------------------------
	DECLARE @TMP_COUNT int

	-- カーソルデータ格納用
	DECLARE @VAL_ACCESS_DATE varchar(20)
	DECLARE @VAL_ACCESS_TIME varchar(20)
	DECLARE @VAL_CLIENT_IP varchar(20)
	DECLARE @VAL_SERVER_NAME varchar(20)
	DECLARE @VAL_SERVER_IP varchar(20)
	DECLARE @VAL_SERVER_PORT int
	DECLARE @VAL_PROTOCOL_STATUS int
	DECLARE @VAL_USER_AGENT varchar(512)
	DECLARE @VAL_URL_DOMAIN varchar(50)
	DECLARE @VAL_URL_PAGE varchar(1000)
	DECLARE @VAL_URL_PARAM varchar(1000)
	DECLARE @VAL_DEPT_ID nvarchar(30)
	DECLARE @VAL_ACCOUNT_ID nvarchar(30)
	DECLARE @VAL_ACCESS_USER_ID varchar(255)
	DECLARE @VAL_SESSION_ID varchar(255)
	DECLARE @VAL_REAL_USER_ID varchar(50)
	DECLARE @VAL_FIRST_LOGIN_FLG varchar(2)
	DECLARE @VAL_REFERRER_DOMAIN varchar(50)
	DECLARE @VAL_REFERRER_PAGE varchar(1000)
	DECLARE @VAL_REFERRER_PARAM varchar(1000)
	DECLARE @VAL_ACTION_KBN varchar(20)
	DECLARE @VAL_S1 varchar(20)
	DECLARE @VAL_S2 varchar(20)
	DECLARE @VAL_S3 varchar(20)
	DECLARE @VAL_S4 varchar(20)
	DECLARE @VAL_S5 varchar(20)
	DECLARE @VAL_S6 varchar(20)
	DECLARE @VAL_S7 varchar(20)
	DECLARE @VAL_S8 varchar(20)
	DECLARE @VAL_S9 varchar(20)
	DECLARE @VAL_S10 varchar(20)
	DECLARE @VAL_S11 varchar(50)
	DECLARE @VAL_S12 varchar(50)
	DECLARE @VAL_S13 varchar(50)
	DECLARE @VAL_S14 varchar(50)
	DECLARE @VAL_S15 varchar(50)
	DECLARE @VAL_S16 varchar(50)
	DECLARE @VAL_S17 varchar(50)
	DECLARE @VAL_S18 varchar(50)
	DECLARE @VAL_S19 varchar(50)
	DECLARE @VAL_S20 varchar(50)
	DECLARE @VAL_P1 varchar(20)
	DECLARE @VAL_P2 varchar(20)
	DECLARE @VAL_P3 varchar(20)
	DECLARE @VAL_P4 varchar(20)
	DECLARE @VAL_P5 varchar(20)
	DECLARE @VAL_P6 varchar(20)
	DECLARE @VAL_P7 varchar(20)
	DECLARE @VAL_P8 varchar(20)
	DECLARE @VAL_P9 varchar(20)
	DECLARE @VAL_P10 varchar(20)
	DECLARE @VAL_P11 varchar(50)
	DECLARE @VAL_P12 varchar(50)
	DECLARE @VAL_P13 varchar(50)
	DECLARE @VAL_P14 varchar(50)
	DECLARE @VAL_P15 varchar(50)
	DECLARE @VAL_P16 varchar(50)
	DECLARE @VAL_P17 varchar(50)
	DECLARE @VAL_P18 varchar(50)
	DECLARE @VAL_P19 varchar(50)
	DECLARE @VAL_P20 varchar(50)
	DECLARE @VAL_USER_AGENT_KBN varchar(10)

	---------------------------------------
	-- カーソル定義
	---------------------------------------
	DECLARE CUR_ACCESSLOG CURSOR FOR
	SELECT	access_date,
			access_time,
			client_ip,
			server_name,
			server_ip,
			server_port,
			protocol_status,
			user_agent,
			url_domain,
			url_page,
			url_param,
			dept_id,
			account_id,
			access_user_id,
			session_id,
			real_user_id,
			first_login_flg,
			referrer_domain,
			referrer_page,
			referrer_param,
			action_kbn,
			s1,
			s2,
			s3,
			s4,
			s5,
			s6,
			s7,
			s8,
			s9,
			s10,
			s11,
			s12,
			s13,
			s14,
			s15,
			s16,
			s17,
			s18,
			s19,
			s20,
			p1,
			p2,
			p3,
			p4,
			p5,
			p6,
			p7,
			p8,
			p9,
			p10,
			p11,
			p12,
			p13,
			p14,
			p15,
			p16,
			p17,
			p18,
			p19,
			p20,
			user_agent_kbn
	  FROM	w2_AccessLog
	 WHERE	access_date = @TARGET_DATE

	---------------------------------------
	-- 開く
	---------------------------------------
	OPEN CUR_ACCESSLOG

	---------------------------------------
	-- 最終行までループ
	---------------------------------------
	WHILE (1=1)
	BEGIN
		---------------------------------------
		-- ①一行を取り出して変数に代入する
		---------------------------------------
		FETCH NEXT FROM CUR_ACCESSLOG
			INTO	@VAL_ACCESS_DATE,
					@VAL_ACCESS_TIME,
					@VAL_CLIENT_IP,
					@VAL_SERVER_NAME,
					@VAL_SERVER_IP,
					@VAL_SERVER_PORT,
					@VAL_PROTOCOL_STATUS,
					@VAL_USER_AGENT,
					@VAL_URL_DOMAIN,
					@VAL_URL_PAGE,
					@VAL_URL_PARAM,
					@VAL_DEPT_ID,
					@VAL_ACCOUNT_ID,
					@VAL_ACCESS_USER_ID,
					@VAL_SESSION_ID,
					@VAL_REAL_USER_ID,
					@VAL_FIRST_LOGIN_FLG,
					@VAL_REFERRER_DOMAIN,
					@VAL_REFERRER_PAGE,
					@VAL_REFERRER_PARAM,
					@VAL_ACTION_KBN,
					@VAL_S1,
					@VAL_S2,
					@VAL_S3,
					@VAL_S4,
					@VAL_S5,
					@VAL_S6,
					@VAL_S7,
					@VAL_S8,
					@VAL_S9,
					@VAL_S10,
					@VAL_S11,
					@VAL_S12,
					@VAL_S13,
					@VAL_S14,
					@VAL_S15,
					@VAL_S16,
					@VAL_S17,
					@VAL_S18,
					@VAL_S19,
					@VAL_S20,
					@VAL_P1,
					@VAL_P2,
					@VAL_P3,
					@VAL_P4,
					@VAL_P5,
					@VAL_P6,
					@VAL_P7,
					@VAL_P8,
					@VAL_P9,
					@VAL_P10,
					@VAL_P11,
					@VAL_P12,
					@VAL_P13,
					@VAL_P14,
					@VAL_P15,
					@VAL_P16,
					@VAL_P17,
					@VAL_P18,
					@VAL_P19,
					@VAL_P20,
					@VAL_USER_AGENT_KBN

		-- 終端なら抜ける
		IF @@FETCH_STATUS != 0
		BEGIN
			BREAK
		END
		
		---------------------------------------
		-- ②ユーザマスタ更新処理（潜在・認知両方）
		---------------------------------------
		IF @VAL_ACCESS_USER_ID <> ''
		BEGIN
			-- INSERT/UPDATE判定
			SELECT	@TMP_COUNT = COUNT(*)
			  FROM	w2_AccessUserMaster
			 WHERE	dept_id = @VAL_DEPT_ID
			   AND	access_user_id = @VAL_ACCESS_USER_ID

			---------------------------------------
			--- ユーザマスタインサート処理
			---------------------------------------
			IF @TMP_COUNT = 0
			BEGIN
				-- 潜在ユーザインサート
				INSERT w2_AccessUserMaster
					(
					dept_id,
					access_user_id,
					user_id,
					first_acc_date,
					last_acc_date,
					recognized_date)
				VALUES(
					@VAL_DEPT_ID,
					@VAL_ACCESS_USER_ID,
					'',
					@VAL_ACCESS_DATE + ' ' + @VAL_ACCESS_TIME,
					@VAL_ACCESS_DATE + ' ' + @VAL_ACCESS_TIME,
					NULL
				)
				
			END
			---------------------------------------
			-- ユーザマスタアップデート処理
			---------------------------------------
			ELSE
			BEGIN
				-- 潜在ユーザアップデート
				UPDATE	w2_AccessUserMaster
				   SET	last_acc_date = @VAL_ACCESS_DATE + ' ' + @VAL_ACCESS_TIME
				 WHERE	dept_id = @VAL_DEPT_ID
				   AND	access_user_id = @VAL_ACCESS_USER_ID
			END
		END
		
		---------------------------------------
		-- ③認知顧客マスタ更新処理：認知顧客のみ
		---------------------------------------
		IF @VAL_REAL_USER_ID <> ''
		BEGIN
			-- INSERT/UPDATE判定
			SELECT	@TMP_COUNT = COUNT(*)
			  FROM 	w2_AccessRecUserMaster
			 WHERE	dept_id = @VAL_DEPT_ID
			   AND	user_id = @VAL_REAL_USER_ID

			---------------------------------------
			-- 認知顧客インサート処理
			---------------------------------------
			IF @TMP_COUNT = 0
			BEGIN
				-- 認知顧客インサート
				INSERT w2_AccessRecUserMaster
					(
					dept_id,
					user_id,
					last_acc_date,
					recognized_date,
					leave_date,
					last_login_date)
				VALUES(
					@VAL_DEPT_ID,
					@VAL_REAL_USER_ID,
					@VAL_ACCESS_DATE + ' ' + @VAL_ACCESS_TIME,
					@VAL_ACCESS_DATE + ' ' + @VAL_ACCESS_TIME,
					NULL,
					@VAL_ACCESS_DATE + ' ' + @VAL_ACCESS_TIME
					)
				
				-- ユーザマスタ紐付け更新
				UPDATE	w2_AccessUserMaster
				   SET	user_id = @VAL_REAL_USER_ID,
						recognized_date = @VAL_ACCESS_DATE + ' ' + @VAL_ACCESS_TIME
				 WHERE	dept_id = @VAL_DEPT_ID
				   AND	access_user_id = @VAL_ACCESS_USER_ID

			END
			---------------------------------------
			-- 認知顧客アップデート処理
			---------------------------------------
			ELSE
			BEGIN
				-- 認知顧客アップデート（アクセス日）
				UPDATE	w2_AccessRecUserMaster
				   SET	last_acc_date = @VAL_ACCESS_DATE + ' ' + @VAL_ACCESS_TIME
				 WHERE	dept_id = @VAL_DEPT_ID
				   AND	user_id = @VAL_REAL_USER_ID

				-- クライアント側でユーザIDのクッキーが初めて保存されたときにこのフラグが立つ。
				-- ユーザが別マシンへアクセスしたとき、実際は認知顧客であってもログインするまでは
				-- (ユーザIDクッキーがないので)潜在ユーザとして識別されてしまう。
				-- ここでログインを行ったとき、ユーザテーブルに既にレコードがあるのでアップデート処理となるが、
				-- 潜在ユーザのレコードにユーザIDを紐づけてやる必要が出てくる。
				-- ※ユーザマスタと潜在顧客マスタとの間には多対一の関係が成り立つ。
				IF @VAL_FIRST_LOGIN_FLG = '1'
				BEGIN
					-- 更新判定
					SELECT	@TMP_COUNT = COUNT(*)
					  FROM	w2_AccessUserMaster
					 WHERE	dept_id = @VAL_DEPT_ID
					   AND	user_id = @VAL_REAL_USER_ID

					-- 新たにユーザIDを紐づける
					IF  @TMP_COUNT = 0
					BEGIN
						-- ユーザID紐付け更新
						UPDATE	w2_AccessUserMaster
						   SET	user_id = @VAL_REAL_USER_ID,
								recognized_date = @VAL_ACCESS_DATE + ' ' + @VAL_ACCESS_TIME
						 WHERE	dept_id = @VAL_DEPT_ID
						   AND	access_user_id = @VAL_ACCESS_USER_ID
					END
				END
			END

			---------------------------------------
			-- 認知顧客共通処理
			---------------------------------------
			-- ログオン判定
			IF @VAL_ACTION_KBN = @KBN_ACTION_LOGIN
			BEGIN
				-- 認知顧客アップデート（ログイン日付）
				UPDATE	w2_AccessRecUserMaster
				   SET	last_login_date = @VAL_ACCESS_DATE + ' ' + @VAL_ACCESS_TIME
				 WHERE	dept_id = @VAL_DEPT_ID
				   AND	user_id = @VAL_REAL_USER_ID
			END
			-- 退会判定
			IF @VAL_ACTION_KBN = @KBN_ACTION_LEAVE
			BEGIN
				-- 認知顧客アップデート（退会日付）
				UPDATE	w2_AccessRecUserMaster
				   SET	leave_date = @VAL_ACCESS_DATE + ' ' + @VAL_ACCESS_TIME
				 WHERE	dept_id = @VAL_DEPT_ID
				   AND	user_id = @VAL_REAL_USER_ID
			END

		END -- 潜在ユーザ・認知顧客データ更新

	END -- WHILE (1=1) 行ごと

	---------------------------------------
	-- 閉じる
	---------------------------------------
	CLOSE CUR_ACCESSLOG
	DEALLOCATE CUR_ACCESSLOG

GO