if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_WorkCoordinateCategory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_WorkCoordinateCategory]
GO
/*
=========================================================================================================
  Module      : ワークコーディネートカテゴリ (w2_WorkCoordinateCategory.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_WorkCoordinateCategory] (
	[coordinate_category_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coordinate_parent_category_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[coordinate_category_name] [nvarchar] (40) NOT NULL DEFAULT (N''),
	[seo_keywords] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (0),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkCoordinateCategory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkCoordinateCategory] PRIMARY KEY  CLUSTERED
	(
		[coordinate_category_id]
	) ON [PRIMARY]
GO