if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserIntegrationHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserIntegrationHistory]
GO
/*
=========================================================================================================
  Module      : ГЖБ[ГUБ[УЭНЗЧЪЧЁПюХё (w2_UserIntegrationHistory.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserIntegrationHistory] (
	[user_integration_no] [bigint] NOT NULL,
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[branch_no] [int] NOT NULL DEFAULT (1),
	[table_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[primary_key1] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[primary_key2] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[primary_key3] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[primary_key4] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[primary_key5] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserIntegrationHistory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserIntegrationHistory] PRIMARY KEY  CLUSTERED
	(
		[user_integration_no],
		[user_id],
		[branch_no]
	) ON [PRIMARY]
GO