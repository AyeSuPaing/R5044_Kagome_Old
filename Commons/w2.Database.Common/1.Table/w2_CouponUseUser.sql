if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CouponUseUser]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CouponUseUser]
GO
/*
=========================================================================================================
  Module      : クーポン利用ユーザーテーブル (w2_CouponUseUser.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CouponUseUser] (
	[coupon_id] [nvarchar] (30) NOT NULL,
	[coupon_use_user] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[fixed_purchase_id] [nvarchar] (30) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CouponUseUser] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CouponUseUser] PRIMARY KEY  CLUSTERED
	(
		[coupon_id],
		[coupon_use_user]
	) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_CouponUseUser] ADD [fixed_purchase_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
*/
