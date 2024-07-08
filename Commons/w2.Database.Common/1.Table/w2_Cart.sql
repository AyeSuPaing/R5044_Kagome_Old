if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_Cart]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_Cart]
GO
/*
=========================================================================================================
  Module      : ФГВвХиВйВ▓Г}ГXГ^(w2_Cart.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_Cart] (
	[cart_id] [nvarchar] (40) NOT NULL,
	[cart_item_no] [int] NOT NULL DEFAULT (1),
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (2),
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[supplier_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[product_count] [int] NOT NULL DEFAULT (1),
	[product_set_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[product_set_no] [int],
	[product_set_count] [int] NOT NULL DEFAULT (1),
	[cart_div_type1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cart_div_type2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cart_div_type3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[kbn1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[kbn2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[kbn3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[productsale_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[mobile_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[product_option_texts] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[gift_order_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[novelty_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[recommend_id] [nvarchar] (30) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_Cart] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_Cart] PRIMARY KEY  CLUSTERED
	(
		[cart_id],
		[cart_item_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_Cart_1] ON [dbo].[w2_Cart]([user_id]) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_Cart] ADD [product_option_texts] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Cart] ADD [gift_order_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Cart] ADD [novelty_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Cart] ADD [recommend_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
*/
