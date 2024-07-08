if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MailClick]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MailClick]
GO
/*
=========================================================================================================
  Module      : メールクリックマスタ(w2_MailClick.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MailClick] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mailtext_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[maildist_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_no] [int] NOT NULL DEFAULT (0),
	[mailclick_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mailclick_url] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[mailclick_key] [nvarchar] (20) COLLATE Japanese_CS_AS_KS_WS,
	[pcmobile_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MailClick] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MailClick] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[mailtext_id],
		[maildist_id],
		[action_no],
		[mailclick_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_MailClick_1] ON [dbo].[w2_MailClick]([mailclick_key]) ON [PRIMARY]
GO
