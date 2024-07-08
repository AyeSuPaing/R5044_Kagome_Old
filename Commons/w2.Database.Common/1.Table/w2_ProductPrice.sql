if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductPrice]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductPrice]
GO
/*
=========================================================================================================
  Module      : 商品価格マスタ(w2_ProductPrice.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductPrice] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[member_rank_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[member_rank_price] [decimal] (18,3) NOT NULL,
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductPrice] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductPrice] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[product_id],
		[variation_id],
		[member_rank_id]
	) ON [PRIMARY]
GO

/*
■ 決済通貨対応
ALTER TABLE [w2_ProductPrice] ALTER COLUMN [member_rank_price] [decimal] (18,3) NOT NULL;
*/