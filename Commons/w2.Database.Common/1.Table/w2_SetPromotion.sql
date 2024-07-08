if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_SetPromotion]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_SetPromotion]
GO
/*
=========================================================================================================
  Module      : セットプロモーションマスタ(w2_SetPromotion.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_SetPromotion] (
	[setpromotion_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[setpromotion_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[setpromotion_disp_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[setpromotion_disp_name_mobile] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[product_discount_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[shipping_charge_free_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[payment_charge_free_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[product_discount_kbn] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[product_discount_setting] [decimal] (18,3) NULL,
	[description_kbn] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[description] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[description_kbn_mobile] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[description_mobile] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[begin_date] [datetime] NOT NULL DEFAULT (getdate()),
	[end_date] [datetime] NULL,
	[target_member_rank] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[target_order_kbn] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[url] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[url_mobile] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (1),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[apply_order] [int] NOT NULL DEFAULT (1),
	[w2_SetPromotion] [nvarchar] (MAX) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_SetPromotion] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_SetPromotion] PRIMARY KEY  CLUSTERED
	(
		[setpromotion_id]
	) ON [PRIMARY]
GO

/*
■ 決済通貨対応
ALTER TABLE [w2_SetPromotion] ALTER COLUMN [product_discount_setting] [decimal] (18,3);
*/

/*
■ ターゲットリスト
ALTER TABLE [w2_SetPromotion] ALTER COLUMN [target_id] [nvarchar] (MAX) NOT NULL DEFAULT (N'')
*/
