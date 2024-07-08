if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_SubscriptionBox]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_SubscriptionBox]
GO
/*
=========================================================================================================
  Module      : 頒布会コース (w2_SubscriptionBox.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_SubscriptionBox] (
	[subscription_box_course_id] [nvarchar] (30) NOT NULL,
	[management_name] [nvarchar] (100) NOT NULL,
	[display_name] [nvarchar] (100) NOT NULL,
	[auto_renewal] [nvarchar] (5) NOT NULL,
	[items_changeable_by_user] [nvarchar] (5) NOT NULL,
	[order_item_determination_type] [nvarchar] (10) NOT NULL,
	[minimum_purchase_quantity] [int],
	[maximum_purchase_quantity] [int],
	[minimum_number_of_products] [int],
	[maximum_number_of_products] [int],
	[valid_flg] [nvarchar] (1),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[fixed_amount_flag] [nvarchar] (1),
	[fixed_amount] [decimal] (18,3),
	[tax_category_id] [nvarchar] (30),
	[display_priority] [int] NOT NULL DEFAULT (0),
	[first_selectable_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[indefinite_period_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_SubscriptionBox] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_SubscriptionBox] PRIMARY KEY  CLUSTERED
	(
		[subscription_box_course_id]
	) ON [PRIMARY]
GO
