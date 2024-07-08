if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_WorkRealShopProductStock]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_WorkRealShopProductStock]
GO
/*
=========================================================================================================
  Module      : リアル店舗商品在庫情報ワークテーブル(w2_WorkRealShopProductStock.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_WorkRealShopProductStock] (
	[real_shop_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[real_shop_stock] [int] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkRealShopProductStock] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkRealShopProductStock] PRIMARY KEY  CLUSTERED
	(
		[real_shop_id],
		[product_id],
		[variation_id]
	) ON [PRIMARY]
GO
