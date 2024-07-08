/*
=========================================================================================================
  Module      : 特集ページ情報 (w2_FeaturePage.sql)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[w2_FeaturePage]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_FeaturePage]
GO

CREATE TABLE [dbo].[w2_FeaturePage] (
	[feature_page_id] [bigint] IDENTITY (1, 1) NOT NULL,
	[management_title] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[page_type] [nvarchar] (20) NOT NULL DEFAULT (N'GROUP'),
	[category_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[permitted_brand_ids] [nvarchar] (300) NOT NULL DEFAULT (N''),
	[file_name] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[file_dir_path] [nvarchar] (300) NOT NULL DEFAULT (N''),
	[page_sort_number] [int] NOT NULL DEFAULT (0),
	[use_type] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[publish] [nvarchar] (20) NOT NULL DEFAULT (N'PUBLIC'),
	[html_page_title] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[metadata_desc] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[condition_publish_date_from] [datetime],
	[condition_publish_date_to] [datetime],
	[condition_member_only_type] [nvarchar] (20) NOT NULL DEFAULT (N'ALL'),
	[condition_member_rank_id] [nvarchar] (30),
	[condition_target_list_type] [nvarchar] (20) NOT NULL DEFAULT (N'OR'),
	[condition_target_list_ids] [nvarchar] (MAX),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FeaturePage] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FeaturePage] PRIMARY KEY CLUSTERED
	(
		[feature_page_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_FeaturePage_1] ON [dbo].[w2_FeaturePage]([page_type]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_FeaturePage_2] ON [dbo].[w2_FeaturePage]([file_name]) ON [PRIMARY]
GO