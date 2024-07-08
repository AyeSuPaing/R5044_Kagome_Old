/*
=========================================================================================================
  Module      : 広告コードログマスタ(w2_AdvCodeLog.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AdvCodeLog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AdvCodeLog]
GO

CREATE TABLE [dbo].[w2_AdvCodeLog] (
	[advcodelog_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[access_date] [nvarchar] (20) NOT NULL DEFAULT (''),
	[access_time] [nvarchar] (20) NOT NULL DEFAULT (''),
	[advertisement_code] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[career_id] [nvarchar] (10) NOT NULL DEFAULT (''),
	[mobile_uid] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[access_user_id] [nvarchar] (255) NOT NULL DEFAULT ('')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AdvCodeLog] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AdvCodeLog] PRIMARY KEY  NONCLUSTERED
	(
		[advcodelog_no]
	) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_AdvCodeLog_1] ON [dbo].[w2_AdvCodeLog]([access_date], [access_time]) ON [PRIMARY]
GO
