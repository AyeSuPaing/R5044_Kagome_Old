if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductSalePrice]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductSalePrice]
GO
/*
=========================================================================================================
  Module      : 商品セール価格マスタ(w2_ProductSalePrice.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductSalePrice] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[productsale_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[sale_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[display_order] [int] NOT NULL DEFAULT (1),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductSalePrice] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductSalePrice] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[productsale_id],
		[product_id],
		[variation_id]
	) ON [PRIMARY]
GO

/*
■ 決済通貨対応
ALTER TABLE [w2_ProductSalePrice] ALTER COLUMN [sale_price] [decimal] (18,3) NOT NULL;
*/