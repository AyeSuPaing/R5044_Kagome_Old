if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_WorkProductStock]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_WorkProductStock]
GO
/*
=========================================================================================================
  Module      : 商品在庫マスタ用ワークテーブル(w2_WorkProductStock)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_WorkProductStock] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[stock] [int] NOT NULL DEFAULT (0),
	[stock_alert] [int] NOT NULL DEFAULT (0),
	[realstock] [int] NOT NULL DEFAULT (0),
	[realstock_b] [int] NOT NULL DEFAULT (0),
	[realstock_c] [int] NOT NULL DEFAULT (0),
	[realstock_reserved] [int] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkProductStock] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkProductStock] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[product_id],
		[variation_id]
	) ON [PRIMARY]
GO
