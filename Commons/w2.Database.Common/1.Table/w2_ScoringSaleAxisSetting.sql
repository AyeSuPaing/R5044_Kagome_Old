/*
=========================================================================================================
  Module      : Scoring Sale Axis Setting (w2_ScoringSaleAxisSetting.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[w2_ScoringSaleAxisSetting]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_ScoringSaleAxisSetting]
GO

CREATE TABLE [dbo].[w2_ScoringSaleAxisSetting] (
	[score_axis_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[score_axis_title] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[axis_name1] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[axis_name2] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[axis_name3] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[axis_name4] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[axis_name5] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[axis_name6] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[axis_name7] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[axis_name8] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[axis_name9] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[axis_name10] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[axis_name11] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[axis_name12] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[axis_name13] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[axis_name14] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[axis_name15] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ScoringSaleAxisSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ScoringSaleAxisSetting] PRIMARY KEY CLUSTERED
	(
		[score_axis_id]
	) ON [PRIMARY]
GO
