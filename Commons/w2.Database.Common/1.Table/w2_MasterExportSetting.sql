if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MasterExportSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MasterExportSetting]
GO
/*
=========================================================================================================
  Module      : マスタ出力定義マスタ(w2_MasterExportSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MasterExportSetting] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[master_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[setting_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[setting_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[fields] [ntext] NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[export_file_type] [nvarchar] (10) NOT NULL DEFAULT (N'CSV')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MasterExportSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MasterExportSetting] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[master_kbn],
		[setting_id]
	) ON [PRIMARY]
GO
