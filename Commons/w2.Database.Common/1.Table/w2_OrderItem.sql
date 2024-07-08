if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_OrderItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_OrderItem]
GO
/*
=========================================================================================================
  Module      : 注文商品情報(w2_OrderItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_OrderItem] (
	[order_id] [nvarchar] (30) NOT NULL,
	[order_item_no] [int] NOT NULL DEFAULT (1),
	[order_shipping_no] [int] NOT NULL DEFAULT (1),
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[supplier_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[product_name] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[product_name_kana] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[product_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[product_price_org] [decimal] (18,3) NOT NULL DEFAULT (0),
	[product_point] [float] NOT NULL DEFAULT (0),
	[product_tax_included_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[product_tax_rate] [decimal] (5,2) NOT NULL DEFAULT (0),
	[product_tax_round_type] [nvarchar] (20) NOT NULL DEFAULT (N'ROUNDDOWN'),
	[product_price_pretax] [decimal] (18,3) NOT NULL DEFAULT (0),
	[product_price_ship] [decimal],
	[product_price_cost] [decimal],
	[product_point_kbn] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[item_realstock_reserved] [int] NOT NULL DEFAULT (0),
	[item_realstock_shipped] [int] NOT NULL DEFAULT (0),
	[item_quantity] [int] NOT NULL DEFAULT (1),
	[item_quantity_single] [int] NOT NULL DEFAULT (1),
	[item_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[item_price_single] [decimal] (18,3) NOT NULL DEFAULT (0),
	[product_set_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[product_set_no] [int],
	[product_set_count] [int],
	[item_return_exchange_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'00'),
	[item_memo] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[item_point] [float] NOT NULL DEFAULT (0),
	[item_cancel_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[item_cancel_date] [datetime],
	[item_return_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[item_return_date] [datetime],
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[product_option_texts] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[brand_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[download_url] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[productsale_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cooperation_id1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cooperation_id2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cooperation_id3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cooperation_id4] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cooperation_id5] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[order_setpromotion_no] [int],
	[order_setpromotion_item_no] [int],
	[stock_returned_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[novelty_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[recommend_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[fixed_purchase_product_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[product_bundle_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[bundle_item_display_type] [nvarchar] (10) NOT NULL DEFAULT (N'VALID'),
	[order_history_display_type] [nvarchar] (AS (CASE WHEN [product_bundle_id] != N'' AND [bundle_item_display_type] = N'INVALID' THEN N'INVALID' ELSE N'VALID' END) PERSISTED NOT NULL),
	[limited_payment_ids] [nvarchar] (250) NOT NULL DEFAULT (N''),
	[plural_shipping_price_free_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[shipping_size_kbn] [nvarchar] (4),
	[column_for_mall_order] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[cooperation_id6] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[cooperation_id7] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[cooperation_id8] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[cooperation_id9] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[cooperation_id10] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[gift_wrapping_id] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[gift_message] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[item_price_tax] [decimal] (18,3) NOT NULL DEFAULT (0),
	[fixed_purchase_discount_value] [decimal] (18,3),
	[fixed_purchase_discount_type] [nvarchar] (10),
	[fixed_purchase_item_order_count] [int],
	[fixed_purchase_item_shipped_count] [int],
	[item_discounted_price] [decimal] (18,3),
	[subscription_box_course_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[subscription_box_fixed_amount] [decimal] (18,3)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_OrderItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_OrderItem] PRIMARY KEY  CLUSTERED
	(
		[order_id],
		[order_item_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_OrderItem_1] ON [dbo].[w2_OrderItem]([order_id], [order_shipping_no]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_OrderItem_2] ON [dbo].[w2_OrderItem]([shop_id], [product_id], [variation_id]) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_OrderItem] ADD [product_option_texts] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderItem] ADD [stock_returned_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderItem] ADD [novelty_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_w2_OrderItem_2')
	CREATE INDEX [IX_w2_OrderItem_2] ON [dbo].[w2_OrderItem]([shop_id], [product_id], [variation_id]) ON [PRIMARY]
ALTER TABLE [w2_OrderItem] ADD [recommend_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderItem] ADD [product_bundle_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderItem] ADD [bundle_item_display_type] [nvarchar] (10) NOT NULL DEFAULT (N'VALID');
ALTER TABLE [w2_OrderItem] ADD [order_history_display_type] AS (
	CASE WHEN [product_bundle_id] != N'' AND [bundle_item_display_type] = N'INVALID' THEN N'INVALID' ELSE N'VALID' END) PERSISTED NOT NULL;
ALTER TABLE [w2_OrderItem] ADD [column_for_mall_order] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderItem] ADD [cooperation_id6] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderItem] ADD [cooperation_id7] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderItem] ADD [cooperation_id8] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderItem] ADD [cooperation_id9] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderItem] ADD [cooperation_id10] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderItem] ADD [gift_wrapping_id] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderItem] ADD [gift_message] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderItem] ADD [fixed_purchase_discount_value] [decimal] (18,3);
ALTER TABLE [w2_OrderItem] ADD [fixed_purchase_discount_type] [nvarchar] (10);
-- 注文商品情報テーブル 定期商品購入回数(注文時点)
IF NOT EXISTS (SELECT column_name FROM INFORMATION_SCHEMA.columns WHERE table_name = 'w2_OrderItem' AND column_name = 'fixed_purchase_item_order_count')
ALTER TABLE [w2_OrderItem] ADD [fixed_purchase_item_order_count] [int];
-- 注文商品情報テーブル 定期商品購入回数(出荷時点)
IF NOT EXISTS (SELECT column_name FROM INFORMATION_SCHEMA.columns WHERE table_name = 'w2_OrderItem' AND column_name = 'fixed_purchase_item_shipped_count')
ALTER TABLE [w2_OrderItem] ADD [fixed_purchase_item_shipped_count] [int];
ALTER TABLE [w2_OrderItem] ADD [item_discounted_price] [decimal] (18,3);
*/

/*
■ 決済通貨対応
ALTER TABLE [w2_OrderItem] ALTER COLUMN [product_price] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_OrderItem] ALTER COLUMN [product_price_org] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_OrderItem] ALTER COLUMN [product_price_pretax] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_OrderItem] ALTER COLUMN [item_price] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_OrderItem] ALTER COLUMN [item_price_single] [decimal] (18,3) NOT NULL;
*/