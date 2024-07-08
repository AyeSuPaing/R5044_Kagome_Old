if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MailDistSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MailDistSetting]
GO
/*
=========================================================================================================
  Module      : メール配信設定マスタ (w2_MailDistSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MailDistSetting] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[maildist_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[maildist_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[status] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[last_count] [bigint] NOT NULL DEFAULT (0),
	[last_errorexcept_count] [bigint] NOT NULL DEFAULT (0),
	[last_mobilemailexcept_count] [bigint] NOT NULL DEFAULT (0),
	[last_dist_date] [datetime],
	[target_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[target_extract_flg] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[target_id2] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[target_extract_flg2] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[target_id3] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[target_extract_flg3] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[target_id4] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[target_extract_flg4] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[target_id5] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[target_extract_flg5] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[except_error_point] [int] NOT NULL DEFAULT (5),
	[except_mobilemail_flg] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[mailtext_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
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
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[last_duplicate_except_count] [bigint] NOT NULL DEFAULT (0),
	[enable_deduplication] [nvarchar] (2) NOT NULL DEFAULT (N'0')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MailDistSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MailDistSetting] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[maildist_id]
	) ON [PRIMARY]
GO