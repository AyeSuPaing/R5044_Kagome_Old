if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MailTemplate]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MailTemplate]
GO
/*
=========================================================================================================
  Module      : メールテンプレートマスタ(w2_MailTemplate.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MailTemplate] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mail_id] [nvarchar] (10) NOT NULL,
	[mail_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[mail_from] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[mail_to] [nvarchar] (1000) NOT NULL DEFAULT (N''),
	[mail_cc] [nvarchar] (1000) NOT NULL DEFAULT (N''),
	[mail_bcc] [nvarchar] (1000) NOT NULL DEFAULT (N''),
	[mail_subject] [nvarchar] (500) NOT NULL DEFAULT (N''),
	[mail_body] [ntext] NOT NULL DEFAULT (N''),
	[mail_subject_mobile] [nvarchar] (500) NOT NULL DEFAULT (N''),
	[mail_body_mobile] [ntext] NOT NULL DEFAULT (N''),
	[mail_attachmentfile_path] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[mail_from_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[mail_category] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[auto_send_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[language_code] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[language_locale_id] [nvarchar] (7) NOT NULL DEFAULT (N''),
	[sms_use_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[line_use_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[mailtext_html] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[sendhtml_flg] [nvarchar] (10) NOT NULL DEFAULT (N'0')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MailTemplate] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MailTemplate] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[mail_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_MailTemplate_1] ON [dbo].[w2_MailTemplate]([mail_category]) ON [PRIMARY]
GO

/*
IF NOT EXISTS (SELECT column_name FROM INFORMATION_SCHEMA.columns WHERE table_name = 'w2_MailTemplate' AND column_name = 'mail_from_name')
	ALTER TABLE [w2_MailTemplate] ADD [mail_from_name] [nvarchar] (50) NOT NULL DEFAULT (N'');
*/

/*
ALTER TABLE [w2_MailTemplate] ADD [sms_use_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0')
ALTER TABLE [w2_MailTemplate] ADD [line_use_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0')
*/

/*
ALTER TABLE [w2_MailTemplate] ADD [mailtext_html] [nvarchar] (MAX) NOT NULL DEFAULT (N'')
ALTER TABLE [w2_MailTemplate] ADD [sendhtml_flg] [nvarchar] (10) NOT NULL DEFAULT (N'0')
*/
