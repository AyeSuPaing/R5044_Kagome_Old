if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_FeatureImage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_FeatureImage]
GO
/*
=========================================================================================================
  Module      : 特集画像マスタ (w2_FeatureImage.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_FeatureImage] (
	[image_id] [bigint] IDENTITY(1, 1) NOT NULL,
	[file_name] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[file_dir_path] [nvarchar] (300) NOT NULL DEFAULT (N''),
	[feature_image_group_id] [bigint] NOT NULL DEFAULT (0),
	[image_sort_number] [int] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FeatureImage] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FeatureImage] PRIMARY KEY  CLUSTERED
	(
		[image_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_FeatureImage_1] ON [dbo].[w2_FeatureImage]([file_name]) ON [PRIMARY]
GO
