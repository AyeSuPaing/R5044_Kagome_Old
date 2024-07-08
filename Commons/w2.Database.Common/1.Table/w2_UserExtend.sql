if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserExtend]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserExtend]
GO
/*
=========================================================================================================
  Module      : ユーザ拡張項目テーブル(w2_UserExtend.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserExtend] (
	[user_id] [nvarchar] (30) NOT NULL,
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserExtend] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserExtend] PRIMARY KEY  CLUSTERED
	(
		[user_id]
	) ON [PRIMARY]
GO
