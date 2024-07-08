if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_FeatureAreaBanner]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_FeatureAreaBanner]
GO
/*
=========================================================================================================
  Module      : 特集エリアバナー (w2_FeatureAreaBanner.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_FeatureAreaBanner] (
	[area_id] [nvarchar] (30) NOT NULL,
	[banner_no] [int] NOT NULL,
	[file_name] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[file_dir_path] [nvarchar] (300) NOT NULL DEFAULT (N''),
	[alt_text] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[text] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[link_url] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[window_type] [nvarchar] (20) NOT NULL DEFAULT (N'NONPOPUP'),
	[publish] [nvarchar] (20) NOT NULL DEFAULT (N'PUBLIC'),
	[condition_publish_date_from] [datetime],
	[condition_publish_date_to] [datetime],
	[condition_member_only_type] [nvarchar] (20) NOT NULL DEFAULT (N'ALL'),
	[condition_member_rank_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[condition_target_list_type] [nvarchar] (20) NOT NULL DEFAULT (N'OR'),
	[condition_target_list_ids] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FeatureAreaBanner] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FeatureAreaBanner] PRIMARY KEY  CLUSTERED
	(
		[area_id],
		[banner_no]
	) ON [PRIMARY]
GO
