if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ShopShipping]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ShopShipping]
GO
/*
=========================================================================================================
  Module      : 店舗配送種別マスタ(w2_ShopShipping.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ShopShipping] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[shipping_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[shop_shipping_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[payment_relation_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_price_kbn] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[shipping_date_set_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[shipping_date_set_begin] [int],
	[shipping_date_set_end] [int],
	[shipping_time_set_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[shipping_time_id1] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id2] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id3] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id4] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message4] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id5] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message5] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id6] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message6] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id7] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message7] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id8] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message8] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id9] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message9] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id10] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message10] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_free_price_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[shipping_free_price] [decimal] (18,3),
	[announce_free_shipping_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[calculation_plural_kbn] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[plural_shipping_price] [decimal] (18,3),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[fixed_purchase_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[fixed_purchase_kbn1_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[fixed_purchase_kbn1_setting] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[fixed_purchase_kbn2_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[fixed_purchase_kbn2_setting] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[fixed_purchase_kbn3_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[fixed_purchase_kbn3_setting] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[fixed_purchase_order_entry_timing] [int] NOT NULL DEFAULT (0),
	[fixed_purchase_cancel_deadline] [int] NOT NULL DEFAULT (0),
	[wrapping_paper_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[wrapping_paper_types] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[wrapping_bag_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[wrapping_bag_types] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[payment_selection_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[permitted_payment_ids] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[shipping_price_separate_estimates_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[shipping_price_separate_estimates_message] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_price_separate_estimates_message_mobile] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[fixed_purchase_shipping_days_required] [int] NOT NULL DEFAULT (0),
	[fixed_purchase_minimum_shipping_span] [int] NOT NULL DEFAULT (1),
	[fixed_purchase_free_shipping_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[business_days_for_shipping] [int] NOT NULL DEFAULT (0),
	[next_shipping_max_change_days] [int] NOT NULL DEFAULT (0),
	[fixed_purchase_shipping_notdisplay_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[fixed_purchase_kbn4_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[fixed_purchase_kbn4_setting1] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[fixed_purchase_kbn4_setting2] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[fixed_purchase_kbn1_setting2] [nvarchar] (150) NOT NULL DEFAULT (N''),
	[fixed_purchase_first_shipping_next_month_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[shipping_base_id] [nvarchar] (2) NOT NULL DEFAULT (N'01')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ShopShipping] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ShopShipping] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[shipping_id]
	) ON [PRIMARY]
GO

/*
ALTER TABLE dbo.w2_ShopShipping ADD [business_days_for_shipping] [int] NOT NULL DEFAULT (0);
ALTER TABLE w2_ShopShipping ADD [next_shipping_max_change_days] [int] NOT NULL DEFAULT (15);
ALTER TABLE w2_ShopShipping ADD [fixed_purchase_first_shipping_next_month_flg] [nvarchar] (1) NOT NULL DEFAULT (0)
*/

/*
■ 決済通貨対応
ALTER TABLE [w2_ShopShipping] ALTER COLUMN [shipping_free_price] [decimal] (18,3);
ALTER TABLE [w2_ShopShipping] ALTER COLUMN [plural_shipping_price] [decimal] (18,3);
*/

ALTER TABLE w2_ShopShipping ADD fixed_purchase_kbn1_setting2 nvarchar(150) NOT NULL DEFAULT N''

/*
IF NOT EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'w2_ShopShipping' AND COLUMN_NAME = 'shipping_base_id')
	ALTER TABLE [w2_ShopShipping] ADD [shipping_base_id] [nvarchar] (2) NOT NULL DEFAULT (N'01');
GO
*/
