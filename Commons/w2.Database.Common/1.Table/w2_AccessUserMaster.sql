if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AccessUserMaster]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AccessUserMaster]
GO
/*
=========================================================================================================
  Module      : アクセスユーザマスタ(w2_AccessUserMaster.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_AccessUserMaster] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[access_user_id] [varchar] (50) NOT NULL DEFAULT (''),
	[user_id] [varchar] (50) NOT NULL DEFAULT (''),
	[first_acc_date] [datetime] NOT NULL,
	[last_acc_date] [datetime] NOT NULL,
	[recognized_date] [datetime] DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AccessUserMaster] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AccessUserMaster] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[access_user_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_AccessUserMaster_1] ON [dbo].[w2_AccessUserMaster]([dept_id], [user_id]) ON [PRIMARY]
GO