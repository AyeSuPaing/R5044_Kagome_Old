if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_PointRuleSchedule]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_PointRuleSchedule]
GO
/*
=========================================================================================================
  Module      : ポイントルールスケジュールテーブル (w2_PointRuleSchedule.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_PointRuleSchedule] (
	[point_rule_schedule_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[point_rule_schedule_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[status] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[last_count] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[last_exec_date] [datetime],
	[target_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[target_extract_flg] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[point_rule_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mail_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[exec_timing] [nvarchar] (10) NOT NULL DEFAULT (N'01'),
	[schedule_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[schedule_day_of_week] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[schedule_year] [int],
	[schedule_month] [int],
	[schedule_day] [int],
	[schedule_hour] [int],
	[schedule_minute] [int],
	[schedule_second] [int],
	[valid_flg] [nvarchar] (10) NOT NULL DEFAULT (N'ON'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_PointRuleSchedule] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_PointRuleSchedule] PRIMARY KEY  CLUSTERED
	(
		[point_rule_schedule_id]
	) ON [PRIMARY]
GO
