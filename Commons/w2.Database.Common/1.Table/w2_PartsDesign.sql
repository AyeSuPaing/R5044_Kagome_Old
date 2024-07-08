if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_PartsDesign]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_PartsDesign]
GO
/*
=========================================================================================================
  Module      : パーツデザイン パーツ管理 (w2_PartsDesign.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_PartsDesign] (
	[parts_id] [bigint] IDENTITY (1, 1) NOT NULL,
	[management_title] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[parts_type] [nvarchar] (20) NOT NULL DEFAULT (N'NOMAL'),
	[file_name] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[pc_file_dir_path] [nvarchar] (300) NOT NULL DEFAULT (N''),
	[group_id] [bigint] NOT NULL DEFAULT (0),
	[parts_sort_number] [int] NOT NULL DEFAULT (0),
	[use_type] [nvarchar] (20) NOT NULL DEFAULT (N'PC_SP'),
	[publish] [nvarchar] (20) NOT NULL DEFAULT (N'PUBLIC'),
	[condition_publish_date_from] [datetime],
	[condition_publish_date_to] [datetime],
	[condition_member_only_type] [nvarchar] (20) NOT NULL DEFAULT (N'ALL'),
	[condition_member_rank_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[condition_target_list_type] [nvarchar] (20) NOT NULL DEFAULT (N'OR'),
	[condition_target_list_ids] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[area_id] [nvarchar] (30) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_PartsDesign] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_PartsDesign] PRIMARY KEY  CLUSTERED
	(
		[parts_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_PartsDesign_1] ON [dbo].[w2_PartsDesign]([area_id]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_PartsDesign_2] ON [dbo].[w2_PartsDesign]([file_name], [pc_file_dir_path]) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_PartsDesign] ADD [area_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
*/