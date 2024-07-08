if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductSearchWordHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductSearchWordHistory]
GO
/*
=========================================================================================================
  Module      : МЯНїГПБ[ГhЧЪЧЁ(w2_ProductSearchWordHistory.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductSearchWordHistory] (
	[history_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[access_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[search_word] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[hits] [int] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductSearchWordHistory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductSearchWordHistory] PRIMARY KEY  NONCLUSTERED
	(
		[history_no]
	) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_ProductSearchWordHistory_1] ON [dbo].[w2_ProductSearchWordHistory]([dept_id], [date_created]) ON [PRIMARY]
GO
