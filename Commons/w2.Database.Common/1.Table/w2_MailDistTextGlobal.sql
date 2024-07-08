if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MailDistTextGlobal]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MailDistTextGlobal]
GO
/*
=========================================================================================================
  Module      : メール配信文章グローバルマスタ (w2_MailDistTextGlobal.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MailDistTextGlobal] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mailtext_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[language_code] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[language_locale_id] [nvarchar] (7) NOT NULL DEFAULT (N''),
	[mailtext_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mailtext_subject] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[mailtext_body] [ntext] NOT NULL DEFAULT (N''),
	[mailtext_html] [ntext] NOT NULL DEFAULT (N''),
	[sendhtml_flg] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[mail_from_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[mail_from] [nvarchar] (500) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[sms_use_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MailDistTextGlobal] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MailDistTextGlobal] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[mailtext_id],
		[language_code],
		[language_locale_id]
	) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_MailDistTextGlobal] ADD [sms_use_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0')
*/
