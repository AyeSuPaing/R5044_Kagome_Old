if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_TaskScheduleLog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_TaskScheduleLog]
GO
/*
=========================================================================================================
  Module      : タスクスケジュールログマスタ (w2_TaskScheduleLog.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_TaskScheduleLog] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_master_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_no] [int] NOT NULL DEFAULT (1),
	[messaging_app_kbn] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[result] [nvarchar] (1000) NOT NULL DEFAULT (N''),
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_TaskScheduleLog] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_TaskScheduleLog] PRIMARY KEY  NONCLUSTERED
	(
		[dept_id],
		[action_kbn],
		[action_master_id],
		[action_no],
		[messaging_app_kbn]
	) ON [PRIMARY]
GO
