if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserCoupon]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserCoupon]
GO
/*
=========================================================================================================
  Module      : ユーザクーポンマスタ (w2_UserCoupon.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserCoupon] (
	[user_id] [nvarchar] (30) NOT NULL,
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_no] [int] NOT NULL DEFAULT (1),
	[order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[use_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[user_coupon_count] [int]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserCoupon] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserCoupon] PRIMARY KEY  CLUSTERED
	(
		[user_id],
		[dept_id],
		[coupon_id],
		[coupon_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_UserCoupon_1] ON [dbo].[w2_UserCoupon]([use_flg]) ON [PRIMARY]
GO
