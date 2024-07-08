if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MailErrorAddr]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MailErrorAddr]
GO
/*
=========================================================================================================
  Module      : メールエラーアドレスマスタ(w2_MailErrorAddr.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MailErrorAddr] (
	[mail_addr] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[error_point] [int] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MailErrorAddr] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MailErrorAddr] PRIMARY KEY  CLUSTERED
	(
		[mail_addr]
	) ON [PRIMARY]
GO
