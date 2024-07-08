/*
=========================================================================================================
  Module      : 特集ページカテゴリ (w2_FeaturePageCategory.sql)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[w2_FeaturePageCategory]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_FeaturePageCategory]
GO

CREATE TABLE [dbo].[w2_FeaturePageCategory] (
	[shop_id] [nvarchar](10) NOT NULL DEFAULT (N''),
	[category_id] [nvarchar](30) NOT NULL DEFAULT (N''),
	[parent_category_id] [nvarchar](30) NOT NULL DEFAULT (N''),
	[category_name] [nvarchar](40) NOT NULL DEFAULT (N''),
	[category_outline] [nvarchar](200) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (1),
	[valid_flg] [nvarchar](1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar](20) NOT NULL DEFAULT (N''),
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FeaturePageCategory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FeaturePageCategory] PRIMARY KEY CLUSTERED
	(
		[shop_id],
		[category_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_FeaturePageCategory_1] ON [dbo].[w2_FeaturePageCategory]([shop_id], [parent_category_id]) ON [PRIMARY]
GO