if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AddTransactionLogSp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[w2_AddTransactionLogSp]
GO
/*
=========================================================================================================
  Module      : 処理ログ追加プロシージャ(w2_AddTransactionLogSp.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE PROCEDURE [dbo].w2_AddTransactionLogSp @TARGET_DATE varchar(10), @TRANSACTION_NAME varchar(30), @LOG_NO bigint OUTPUT AS

	IF @LOG_NO IS NULL
	BEGIN
		-- 処理ログインサート
		INSERT	w2_AccessLogWorkLog
				(
				target_date,
				transaction_name,
				begin_date,
				end_date
				)
		VALUES	(
				@TARGET_DATE,
				@TRANSACTION_NAME,
				getdate(),
				NULL
				)
		
		SET  @LOG_NO = @@IDENTITY
	END
	ELSE
	BEGIN
		-- 処理ログアップデート
		UPDATE	w2_AccessLogWorkLog
		   SET	target_date = @TARGET_DATE,
				end_date = getdate()
		 WHERE	log_no = @LOG_NO

	END
GO