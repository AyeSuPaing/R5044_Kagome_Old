if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_SubscriptionBoxItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_SubscriptionBoxItem]
GO
/*
=========================================================================================================
  Module      : 頒布会選択可能商品 (w2_SubscriptionBoxItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_SubscriptionBoxItem] (
	[subscription_box_course_id] [nvarchar] (30) NOT NULL,
	[branch_no] [int] NOT NULL,
	[shop_id] [nvarchar] (10) NOT NULL,
	[product_id] [nvarchar] (30) NOT NULL,
	[variation_id] [nvarchar] (60) NOT NULL,
	[selectable_since] [datetime],
	[selectable_until] [datetime]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_SubscriptionBoxItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_SubscriptionBoxItem] PRIMARY KEY  CLUSTERED
	(
		[subscription_box_course_id],
		[branch_no]
	) ON [PRIMARY]
GO
