/*
=========================================================================================================
  Module      : 特集ページコンテンツ (w2_FeaturePageContents.sql)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[w2_FeaturePageContents]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_FeaturePageContents]
GO

CREATE TABLE [dbo].[w2_FeaturePageContents] (
	[feature_page_id] [bigint] NOT NULL,
	[contents_kbn] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[contents_type] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[contents_sort_number] [int] NOT NULL DEFAULT (0),
	[page_title] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[alt_text] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[product_group_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[product_list_title] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[display_number] [int] NOT NULL DEFAULT (0),
	[pagination_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FeaturePageContents] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FeaturePageContents] PRIMARY KEY CLUSTERED
	(
		[feature_page_id],
		[contents_kbn],
		[contents_type],
		[contents_sort_number]
	) ON [PRIMARY]
GO