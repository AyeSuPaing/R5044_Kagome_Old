/*
=========================================================================================================
  Module      : モバイルIP設定マスタ(w2_MobileIPSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MobileIPSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MobileIPSetting]
GO

CREATE TABLE [dbo].[w2_MobileIPSetting] (
	[ip_setting] [nvarchar] (30) NOT NULL,
	[date_changed] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MobileIPSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MobileIPSetting] PRIMARY KEY  CLUSTERED
	(
		[ip_setting]
	) ON [PRIMARY]
GO
