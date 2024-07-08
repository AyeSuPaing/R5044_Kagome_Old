if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AssignDeptIdToAccessLogSp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[w2_AssignDeptIdToAccessLogSp]
GO
/*
=========================================================================================================
  Module      : 識別IDアクセスログ発行プロシージャ(w2_AssignDeptIdToAccessLogSp.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE PROCEDURE [dbo].[w2_AssignDeptIdToAccessLogSp] AS

	-- 対象日取得
	DECLARE @TARGET_DATE varchar(10)
	SELECT	TOP 1 @TARGET_DATE = target_date
	  FROM	w2_AccessLogStatus
	
	-- アップデート
	UPDATE	w2_AccessLog
	   SET	dept_id = ISNULL((
				SELECT	dept_id
				  FROM	w2_AccessLogAccount
				 WHERE	w2_AccessLog.account_id = w2_AccessLogAccount.account_id
			),'')
	 WHERE	w2_AccessLog.access_date <= @TARGET_DATE
	   AND	w2_AccessLog.dept_id = ''

GO