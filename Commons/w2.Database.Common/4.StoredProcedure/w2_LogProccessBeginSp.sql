if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_LogProccessBeginSp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[w2_LogProccessBeginSp]
GO
/*
=========================================================================================================
  Module      : ログ加工開始プロシージャ(w2_LogProccessBeginSp.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE PROCEDURE [dbo].[w2_LogProccessBeginSp] @TARGET_DATE varchar(10) OUTPUT AS

	-- 初期化
	SET @TARGET_DATE = ''

	-- 加工可能日次取得
	SELECT	@TARGET_DATE = target_date
	  FROM	w2_AccessLogStatus
	 WHERE	day_status = '10'

	-- 取得できなければ例外
	IF @TARGET_DATE = ''
	BEGIN
		RAISERROR ('加工可能なログはありません',16,1)
	END
	
	--処理開始ステータスへ変更
	UPDATE	w2_AccessLogStatus
	   SET	day_status = '11'
	 WHERE	target_date = @TARGET_DATE

GO