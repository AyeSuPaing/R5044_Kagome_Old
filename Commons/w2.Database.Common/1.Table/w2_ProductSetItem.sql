if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductSetItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductSetItem]
GO
/*
=========================================================================================================
  Module      : 商品セットアイテムマスタ(w2_ProductSetItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductSetItem] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_set_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[setitem_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[count_min] [int],
	[count_max] [int],
	[family_flg] [nvarchar] (10) NOT NULL DEFAULT (N'1'),
	[display_order] [int] NOT NULL DEFAULT (1),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductSetItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductSetItem] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[product_set_id],
		[product_id],
		[variation_id]
	) ON [PRIMARY]
GO

/*
■ 決済通貨対応
ALTER TABLE [w2_ProductSetItem] ALTER COLUMN [setitem_price] [decimal] (18,3) NOT NULL;
*/