if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_StockOrderItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_StockOrderItem]
GO
/*
=========================================================================================================
  Module      : 発注商品情報マスタ(w2_StockOrderItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_StockOrderItem] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[stock_order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[order_count] [int] NOT NULL DEFAULT (0),
	[delivery_count] [int] NOT NULL DEFAULT (0),
	[order_status] [nvarchar] (10) NOT NULL DEFAULT (N'00'),
	[delivery_status] [nvarchar] (10) NOT NULL DEFAULT (N'00'),
	[order_date] [datetime],
	[order_input_date] [datetime] NOT NULL DEFAULT (getdate()),
	[delivery_reply_date] [datetime],
	[delivery_sche_date] [datetime],
	[delivery_date] [datetime],
	[delivery_input_date] [datetime],
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_StockOrderItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_StockOrderItem] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[stock_order_id],
		[product_id],
		[variation_id]
	) ON [PRIMARY]
GO
