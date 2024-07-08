if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MallExhibitsConfig]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MallExhibitsConfig]
GO
/*
=========================================================================================================
  Module      : モール出品設定マスタ(w2_MallExhibitsConfig.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MallExhibitsConfig] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[exhibits_flg1] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg2] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg3] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg4] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg5] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg6] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg7] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg8] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg9] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg10] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg11] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg12] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg13] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg14] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg15] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg16] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg17] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg18] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg19] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[exhibits_flg20] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MallExhibitsConfig] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MallExhibitsConfig] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[product_id]
	) ON [PRIMARY]
GO
