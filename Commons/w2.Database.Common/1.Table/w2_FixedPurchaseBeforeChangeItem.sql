if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_FixedPurchaseBeforeChangeItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_FixedPurchaseBeforeChangeItem]
GO
/*
=========================================================================================================
  Module      : 定期変更元商品(w2_FixedPurchaseBeforeChangeItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_FixedPurchaseBeforeChangeItem] (
	[fixed_purchase_product_change_id] [nvarchar](50) NOT NULL DEFAULT (N''),
	[item_unit_type] [nvarchar](30) NOT NULL DEFAULT (N'PRODUCT'),
	[shop_id] [nvarchar](10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar](30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar](60) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar](20) NOT NULL DEFAULT (N'')
	) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FixedPurchaseBeforeChangeItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FixedPurchaseBeforeChangeItem] PRIMARY KEY  CLUSTERED
	(
		[fixed_purchase_product_change_id],
		[shop_id],
		[product_id],
		[variation_id]
	) ON [PRIMARY]
GO