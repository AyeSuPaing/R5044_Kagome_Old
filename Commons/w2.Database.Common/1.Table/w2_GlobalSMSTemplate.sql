if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_GlobalSMSTemplate]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_GlobalSMSTemplate]
GO
/*
=========================================================================================================
  Module      : SMSテンプレート (w2_GlobalSMSTemplate.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_GlobalSMSTemplate] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mail_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[phone_carrier] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[sms_text] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_GlobalSMSTemplate] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_GlobalSMSTemplate] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[mail_id],
		[phone_carrier]
	) ON [PRIMARY]
GO
