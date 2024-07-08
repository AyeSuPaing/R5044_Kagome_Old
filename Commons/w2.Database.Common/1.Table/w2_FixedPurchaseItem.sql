if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_FixedPurchaseItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_FixedPurchaseItem]
GO
/*
=========================================================================================================
  Module      : 定期購入商品情報(w2_FixedPurchaseItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_FixedPurchaseItem] (
	[fixed_purchase_id] [nvarchar] (30) NOT NULL,
	[fixed_purchase_item_no] [int] NOT NULL DEFAULT (1),
	[fixed_purchase_shipping_no] [int] NOT NULL DEFAULT (1),
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[supplier_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[item_quantity] [int] NOT NULL DEFAULT (1),
	[item_quantity_single] [int] NOT NULL DEFAULT (1),
	[item_price] [decimal] NOT NULL DEFAULT (0),
	[item_price_single] [decimal] NOT NULL DEFAULT (0),
	[product_set_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[product_set_no] [int],
	[product_set_count] [int],
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[product_option_texts] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[item_order_count] [int] NOT NULL DEFAULT (0),
	[item_shipped_count] [int] NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FixedPurchaseItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FixedPurchaseItem] PRIMARY KEY  CLUSTERED
	(
		[fixed_purchase_id],
		[fixed_purchase_item_no]
	) ON [PRIMARY]
GO

/*
-- 定期購入商品情報テーブル 商品購入回数（注文基準）
IF NOT EXISTS (SELECT column_name FROM INFORMATION_SCHEMA.columns WHERE table_name = 'w2_FixedPurchaseItem' AND column_name = 'item_order_count')
ALTER TABLE [w2_FixedPurchaseItem] ADD [item_order_count] [int] NOT NULL DEFAULT (0);
-- 定期購入商品情報テーブル 商品購入回数（出荷基準）
IF NOT EXISTS (SELECT column_name FROM INFORMATION_SCHEMA.columns WHERE table_name = 'w2_FixedPurchaseItem' AND column_name = 'item_shipped_count')
ALTER TABLE [w2_FixedPurchaseItem] ADD [item_shipped_count] [int] NOT NULL DEFAULT (0);
*/
