if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductStockHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductStockHistory]
GO
/*
=========================================================================================================
  Module      : 商品在庫履歴マスタ(w2_ProductStockHistory.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductStockHistory] (
	[history_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[action_status] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[add_stock] [int] NOT NULL DEFAULT (0),
	[add_realstock] [int] NOT NULL DEFAULT (0),
	[add_realstock_b] [int] NOT NULL DEFAULT (0),
	[add_realstock_c] [int] NOT NULL DEFAULT (0),
	[add_realstock_reserved] [int] NOT NULL DEFAULT (0),
	[update_stock] [int],
	[update_realstock] [int],
	[update_realstock_b] [int],
	[update_realstock_c] [int],
	[update_realstock_reserved] [int],
	[update_memo] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[sync_flg] [bit] NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductStockHistory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductStockHistory] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[product_id],
		[variation_id],
		[history_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_ProductStockHistory_1] ON [dbo].[w2_ProductStockHistory]([order_id]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_ProductStockHistory_2] ON [dbo].[w2_ProductStockHistory]([date_created]) ON [PRIMARY]
GO
