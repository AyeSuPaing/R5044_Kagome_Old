if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AccessLogAccount]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AccessLogAccount]
GO
/*
=========================================================================================================
  Module      : アクセスログアカウントテーブル(w2_AccessLogAccount.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_AccessLogAccount] (
	[account_id] [nvarchar] (30) NOT NULL DEFAULT (''),
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AccessLogAccount] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AccessLogAccount] PRIMARY KEY  CLUSTERED
	(
		[account_id]
	) ON [PRIMARY]
GO
