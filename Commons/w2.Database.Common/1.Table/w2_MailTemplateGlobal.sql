if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MailTemplateGlobal]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MailTemplateGlobal]
GO
/*
=========================================================================================================
  Module      : メールテンプレートグローバルマスタ (w2_MailTemplateGlobal.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MailTemplateGlobal] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mail_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[language_code] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[language_locale_id] [nvarchar] (7) NOT NULL DEFAULT (N''),
	[mail_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[mail_from] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[mail_to] [nvarchar] (1000) NOT NULL DEFAULT (N''),
	[mail_cc] [nvarchar] (1000) NOT NULL DEFAULT (N''),
	[mail_bcc] [nvarchar] (1000) NOT NULL DEFAULT (N''),
	[mail_subject] [nvarchar] (500) NOT NULL DEFAULT (N''),
	[mail_body] [ntext] NOT NULL DEFAULT (N''),
	[mail_attachmentfile_path] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[mail_from_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[mail_category] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[auto_send_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[sms_use_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[mailtext_html] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[sendhtml_flg] [nvarchar] (10) NOT NULL DEFAULT (N'0')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MailTemplateGlobal] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MailTemplateGlobal] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[mail_id],
		[language_code],
		[language_locale_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_MailTemplateGlobal_1] ON [dbo].[w2_MailTemplateGlobal]([mail_category]) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_MailTemplateGlobal] ADD [sms_use_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0')
*/

/*
ALTER TABLE [w2_MailTemplateGlobal] ADD [mailtext_html] [nvarchar] (MAX) NOT NULL DEFAULT (N'')
ALTER TABLE [w2_MailTemplateGlobal] ADD [sendhtml_flg] [nvarchar] (10) NOT NULL DEFAULT (N'0')
*/


