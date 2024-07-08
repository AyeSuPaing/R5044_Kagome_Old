if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MailDistExceptList]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MailDistExceptList]
GO
/*
=========================================================================================================
  Module      : メール配信排除アドレス(w2_MailDistExceptList.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MailDistExceptList] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[maildist_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mail_addr] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MailDistExceptList] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MailDistExceptList] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[maildist_id],
		[mail_addr]
	) ON [PRIMARY]
GO
