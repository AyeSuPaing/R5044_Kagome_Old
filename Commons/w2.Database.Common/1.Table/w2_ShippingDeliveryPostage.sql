if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ShippingDeliveryPostage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ShippingDeliveryPostage]
GO
/*
=========================================================================================================
  Module      : 配送料マスタ(w2_ShippingDeliveryPostage.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ShippingDeliveryPostage] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[shipping_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[delivery_company_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[shipping_price_kbn] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[shipping_free_price_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[shipping_free_price] [decimal] (18,3),
	[announce_free_shipping_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[calculation_plural_kbn] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[plural_shipping_price] [decimal] (18,3),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[storepickup_free_price_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[free_shipping_fee] [decimal] (18,3) NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ShippingDeliveryPostage] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ShippingDeliveryPostage] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[shipping_id],
		[delivery_company_id]
	) ON [PRIMARY]
GO

/*
-- V5.14
ALTER TABLE [w2_ShippingDeliveryPostage] ADD [storepickup_free_price_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_ShippingDeliveryPostage] ADD [free_shipping_fee] [decimal] (18,3) NOT NULL DEFAULT (0);
*/
