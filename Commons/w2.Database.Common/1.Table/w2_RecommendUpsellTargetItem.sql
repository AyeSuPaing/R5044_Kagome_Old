if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_RecommendUpsellTargetItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_RecommendUpsellTargetItem]
GO
/*
=========================================================================================================
  Module      : レコメンドアップセル対象アイテム (w2_RecommendUpsellTargetItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_RecommendUpsellTargetItem] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[recommend_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[recommend_upsell_target_item_type] [nvarchar] (30) NOT NULL DEFAULT (N'NORMAL'),
	[recommend_upsell_target_item_product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[recommend_upsell_target_item_variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_RecommendUpsellTargetItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_RecommendUpsellTargetItem] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[recommend_id],
		[recommend_upsell_target_item_type],
		[recommend_upsell_target_item_product_id],
		[recommend_upsell_target_item_variation_id]
	) ON [PRIMARY]
GO
