if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_SubscriptionBoxDefaultItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_SubscriptionBoxDefaultItem]
GO
/*
=========================================================================================================
  Module      : 頒布会デフォルト注文商品 (w2_SubscriptionBoxDefaultItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_SubscriptionBoxDefaultItem] (
	[subscription_box_course_id] [nvarchar] (30) NOT NULL,
	[branch_no] [int] NOT NULL,
	[count] [int],
	[term_since] [datetime],
	[term_until] [datetime],
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[product_id] [nvarchar] (30) NOT NULL,
	[variation_id] [nvarchar] (60) NOT NULL,
	[item_quantity] [int] NOT NULL,
	[necessary_product_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_SubscriptionBoxDefaultItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_SubscriptionBoxDefaultItem] PRIMARY KEY  CLUSTERED
	(
		[subscription_box_course_id],
		[branch_no]
	) ON [PRIMARY]
GO

--ALTER TABLE [dbo].[w2_SubscriptionBoxDefaultItem] ADD [necessary_product_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
