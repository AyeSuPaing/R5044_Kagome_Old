if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MemberRankRule]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MemberRankRule]
GO
/*
=========================================================================================================
  Module      : 会員ランク付与ルール情報(w2_MemberRankRule.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MemberRankRule] (
	[member_rank_rule_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[member_rank_rule_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[status] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[last_count] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[last_exec_date] [datetime],
	[target_extract_type] [nvarchar] (10) NOT NULL DEFAULT (N'DURING'),
	[target_extract_start] [datetime],
	[target_extract_end] [datetime],
	[target_extract_days_ago] [int],
	[target_extract_total_price_from] [decimal] (18,3),
	[target_extract_total_price_to] [decimal] (18,3),
	[target_extract_total_count_from] [int],
	[target_extract_total_count_to] [int],
	[target_extract_old_rank_flg] [nvarchar] (10) NOT NULL DEFAULT (N'OFF'),
	[rank_change_type] [nvarchar] (10) NOT NULL DEFAULT (N'UP'),
	[rank_change_rank_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
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

ALTER TABLE [dbo].[w2_MemberRankRule] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MemberRankRule] PRIMARY KEY  CLUSTERED
	(
		[member_rank_rule_id]
	) ON [PRIMARY]
GO

/*
■ 決済通貨対応
ALTER TABLE [w2_MemberRankRule] ALTER COLUMN [target_extract_total_price_from] [decimal] (18,3);
ALTER TABLE [w2_MemberRankRule] ALTER COLUMN [target_extract_total_price_to] [decimal] (18,3);
*/
