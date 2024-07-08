if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductListDispSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductListDispSetting]
GO
/*
=========================================================================================================
  Module      : 商品一覧表示設定マスタ([w2_ProductListDispSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductListDispSetting] (
	[setting_id] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[setting_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[disp_enable] [nvarchar] (10) NOT NULL DEFAULT (N'ON'),
	[disp_no] [int] NOT NULL,
	[default_disp_flg] [nvarchar] (10) NOT NULL DEFAULT (N'OFF'),
	[description] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[setting_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'SORT'),
	[disp_count] [int]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductListDispSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductListDispSetting] PRIMARY KEY  CLUSTERED
	(
		[setting_id], [setting_kbn]
	) ON [PRIMARY]
GO
