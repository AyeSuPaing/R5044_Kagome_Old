if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_TaskSchedule]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_TaskSchedule]
GO
/*
=========================================================================================================
  Module      : タスクスケジュールマスタ(w2_TaskSchedule.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_TaskSchedule] (
	[schedule_date] [datetime] NOT NULL,
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_master_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_no] [int] NOT NULL DEFAULT (1),
	[prepare_status] [nvarchar] (10) NOT NULL DEFAULT (N'00'),
	[execute_status] [nvarchar] (10) NOT NULL DEFAULT (N'00'),
	[progress] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stop_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_begin] [datetime],
	[date_end] [datetime],
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_TaskSchedule] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_TaskSchedule] PRIMARY KEY  NONCLUSTERED
	(
		[dept_id],
		[action_kbn],
		[action_master_id],
		[action_no]
	) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_TaskSchedule_1] ON [dbo].[w2_TaskSchedule]([schedule_date]) ON [PRIMARY]
GO
