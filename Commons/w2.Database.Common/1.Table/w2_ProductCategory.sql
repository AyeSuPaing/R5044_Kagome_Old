if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductCategory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductCategory]
GO
/*
=========================================================================================================
  Module      : 商品カテゴリマスタ(w2_ProductCategory.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductCategory] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[category_id] [nvarchar] (30) NOT NULL,
	[parent_category_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[name] [nvarchar] (40) NOT NULL DEFAULT (N''),
	[name_mobile] [nvarchar] (40) NOT NULL DEFAULT (N''),
	[seo_keywords] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[url] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[mobile_disp_flg] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[permitted_brand_ids] [nvarchar] (300) NOT NULL DEFAULT (N''),
	[use_recommend_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[member_rank_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[lower_member_can_display_tree_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[name_kana] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[display_order] INT NOT NULL DEFAULT (1),
	[child_category_sort_kbn] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[only_fixed_purchase_member_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[canonical_text] [nvarchar] (100) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductCategory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductCategory] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[category_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_ProductCategory_1] ON [dbo].[w2_ProductCategory]([shop_id], [parent_category_id]) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_ProductCategory] ADD [only_fixed_purchase_member_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
*/

/*
■ カノニカルタグ用テキスト対応
ALTER TABLE [w2_ProductCategory] ADD [canonical_text] [nvarchar] (100) NOT NULL DEFAULT (N'');
*/
