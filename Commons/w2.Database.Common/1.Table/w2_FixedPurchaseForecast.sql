if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_FixedPurchaseForecast]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_FixedPurchaseForecast]
GO
/*
=========================================================================================================
  Module      : 定期出荷予測 (w2_FixedPurchaseForecast.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_FixedPurchaseForecast] (
	[target_month] [datetime] NOT NULL,
	[fixed_purchase_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[product_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[item_quantity] [int] NOT NULL DEFAULT (0),
	[delivery_frequency] [int] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[delivery_frequency_by_scheduled_shipping_date] [int] DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FixedPurchaseForecast] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FixedPurchaseForecast] PRIMARY KEY  CLUSTERED
	(
		[target_month],
		[fixed_purchase_id],
		[shop_id],
		[product_id],
		[variation_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_FixedPurchaseForecast_1] ON [dbo].[w2_FixedPurchaseForecast]([target_month], [shop_id], [product_id], [variation_id]) ON [PRIMARY]
GO
