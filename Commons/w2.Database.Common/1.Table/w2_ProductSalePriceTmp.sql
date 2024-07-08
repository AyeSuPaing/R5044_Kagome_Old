if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductSalePriceTmp]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductSalePriceTmp]
GO
/*
=========================================================================================================
  Module      : 商品セール価格テンポラリテーブル (w2_ProductSalePriceTmp.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductSalePriceTmp] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[productsale_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[sale_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[display_order] [int] NOT NULL DEFAULT (1),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductSalePriceTmp] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductSalePriceTmp] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[productsale_id],
		[product_id]
	) ON [PRIMARY]
GO
