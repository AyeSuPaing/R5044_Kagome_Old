if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_FeatureAreaType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_FeatureAreaType]
GO
/*
=========================================================================================================
  Module      : 特集エリアタイプマスタ (w2_FeatureAreaType.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_FeatureAreaType] (
	[area_type_id] [nvarchar] (30) NOT NULL,
	[area_type_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_type] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[internal_memo] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[pc_start_tag] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[pc_repeat_tag] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[pc_end_tag] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[pc_script_tag] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[sp_start_tag] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[sp_repeat_tag] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[sp_end_tag] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[sp_script_tag] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FeatureAreaType] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FeatureAreaType] PRIMARY KEY  CLUSTERED
	(
		[area_type_id]
	) ON [PRIMARY]
GO
