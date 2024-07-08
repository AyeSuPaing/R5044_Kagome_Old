if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AccessRecUserMaster]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AccessRecUserMaster]
GO
/*
=========================================================================================================
  Module      : 認知ユーザマスタ(w2_AccessRecUserMaster.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_AccessRecUserMaster] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[user_id] [varchar] (50) NOT NULL DEFAULT (''),
	[recognized_date] [datetime],
	[last_acc_date] [datetime] NOT NULL DEFAULT (getdate()),
	[last_login_date] [datetime] NOT NULL DEFAULT (getdate()),
	[leave_date] [datetime]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AccessRecUserMaster] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AccessRecUserMaster] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[user_id]
	) ON [PRIMARY]
GO
