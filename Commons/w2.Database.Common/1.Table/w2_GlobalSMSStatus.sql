if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_GlobalSMSStatus]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_GlobalSMSStatus]
GO
/*
=========================================================================================================
  Module      : SMSステータス (w2_GlobalSMSStatus.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_GlobalSMSStatus] (
	[message_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[global_tel_no] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[sms_status] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_GlobalSMSStatus] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_GlobalSMSStatus] PRIMARY KEY  CLUSTERED
	(
		[message_id]
	) ON [PRIMARY]
GO
