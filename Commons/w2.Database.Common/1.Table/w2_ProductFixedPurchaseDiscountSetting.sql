if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductFixedPurchaseDiscountSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductFixedPurchaseDiscountSetting]
GO
/*
=========================================================================================================
  Module      : 商品定期購入割引設定 (w2_ProductFixedPurchaseDiscountSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductFixedPurchaseDiscountSetting] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[order_count] [int] NOT NULL,
	[order_count_more_than_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[product_count] [int] NOT NULL,
	[discount_value] [decimal] (18,3),
	[discount_type] [nvarchar] (10),
	[point_value] [decimal],
	[point_type] [nvarchar] (10),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductFixedPurchaseDiscountSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductFixedPurchaseDiscountSetting] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[product_id],
		[order_count],
		[product_count]
	) ON [PRIMARY]
GO

/*
■ 決済通貨対応
ALTER TABLE [w2_ProductFixedPurchaseDiscountSetting] ALTER COLUMN [discount_value] [decimal] (18,3);
*/