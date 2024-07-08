if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MobileGroup]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MobileGroup]
GO
/*
=========================================================================================================
  Module      : モバイル機種グループマスタ(w2_MobileGroup.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MobileGroup] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[career_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mobile_group_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mobile_group_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[model_no] [int] NOT NULL DEFAULT (1),
	[model_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MobileGroup] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MobileGroup] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[career_id],
		[mobile_group_id],
		[model_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_MobileGroup_1] ON [dbo].[w2_MobileGroup]([dept_id], [career_id], [model_name]) ON [PRIMARY]
GO
