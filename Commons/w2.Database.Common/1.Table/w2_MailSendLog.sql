if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MailSendLog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MailSendLog]
GO
/*
=========================================================================================================
  Module      : メール送信ログ (w2_MailSendLog.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MailSendLog] (
	[log_no] [bigint] IDENTITY (1, 1),
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mailtext_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mailtext_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[maildist_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[maildist_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mail_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mail_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[mail_from_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[mail_from] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[mail_to] [nvarchar] (1000) NOT NULL DEFAULT (N''),
	[mail_cc] [nvarchar] (1000) NOT NULL DEFAULT (N''),
	[mail_bcc] [nvarchar] (1000) NOT NULL DEFAULT (N''),
	[mail_subject] [nvarchar] (500) NOT NULL DEFAULT (N''),
	[mail_body] [ntext] NOT NULL DEFAULT (N''),
	[mail_body_html] [ntext] NOT NULL DEFAULT (N''),
	[error_message] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[date_send_mail] [datetime] NOT NULL,
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[read_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[date_read_mail] [datetime],
	[text_history_no] [bigint] NOT NULL DEFAULT (0),
	[mail_addr_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MailSendLog] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MailSendLog] PRIMARY KEY  CLUSTERED
	(
		[log_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_MailSendLog_1] ON [dbo].[w2_MailSendLog]([user_id]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_MailSendLog_2] ON [dbo].[w2_MailSendLog]([date_send_mail]) ON [PRIMARY]
GO