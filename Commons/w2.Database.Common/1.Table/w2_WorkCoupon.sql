if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_WorkCoupon]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_WorkCoupon]
GO
/*
=========================================================================================================
  Module      : クーポンワークテーブル (w2_WorkCoupon.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_WorkCoupon] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_code] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_disp_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_disp_name_mobile] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_discription] [ntext] NOT NULL DEFAULT (N''),
	[coupon_discription_mobile] [ntext] NOT NULL DEFAULT (N''),
	[coupon_type] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[coupon_count] [int] NOT NULL DEFAULT (0),
	[publish_date_bgn] [datetime] NOT NULL,
	[publish_date_end] [datetime] NOT NULL,
	[discount_price] [decimal] (18,3),
	[discount_rate] [decimal],
	[expire_day] [int],
	[expire_date_bgn] [datetime],
	[expire_date_end] [datetime],
	[product_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'01'),
	[exceptional_product] [ntext] NOT NULL DEFAULT (N''),
	[exceptional_icon] [int] NOT NULL DEFAULT (0),
	[usable_price] [decimal] (18,3),
	[use_together_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[disp_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[coupon_disp_discription] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[free_shipping_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[exceptional_brand_ids] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[exceptional_product_category_ids] [nvarchar] (max) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkCoupon] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkCoupon] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[coupon_id]
	) ON [PRIMARY]
GO

/*
■ 決済通貨対応
ALTER TABLE [w2_WorkCoupon] ALTER COLUMN [discount_price] [decimal] (18,3);
ALTER TABLE [w2_WorkCoupon] ALTER COLUMN [usable_price] [decimal] (18,3);
*/

/*
■クーポン配送料無料フラグ
ALTER TABLE [w2_WorkCoupon] ADD [free_shipping_flg] NVARCHAR(1) NOT NULL DEFAULT N'0';
*/

/*
■ ブランド・カテゴリ毎にクーポン設定
ALTER TABLE [w2_WorkCoupon] ADD [exceptional_brand_ids] NVARCHAR(MAX) NOT NULL DEFAULT N'';
ALTER TABLE [w2_WorkCoupon] ADD [exceptional_product_category_ids] NVARCHAR(MAX) NOT NULL DEFAULT N'';
*/