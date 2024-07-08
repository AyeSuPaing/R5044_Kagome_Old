if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductDefaultSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductDefaultSetting]
GO
/*
=========================================================================================================
  Module      : 商品初期設定マスタ(w2_ProductDefaultSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductDefaultSetting] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[init_data] [ntext] NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductDefaultSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductDefaultSetting] PRIMARY KEY  CLUSTERED
	(
		[shop_id]
	) ON [PRIMARY]
GO
