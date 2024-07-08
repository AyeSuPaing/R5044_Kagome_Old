if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MailClickLog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MailClickLog]
GO
/*
=========================================================================================================
  Module      : メールクリックログ(w2_MailClickLog.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MailClickLog] (
	[log_no] [bigint] IDENTITY (1, 1),
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mailtext_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[maildist_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_no] [int] NOT NULL DEFAULT (1),
	[mailclick_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MailClickLog] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MailClickLog] PRIMARY KEY  NONCLUSTERED
	(
		[log_no]
	) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_MailClickLog_1] ON [dbo].[w2_MailClickLog]([dept_id], [mailtext_id], [maildist_id], [action_no], [mailclick_id], [date_created]) ON [PRIMARY]
GO
