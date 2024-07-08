if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_RecommendApplyConditionItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_RecommendApplyConditionItem]
GO
/*
=========================================================================================================
  Module      : レコメンド適用条件アイテム (w2_RecommendApplyConditionItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_RecommendApplyConditionItem] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[recommend_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[recommend_apply_condition_type] [nvarchar] (30) NOT NULL DEFAULT (N'BUY'),
	[recommend_apply_condition_item_type] [nvarchar] (30) NOT NULL DEFAULT (N'NORMAL'),
	[recommend_apply_condition_item_unit_type] [nvarchar] (30) NOT NULL DEFAULT (N'PRODUCT'),
	[recommend_apply_condition_item_product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[recommend_apply_condition_item_variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[recommend_apply_condition_item_sort_no] [int] NOT NULL DEFAULT (1),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_RecommendApplyConditionItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_RecommendApplyConditionItem] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[recommend_id],
		[recommend_apply_condition_type],
		[recommend_apply_condition_item_type],
		[recommend_apply_condition_item_unit_type],
		[recommend_apply_condition_item_product_id],
		[recommend_apply_condition_item_variation_id]
	) ON [PRIMARY]
GO
