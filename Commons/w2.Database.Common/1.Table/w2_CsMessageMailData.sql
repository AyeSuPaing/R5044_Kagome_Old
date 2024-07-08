if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsMessageMailData]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsMessageMailData]
GO
/*
=========================================================================================================
  Module      : メッセージメールデータ(w2_CsMessageMailData.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsMessageMailData] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mail_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mail_data] [nvarchar] (max) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsMessageMailData] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsMessageMailData] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[mail_id]
	) ON [PRIMARY]
GO
