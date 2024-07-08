if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_PageDesign]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_PageDesign]
GO
/*
=========================================================================================================
  Module      : ページデザイン ページ管理 (w2_PageDesign.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_PageDesign] (
	[page_id] [bigint] IDENTITY (1, 1) NOT NULL,
	[management_title] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[page_type] [nvarchar] (20) NOT NULL DEFAULT (N'NOMAL'),
	[file_name] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[pc_file_dir_path] [nvarchar] (300) NOT NULL DEFAULT (N''),
	[group_id] [bigint] NOT NULL DEFAULT (0),
	[page_sort_number] [int] NOT NULL DEFAULT (0),
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
	[metadata_desc] [nvarchar] (200) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_PageDesign] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_PageDesign] PRIMARY KEY  CLUSTERED
	(
		[page_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_PageDesign_2] ON [dbo].[w2_PageDesign]([file_name], [pc_file_dir_path]) ON [PRIMARY]
GO
