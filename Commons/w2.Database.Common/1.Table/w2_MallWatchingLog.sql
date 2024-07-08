if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MallWatchingLog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MallWatchingLog]
GO
/*
=========================================================================================================
  Module      : モール監視ログマスタ(w2_MallWatchingLog.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MallWatchingLog] (
	[log_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[watching_date] [varchar] (20) NOT NULL DEFAULT (''),
	[watching_time] [varchar] (20) NOT NULL DEFAULT (''),
	[batch_id] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[mall_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[log_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[log_message] [ntext] NOT NULL DEFAULT (N''),
	[log_content1] [ntext] NOT NULL DEFAULT (N''),
	[log_content2] [ntext] NOT NULL DEFAULT (N''),
	[log_content3] [ntext] NOT NULL DEFAULT (N''),
	[log_content4] [ntext] NOT NULL DEFAULT (N''),
	[log_content5] [ntext] NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MallWatchingLog] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MallWatchingLog] PRIMARY KEY  NONCLUSTERED
	(
		[log_no]
	) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_MallWatchingLog_1] ON [dbo].[w2_MallWatchingLog]([watching_date], [watching_time]) ON [PRIMARY]
GO
