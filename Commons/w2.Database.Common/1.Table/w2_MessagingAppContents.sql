if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MessagingAppContents]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MessagingAppContents]
GO
/*
=========================================================================================================
  Module      : メッセージアプリ向けコンテンツ (w2_MessagingAppContents.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MessagingAppContents] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[master_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[master_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[messaging_app_kbn] [nvarchar] (20) NOT NULL DEFAULT (N'0'),
	[branch_no] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[media_type] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[contents] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MessagingAppContents] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MessagingAppContents] PRIMARY KEY  NONCLUSTERED
	(
		[dept_id],
		[master_kbn],
		[master_id],
		[messaging_app_kbn],
		[branch_no]
	) ON [PRIMARY]
GO