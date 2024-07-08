if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_SMSErrorPointGlobalTelNo]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_SMSErrorPointGlobalTelNo]
GO
/*
=========================================================================================================
  Module      : SMSエラーポイント (w2_SMSErrorPointGlobalTelNo.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_SMSErrorPointGlobalTelNo] (
	[global_tel_no] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[error_point] [int] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_SMSErrorPointGlobalTelNo] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_SMSErrorPointGlobalTelNo] PRIMARY KEY  CLUSTERED
	(
		[global_tel_no]
	) ON [PRIMARY]
GO
