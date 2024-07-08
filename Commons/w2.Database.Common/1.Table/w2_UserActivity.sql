if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserActivity]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserActivity]
GO
/*
=========================================================================================================
  Module      : ユーザーアクティビティ (w2_UserActivity.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserActivity] (
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[master_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[master_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[date] [datetime] NOT NULL,
	[tgt_year] AS DATEPART(year, date) PERSISTED NOT NULL,
	[tgt_month] AS DATEPART(month, date) PERSISTED NOT NULL,
	[tgt_day] AS DATEPART(day, date) PERSISTED NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserActivity] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserActivity] PRIMARY KEY  CLUSTERED
	(
		[user_id],
		[master_kbn],
		[master_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_UserActivity_1] ON [dbo].[w2_UserActivity]([master_kbn], [master_id]) ON [PRIMARY]
GO
