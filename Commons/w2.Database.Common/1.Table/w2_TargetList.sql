if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_TargetList]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_TargetList]
GO
/*
=========================================================================================================
  Module      : ターゲットリスト設定マスタ(w2_TargetList.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_TargetList] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[target_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[target_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[status] [nvarchar] (10) NOT NULL DEFAULT (N'00'),
	[target_type] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[target_condition] [ntext] NOT NULL DEFAULT (N''),
	[data_count] [int],
	[data_count_date] [datetime],
	[exec_timing] [nvarchar] (10) NOT NULL DEFAULT (N'01'),
	[schedule_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[schedule_day_of_week] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[schedule_year] [int],
	[schedule_month] [int],
	[schedule_day] [int],
	[schedule_hour] [int],
	[schedule_minute] [int],
	[schedule_second] [int],
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_TargetList] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_TargetList] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[target_id]
	) ON [PRIMARY]
GO
