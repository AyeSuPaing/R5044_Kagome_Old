if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ContentsSummaryAnalysis]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ContentsSummaryAnalysis]
GO
/*
=========================================================================================================
  Module      : ГRГУГeГУГcЙЁР═ (w2_ContentsSummaryAnalysis.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ContentsSummaryAnalysis] (
	[data_no] [bigint] IDENTITY(1,1) NOT NULL,
	[date] [datetime] NOT NULL,
	[tgt_year] AS DATEPART(year, date) PERSISTED NOT NULL,
	[tgt_month] AS DATEPART(month, date) PERSISTED NOT NULL,
	[tgt_day] AS DATEPART(day, date) PERSISTED NOT NULL,
	[report_type] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[access_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[contents_type] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[contents_id] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[count] [bigint] NOT NULL DEFAULT (0),
	[price] [decimal] (23,3) NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ContentsSummaryAnalysis] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ContentsSummaryAnalysis] PRIMARY KEY  CLUSTERED
	(
		[data_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_ContentsSummaryAnalysis_1] ON [dbo].[w2_ContentsSummaryAnalysis]([date], [report_type], [access_kbn], [contents_type], [contents_id]) ON [PRIMARY]
GO
