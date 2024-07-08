if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserIntegrationUser]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserIntegrationUser]
GO
/*
=========================================================================================================
  Module      : ユーザー統合ユーザー情報 (w2_UserIntegrationUser.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserIntegrationUser] (
	[user_integration_no] [bigint] NOT NULL,
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[representative_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserIntegrationUser] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserIntegrationUser] PRIMARY KEY  CLUSTERED
	(
		[user_integration_no],
		[user_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_UserIntegrationUser_1] ON [dbo].[w2_UserIntegrationUser]([user_id]) ON [PRIMARY]
GO