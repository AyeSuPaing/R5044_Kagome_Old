if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductBundleItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductBundleItem]
GO
/*
=========================================================================================================
  Module      : 商品同梱 同梱商品テーブル (w2_ProductBundleItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductBundleItem] (
	[product_bundle_id] [nvarchar] (30) NOT NULL,
	[product_bundle_item_no] [int] NOT NULL DEFAULT (1),
	[grant_product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[grant_product_variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[grant_product_count] [int] NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[ordered_product_except_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductBundleItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductBundleItem] PRIMARY KEY  CLUSTERED
	(
		[product_bundle_id],
		[product_bundle_item_no]
	) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_ProductBundleItem] ADD [ordered_product_except_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
*/