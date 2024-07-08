if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_DispSummaryAnalysis]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_DispSummaryAnalysis]
GO
/*
=========================================================================================================
  Module      : サマリ分析結果テーブル(w2_DispSummaryAnalysis.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_DispSummaryAnalysis] (
	[data_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[summary_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[tgt_year] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[tgt_month] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[tgt_day] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[value_name] [nvarchar] (1000) NOT NULL DEFAULT (N''),
	[counts] [bigint] NOT NULL DEFAULT (0),
	[reserved1] [bigint] NOT NULL DEFAULT (0),
	[reserved2] [bigint] NOT NULL DEFAULT (0),
	[reserved3] [bigint] NOT NULL DEFAULT (0),
	[reserved4] [bigint] NOT NULL DEFAULT (0),
	[reserved5] [bigint] NOT NULL DEFAULT (0),
	[reserved6] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[reserved7] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[reserved8] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[reserved9] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[reserved10] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[price] [decimal](23,3) NOT NULL DEFAULT (0),
	[price_tax] [decimal](23,3) NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_DispSummaryAnalysis] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_DispSummaryAnalysis] PRIMARY KEY  NONCLUSTERED
	(
		[data_no]
	) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_DispSummaryAnalysis_1] ON [dbo].[w2_DispSummaryAnalysis]([dept_id], [summary_kbn], [tgt_year], [tgt_month], [tgt_day]) ON [PRIMARY]
GO

/*
■ 決済通貨対応
ALTER TABLE [w2_DispSummaryAnalysis] ADD [price] [decimal] (23,3) NOT NULL DEFAULT (0);
ALTER TABLE [w2_DispSummaryAnalysis] ADD [price_tax] [decimal] (23,3) NOT NULL DEFAULT (0);
*/