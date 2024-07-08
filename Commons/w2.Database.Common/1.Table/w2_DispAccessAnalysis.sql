if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_DispAccessAnalysis]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_DispAccessAnalysis]
GO
/*
=========================================================================================================
  Module      : アクセス解析結果テーブル(w2_DispAccessAnalysis.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_DispAccessAnalysis] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[tgt_year] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[tgt_month] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[tgt_day] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[access_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[total_page_views] [bigint] NOT NULL DEFAULT (0),
	[total_users] [bigint] NOT NULL DEFAULT (0),
	[total_sessions] [bigint] NOT NULL DEFAULT (0),
	[total_new_users] [bigint] NOT NULL DEFAULT (0),
	[total_new_sessions] [bigint] NOT NULL DEFAULT (0),
	[reserved1] [bigint] NOT NULL DEFAULT (0),
	[reserved2] [bigint] NOT NULL DEFAULT (0),
	[reserved3] [bigint] NOT NULL DEFAULT (0),
	[reserved4] [bigint] NOT NULL DEFAULT (0),
	[reserved5] [bigint] NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_DispAccessAnalysis] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_DispAccessAnalysis] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[tgt_year],
		[tgt_month],
		[tgt_day],
		[access_kbn]
	) ON [PRIMARY]
GO
