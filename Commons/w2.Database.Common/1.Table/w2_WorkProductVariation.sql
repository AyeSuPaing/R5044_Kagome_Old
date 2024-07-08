if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_WorkProductVariation]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_WorkProductVariation]
GO
/*
=========================================================================================================
  Module      : 商品バリエーションマスタ用ワークテーブル (w2_WorkProductVariation.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_WorkProductVariation] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[variation_cooperation_id1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_cooperation_id2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_cooperation_id3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_cooperation_id4] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_cooperation_id5] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mall_variation_id1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mall_variation_id2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mall_variation_type] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[variation_name1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_name2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_name3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[special_price] [decimal] (18,3),
	[variation_image_head] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[variation_image_mobile] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_type] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[shipping_size_kbn] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (1),
	[variation_mall_cooperated_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[variation_download_url] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[variation_fixed_purchase_firsttime_price] [decimal] (18,3),
	[variation_fixed_purchase_price] [decimal] (18,3),
	[variation_cooperation_id6] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[variation_cooperation_id7] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[variation_cooperation_id8] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[variation_cooperation_id9] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[variation_cooperation_id10] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[variation_andmall_reservation_flg] [nvarchar] (3) NOT NULL DEFAULT (N'001'),
	[variation_color_id] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[variation_weight_gram] [int] NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkProductVariation] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkProductVariation] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[product_id],
		[variation_id]
	) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_WorkProductVariation] ADD [variation_andmall_reservation_flg] [nvarchar] (3) NOT NULL DEFAULT (N'001');
ALTER TABLE [w2_WorkProductVariation] ADD [variation_weight_gram] [int] NOT NULL DEFAULT (0);
*/

/*
■ 決済通貨対応
ALTER TABLE [w2_WorkProductVariation] ALTER COLUMN [price] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_WorkProductVariation] ALTER COLUMN [special_price] [decimal] (18,3);
ALTER TABLE [w2_WorkProductVariation] ALTER COLUMN [variation_fixed_purchase_firsttime_price] [decimal] (18,3);
ALTER TABLE [w2_WorkProductVariation] ALTER COLUMN [variation_fixed_purchase_price] [decimal] (18,3);
*/