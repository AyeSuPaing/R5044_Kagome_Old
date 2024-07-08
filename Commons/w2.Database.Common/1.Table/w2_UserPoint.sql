if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserPoint]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserPoint]
GO
/*
=========================================================================================================
  Module      : ユーザポイントマスタ(w2_UserPoint.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserPoint] (
	[user_id] [nvarchar] (30) NOT NULL,
	[point_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'01'),
	[point_kbn_no] [int] NOT NULL DEFAULT (1),
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[point_rule_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[point_rule_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'01'),
	[point_type] [nvarchar] (10) NOT NULL DEFAULT (N'01'),
	[point_inc_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[point] [decimal] NOT NULL DEFAULT (0),
	[point_exp] [datetime],
	[history_no] [int],
	[kbn1] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[kbn2] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[kbn3] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[kbn4] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[kbn5] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[effective_date] [datetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserPoint] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserPoint] PRIMARY KEY  CLUSTERED
	(
		[user_id],
		[point_kbn_no]
	) ON [PRIMARY]
GO
