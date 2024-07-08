if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_OrderCoupon]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_OrderCoupon]
GO
/*
=========================================================================================================
  Module      : ТНХ╢ГNБ[Г|ГУПюХё(w2_UserCoupon.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_OrderCoupon] (
	[order_id] [nvarchar] (30) NOT NULL,
	[order_coupon_no] [int] NOT NULL DEFAULT (1),
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_no] [int] NOT NULL DEFAULT (1),
	[coupon_code] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_disp_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_type] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[coupon_discount_price] [decimal] (18,3),
	[coupon_discount_rate] [decimal],
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_OrderCoupon] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_OrderCoupon] PRIMARY KEY  CLUSTERED
	(
		[order_id],
		[order_coupon_no]
	) ON [PRIMARY]
GO

/*
Бб МИН╧Т╩Й▌С╬ЙЮ
ALTER TABLE [w2_OrderCoupon] ALTER COLUMN [coupon_discount_price] [decimal] (18,3);
*/