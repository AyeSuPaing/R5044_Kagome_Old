﻿if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_WorkUserCoupon]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_WorkUserCoupon]
GO
/*
=========================================================================================================
  Module      : ユーザクーポンワークテーブル (w2_WorkUserCoupon.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_WorkUserCoupon] (
	[user_id] [nvarchar] (30) NOT NULL,
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coupon_no] [int] NOT NULL DEFAULT (1),
	[order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[use_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkUserCoupon] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkUserCoupon] PRIMARY KEY  CLUSTERED
	(
		[user_id],
		[dept_id],
		[coupon_id],
		[coupon_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_WorkUserCoupon_1] ON [dbo].[w2_WorkUserCoupon]([use_flg]) ON [PRIMARY]
GO
