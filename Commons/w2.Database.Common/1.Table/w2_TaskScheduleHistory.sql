if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_TaskScheduleHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_TaskScheduleHistory]
GO
/*
=========================================================================================================
  Module      : タスクスケジュール履歴マスタ(w2_TaskScheduleHistory.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_TaskScheduleHistory] (
	[history_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[schedule_date] [datetime] NOT NULL DEFAULT (GETDATE()),
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_master_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_no] [int] NOT NULL DEFAULT (1),
	[action_step] [int] NOT NULL DEFAULT (0),
	[action_kbn_detail] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_result] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[target_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mail_addr] [nvarchar] (256) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_TaskScheduleHistory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_TaskScheduleHistory] PRIMARY KEY  NONCLUSTERED
	(
		[history_no]
	) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_TaskScheduleHistory_1] ON [dbo].[w2_TaskScheduleHistory]([dept_id], [action_kbn], [action_master_id], [action_no], [action_step]) ON [PRIMARY]
GO
