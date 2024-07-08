/*
=========================================================================================================
  Module      : Fixed purchase item work table(w2_WorkFixedPurchaseItem.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[w2_WorkFixedPurchaseItem]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_WorkFixedPurchaseItem]
GO

CREATE TABLE [dbo].[w2_WorkFixedPurchaseItem] (
	[fixed_purchase_id] [nvarchar] (30) NOT NULL,
	[fixed_purchase_item_no] [int] NOT NULL DEFAULT (1),
	[fixed_purchase_shipping_no] [int] NOT NULL DEFAULT (1),
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[supplier_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[item_quantity] [int] NOT NULL DEFAULT (1),
	[item_quantity_single] [int] NOT NULL DEFAULT (1),
	[product_set_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[product_set_no] [int],
	[product_set_count] [int],
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[product_option_texts] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[item_order_count] [int] NOT NULL DEFAULT (0),
	[item_shipped_count] [int] NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkFixedPurchaseItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkFixedPurchaseItem] PRIMARY KEY CLUSTERED
	(
		[fixed_purchase_id],
		[fixed_purchase_item_no]
	) ON [PRIMARY]
GO
