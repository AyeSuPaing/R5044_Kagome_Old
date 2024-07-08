if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ContentsLog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ContentsLog]
GO
/*
=========================================================================================================
  Module      : コンテンツログ (w2_ContentsLog.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ContentsLog] (
	[log_no] [bigint] IDENTITY(1,1) NOT NULL,
	[date] [datetime] DEFAULT (getdate()),
	[report_type] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[access_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[contents_type] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[contents_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[price] [decimal] (23,3) NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ContentsLog] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ContentsLog] PRIMARY KEY  CLUSTERED
	(
		[log_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_ContentsLog_1] ON [dbo].[w2_ContentsLog]([date], [report_type], [access_kbn], [contents_type], [contents_id]) ON [PRIMARY]
GO
