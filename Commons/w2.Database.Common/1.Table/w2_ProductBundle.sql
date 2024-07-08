if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductBundle]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductBundle]
GO
/*
=========================================================================================================
  Module      : 商品同梱テーブル (w2_ProductBundle.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductBundle] (
	[product_bundle_id] [nvarchar] (30) NOT NULL,
	[product_bundle_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[target_order_type] [nvarchar] (20) NOT NULL DEFAULT (N'ALL'),
	[description] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[start_datetime] [datetime] NOT NULL,
	[end_datetime] [datetime],
	[target_product_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'SELECT'),
	[target_product_ids] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[target_order_fixed_purchase_count_from] [int] NOT NULL DEFAULT (0),
	[target_order_fixed_purchase_count_to] [int] NOT NULL DEFAULT (0),
	[usable_times_kbn] [nvarchar] (20) NOT NULL DEFAULT (N'NO_LIMIT'),
	[apply_type] [nvarchar] (20) NOT NULL DEFAULT (N'FOR_ORDER'),
	[valid_flg] [nvarchar] (10) NOT NULL DEFAULT (N'VALID'),
	[multiple_apply_flg] [nvarchar] (10) NOT NULL DEFAULT (N'INVALID'),
	[apply_order] [int] NOT NULL DEFAULT (100),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[usable_times] [int],
	[target_product_category_ids] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[except_product_ids] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[except_product_category_ids] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[target_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[target_id_except_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[target_order_price_subtotal_min] [decimal] (18,3),
	[target_product_count_min] [int],
	[target_advcodes_first] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[target_advcodes_new] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[target_coupon_codes] [nvarchar] (max) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductBundle] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductBundle] PRIMARY KEY  CLUSTERED
	(
		[product_bundle_id]
	) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_ProductBundle] ALTER COLUMN [usable_times_kbn] [nvarchar] (20);
ALTER TABLE [w2_ProductBundle] ALTER COLUMN [target_order_fixed_purchase_count_from] [int];
ALTER TABLE [w2_ProductBundle] ALTER COLUMN [target_order_fixed_purchase_count_to] [int];
ALTER TABLE [w2_ProductBundle] ADD [usable_times] [int];
ALTER TABLE [w2_ProductBundle] ADD [target_product_category_ids] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_ProductBundle] ADD [except_product_ids] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_ProductBundle] ADD [except_product_category_ids] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_ProductBundle] ADD [target_id] [nvarchar] (10) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_ProductBundle] ADD [target_id_except_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_ProductBundle] ADD [target_order_price_subtotal_min] [decimal] (18,3);
ALTER TABLE [w2_ProductBundle] ADD [target_product_count_min] [int];
ALTER TABLE [w2_ProductBundle] ADD [target_advcodes_first] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_ProductBundle] ADD [target_advcodes_new] [nvarchar] (max) NOT NULL DEFAULT (N'');
*/
