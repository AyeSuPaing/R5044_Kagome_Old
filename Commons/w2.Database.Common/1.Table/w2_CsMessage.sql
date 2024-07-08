if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsMessage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsMessage]
GO
/*
=========================================================================================================
  Module      : メッセージマスタ(w2_CsMessage.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsMessage] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[incident_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[message_no] [int] NOT NULL DEFAULT (1),
	[media_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'TEL'),
	[direction_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'REC'),
	[operator_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[inquiry_reply_date] [datetime],
	[message_status] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[user_name1] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[user_name2] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[user_name_kana1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[user_name_kana2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[user_mail_addr] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[user_tel1] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[inquiry_title] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[inquiry_text] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[reply_text] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[reply_operator_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[receive_mail_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[send_mail_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[valid_flg] [nvarchar] (10) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsMessage] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsMessage] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[incident_id],
		[message_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_CsMessage_1] ON [dbo].[w2_CsMessage]([dept_id], [operator_id], [message_status], [valid_flg]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_CsMessage_2] ON [dbo].[w2_CsMessage]([dept_id], [message_status]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_CsMessage_3] ON [dbo].[w2_CsMessage]([dept_id], [reply_operator_id]) ON [PRIMARY]
GO
