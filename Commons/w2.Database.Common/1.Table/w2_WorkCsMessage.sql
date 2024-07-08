/*
=========================================================================================================
  Module      : Cs message work table(w2_WorkCsMessage.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[w2_WorkCsMessage]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_WorkCsMessage]
GO

CREATE TABLE [dbo].[w2_WorkCsMessage] (
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
	[inquiry_text] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[reply_text] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[reply_operator_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[receive_mail_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[send_mail_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[valid_flg] [nvarchar] (10) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkCsMessage] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkCsMessage] PRIMARY KEY CLUSTERED
	(
		[dept_id],
		[incident_id],
		[message_no]
	) ON [PRIMARY]
GO
