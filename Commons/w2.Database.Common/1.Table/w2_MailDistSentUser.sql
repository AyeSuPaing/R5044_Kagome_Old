if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MailDistSentUser]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MailDistSentUser]
GO
/*
=========================================================================================================
  Module      : メール配信送信済ユーザ (w2_MailDistSentUser.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MailDistSentUser] (
	[maildist_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MailDistSentUser] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MailDistSentUser] PRIMARY KEY  CLUSTERED
	(
		[maildist_id],
		[user_id]
	) ON [PRIMARY]
GO