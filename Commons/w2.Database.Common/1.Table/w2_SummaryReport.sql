IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[w2_SummaryReport]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_SummaryReport]
GO
/*
=========================================================================================================
  Module      : Summary Report (w2_SummaryReport.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_SummaryReport] (
	[period_kbn] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[data_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[report_date] [datetime] NOT NULL DEFAULT (GETDATE()),
	[data] [decimal] (18,3) NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_SummaryReport] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_SummaryReport] PRIMARY KEY  CLUSTERED
	(
		[period_kbn],
		[data_kbn],
		[report_date]
	) ON [PRIMARY]
GO