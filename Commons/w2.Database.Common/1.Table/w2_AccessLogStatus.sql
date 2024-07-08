if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AccessLogStatus]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AccessLogStatus]
GO
/*
=========================================================================================================
  Module      : アクセスログ処理ステータス(w2_AccessLogStatus.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_AccessLogStatus] (
	[target_date] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[day_status] [nvarchar] (2) NOT NULL DEFAULT (N'00'),
	[month_status] [nvarchar] (2) NOT NULL DEFAULT (N'00'),
	[import_files] [int] NOT NULL DEFAULT (0),
	[import_files_total] [int] NOT NULL DEFAULT (0),
	[target_filename] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AccessLogStatus] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AccessLogStatus] PRIMARY KEY  CLUSTERED
	(
		[target_date]
	) ON [PRIMARY]
GO
