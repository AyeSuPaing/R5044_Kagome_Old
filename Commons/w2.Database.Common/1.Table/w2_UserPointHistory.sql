if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserPointHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserPointHistory]
GO
/*
=========================================================================================================
  Module      : ユーザポイント履歴マスタ(w2_UserPointHistory.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserPointHistory] (
	[user_id] [nvarchar] (30) NOT NULL,
	[history_no] [int] NOT NULL,
	[tgt_year] AS DATEPART(year, date_created) PERSISTED NOT NULL,
	[tgt_month] AS DATEPART(month, date_created) PERSISTED NOT NULL,
	[tgt_day] AS DATEPART(day, date_created) PERSISTED NOT NULL,
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[point_rule_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[point_rule_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'01'),
	[point_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'01'),
	[point_type] [nvarchar] (10) NOT NULL DEFAULT (N'01'),
	[point_inc_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[point_inc] [decimal] NOT NULL DEFAULT (0),
	[point_exp_extend] [nvarchar] (10) NOT NULL DEFAULT (N'+000000'),
	[user_point_exp] [datetime],
	[kbn1] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[kbn2] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[kbn3] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[kbn4] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[kbn5] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[memo] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[effective_date] [datetime],
	[restored_flg] [nvarchar] (2) NOT NULL DEFAULT (N'2'),
	[history_group_no] [int] NOT NULL,
	[cart_id] [nvarchar] (30) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserPointHistory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserPointHistory] PRIMARY KEY  CLUSTERED
	(
		[tgt_year],
		[tgt_month],
		[tgt_day],
		[user_id],
		[history_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_UserPointHistory_1] ON [dbo].[w2_UserPointHistory]([user_id], [history_no]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_UserPointHistory_2] ON [dbo].[w2_UserPointHistory]([user_id], [history_group_no]) ON [PRIMARY]
GO