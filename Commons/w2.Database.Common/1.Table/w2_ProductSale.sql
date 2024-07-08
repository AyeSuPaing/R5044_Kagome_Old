if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductSale]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductSale]
GO
/*
=========================================================================================================
  Module      : 商品セールマスタ(w2_ProductSale.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductSale] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[productsale_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[productsale_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'TS'),
	[productsale_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[closedmarket_password] [nvarchar] (30) COLLATE Japanese_CS_AS_KS_WS NOT NULL DEFAULT (N''),
	[date_bgn] [datetime] NOT NULL,
	[date_end] [datetime] NOT NULL,
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductSale] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductSale] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[productsale_id]
	) ON [PRIMARY]
GO
