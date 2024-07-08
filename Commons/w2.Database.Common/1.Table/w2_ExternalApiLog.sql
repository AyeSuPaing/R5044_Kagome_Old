if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ExternalApiLog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ExternalApiLog]
GO
/*
=========================================================================================================
  Module      : КOХФШAМgВ`ВoВhГНГOГ}ГXГ^(w2_ExternalApiLog.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ExternalApiLog] (
	[log_id] [bigint] identity(1, 1) NOT NULL,
	[date_excuted] [datetime] NOT NULL,
	[log_level] [nvarchar] (10) NOT NULL,
	[source] [nvarchar] (500) NOT NULL,
	[stacktrace] [nvarchar] (max) NOT NULL,
	[message] [nvarchar] (200) NOT NULL,
	[data] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[exception] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ExternalApiLog] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ExternalApiLog] PRIMARY KEY  CLUSTERED
	(
		[log_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_ExternalApiLog_1] ON [dbo].[w2_ExternalApiLog]([date_excuted], [log_level]) ON [PRIMARY]
GO
