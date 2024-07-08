if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_FeatureImageGroup]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_FeatureImageGroup]
GO
/*
=========================================================================================================
  Module      : 特集画像グループマスタ (w2_FeatureImageGroup.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_FeatureImageGroup] (
	[feature_image_group_id] [bigint] IDENTITY(1, 1) NOT NULL,
	[group_name] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[group_sort_number] [int] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FeatureImageGroup] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FeatureImageGroup] PRIMARY KEY  CLUSTERED
	(
		[feature_image_group_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_FeatureImageGroup_1] ON [dbo].[w2_FeatureImageGroup]([group_name]) ON [PRIMARY]
GO
