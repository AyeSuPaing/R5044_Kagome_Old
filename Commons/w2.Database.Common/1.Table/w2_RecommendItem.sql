if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_RecommendItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_RecommendItem]
GO
/*
=========================================================================================================
  Module      : レコメンドアイテム (w2_RecommendItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_RecommendItem] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[recommend_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[recommend_item_type] [nvarchar] (30) NOT NULL DEFAULT (N'NORMAL'),
	[recommend_item_product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[recommend_item_variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[recommend_item_add_quantity_type] [nvarchar] (30) NOT NULL DEFAULT (N'SPECIFY_QUANTITY'),
	[recommend_item_add_quantity] [int] NOT NULL DEFAULT (1),
	[recommend_item_sort_no] [int] NOT NULL DEFAULT (1),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[fixed_purchase_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_setting1] [nvarhcar] (10) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_RecommendItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_RecommendItem] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[recommend_id],
		[recommend_item_type],
		[recommend_item_product_id],
		[recommend_item_variation_id]
	) ON [PRIMARY]
GO
