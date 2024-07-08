if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_TaskScheduleHistorySummary]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_TaskScheduleHistorySummary]
GO
/*
=========================================================================================================
  Module      : タスクスケジュール履歴集計テーブル (w2_TaskScheduleHistorySummary.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_TaskScheduleHistorySummary] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_master_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_no] [int] NOT NULL DEFAULT (1),
	[action_result] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[action_kbn_detail] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[target_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[history_count] [int] NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_TaskScheduleHistorySummary] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_TaskScheduleHistorySummary] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[action_kbn],
		[action_master_id],
		[action_no],
		[action_result],
		[action_kbn_detail],
		[target_id]
	) ON [PRIMARY]
GO
