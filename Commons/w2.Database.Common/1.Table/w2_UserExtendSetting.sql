if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserExtendSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserExtendSetting]
GO
/*
=========================================================================================================
  Module      : ユーザ拡張項目設定テーブル(w2_UserExtendSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserExtendSetting] (
	[setting_id] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[setting_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[outline_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'TEXT'),
	[outline] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[sort_order] [int] NOT NULL DEFAULT (1),
	[input_type] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[input_default] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[init_only_flg] [nvarchar] (30) NOT NULL DEFAULT (N'UPDATABLE'),
	[display_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserExtendSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserExtendSetting] PRIMARY KEY  CLUSTERED
	(
		[setting_id]
	) ON [PRIMARY]
GO
