if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ShopShippingZone]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ShopShippingZone]
GO
/*
=========================================================================================================
  Module      : 店舗配送料地帯マスタ(w2_ShopShippingZone.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ShopShippingZone] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[shipping_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[shipping_zone_no] [int] NOT NULL DEFAULT (N'0'),
	[shipping_zone_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[zip] [ntext] NOT NULL DEFAULT (N''),
	[size_xxs_shipping_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[size_xs_shipping_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[size_s_shipping_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[size_m_shipping_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[size_l_shipping_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[size_xl_shipping_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[size_xxl_shipping_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[size_mail_shipping_price] [decimal] NOT NULL DEFAULT (0),
	[delivery_company_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[conditional_shipping_price_threshold] [decimal] (18,3),
	[conditional_shipping_price] [decimal] (18,3),
	[unavailable_shipping_area_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ShopShippingZone] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ShopShippingZone] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[shipping_id],
		[delivery_company_id],
		[shipping_zone_no]
	) ON [PRIMARY]
GO

/*
■ 決済通貨対応
ALTER TABLE [w2_ShopShippingZone] ALTER COLUMN [size_xxs_shipping_price] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_ShopShippingZone] ALTER COLUMN [size_xs_shipping_price] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_ShopShippingZone] ALTER COLUMN [size_s_shipping_price] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_ShopShippingZone] ALTER COLUMN [size_m_shipping_price] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_ShopShippingZone] ALTER COLUMN [size_l_shipping_price] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_ShopShippingZone] ALTER COLUMN [size_xl_shipping_price] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_ShopShippingZone] ALTER COLUMN [size_xxl_shipping_price] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_ShopShippingZone] ALTER COLUMN [size_mail_shipping_price] [decimal] (18,3) NOT NULL;

■配送サービスごとの配送料設定
ALTER TABLE [w2_ShopShippingZone] ADD [delivery_company_id] [nvarchar] (10) NOT NULL DEFAULT (N'');
*/