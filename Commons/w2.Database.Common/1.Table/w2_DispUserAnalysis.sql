if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_DispUserAnalysis]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_DispUserAnalysis]
GO
/*
=========================================================================================================
  Module      : ユーザ解析結果テーブル(w2_DispUserAnalysis.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_DispUserAnalysis] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[tgt_year] [nvarchar] (4) NOT NULL DEFAULT (''),
	[tgt_month] [nvarchar] (2) NOT NULL DEFAULT (''),
	[tgt_day] [nvarchar] (2) NOT NULL DEFAULT (''),
	[potential_new] [bigint] NOT NULL DEFAULT (0),
	[potential_all] [bigint] NOT NULL DEFAULT (0),
	[potential_active] [bigint] NOT NULL DEFAULT (0),
	[potential_unactive1] [bigint] NOT NULL DEFAULT (0),
	[potential_unactive2] [bigint] NOT NULL DEFAULT (0),
	[recognize_new] [bigint] NOT NULL DEFAULT (0),
	[recognize_all] [bigint] NOT NULL DEFAULT (0),
	[recognize_active] [bigint] NOT NULL DEFAULT (0),
	[recognize_unactive1] [bigint] NOT NULL DEFAULT (0),
	[recognize_unactive2] [bigint] NOT NULL DEFAULT (0),
	[leave_new] [bigint] NOT NULL DEFAULT (0),
	[leave_all] [bigint] NOT NULL DEFAULT (0),
	[reserved1] [bigint] NOT NULL DEFAULT (0),
	[reserved2] [bigint] NOT NULL DEFAULT (0),
	[reserved3] [bigint] NOT NULL DEFAULT (0),
	[reserved4] [bigint] NOT NULL DEFAULT (0),
	[reserved5] [bigint] NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_DispUserAnalysis] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_DispUserAnalysis] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[tgt_year],
		[tgt_month],
		[tgt_day]
	) ON [PRIMARY]
GO
