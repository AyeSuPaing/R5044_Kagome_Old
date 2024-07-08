if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserCouponHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserCouponHistory]
GO
/*
=========================================================================================================
  Module      : ユーザクーポン履歴マスタ(w2_UserCoupon.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserCouponHistory] (
	[user_id] [nvarchar] (30) NOT NULL,
	[history_no] [int] NOT NULL DEFAULT (1),
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_code] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[history_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[action_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[coupon_inc] [decimal] NOT NULL DEFAULT (0),
	[coupon_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[memo] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[fixed_purchase_id] [nvarchar] (30) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserCouponHistory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserCouponHistory] PRIMARY KEY  CLUSTERED
	(
		[user_id],
		[history_no]
	) ON [PRIMARY]
GO

/*
■ 決済通貨対応
ALTER TABLE [w2_UserCouponHistory] ALTER COLUMN [coupon_price] [decimal] (18,3) NOT NULL;
■定期の次回購入の利用クーポン対応
ALTER TABLE [w2_UserCouponHistory] ADD [fixed_purchase_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
*/