if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_TempTargetUser]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_TempTargetUser]
GO
/*
=========================================================================================================
  Module      : ターゲットユーザリストの一時保持テーブル (w2_TempTargetUser.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_TempTargetUser] (
	[user_id] [nvarchar] (30) NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_TempTargetUser] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_TempTargetUser] PRIMARY KEY  CLUSTERED
	(
		[user_id]
	) ON [PRIMARY]
GO
