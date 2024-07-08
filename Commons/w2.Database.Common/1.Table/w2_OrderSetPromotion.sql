if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_OrderSetPromotion]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_OrderSetPromotion]
GO
/*
=========================================================================================================
  Module      : 注文セットプロモーション情報(w2_OrderSetPromotion.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_OrderSetPromotion] (
	[order_id] [nvarchar] (30) NOT NULL,
	[order_setpromotion_no] [int] NOT NULL DEFAULT (1),
	[setpromotion_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[setpromotion_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[setpromotion_disp_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[undiscounted_product_subtotal] [decimal] NOT NULL DEFAULT (0),
	[product_discount_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[product_discount_amount] [decimal] NOT NULL DEFAULT (0),
	[shipping_charge_free_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[shipping_charge_discount_amount] [decimal] NOT NULL DEFAULT (0),
	[payment_charge_free_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[payment_charge_discount_amount] [decimal] NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_OrderSetPromotion] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_OrderSetPromotion] PRIMARY KEY  CLUSTERED
	(
		[order_id],
		[order_setpromotion_no]
	) ON [PRIMARY]
GO

/*
■ 決済通貨対応
ALTER TABLE [w2_OrderSetPromotion] ALTER COLUMN [undiscounted_product_subtotal] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_OrderSetPromotion] ALTER COLUMN [product_discount_amount] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_OrderSetPromotion] ALTER COLUMN [shipping_charge_discount_amount] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_OrderSetPromotion] ALTER COLUMN [payment_charge_discount_amount] [decimal] (18,3) NOT NULL;
*/