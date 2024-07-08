if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsMessageMailAttachment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsMessageMailAttachment]
GO
/*
=========================================================================================================
  Module      : メッセージメール添付ファイル(w2_CsMessageMailAttachment.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsMessageMailAttachment] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mail_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[file_no] [int] NOT NULL,
	[file_name] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[file_data] [varbinary] (max) NOT NULL,
	[date_created] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsMessageMailAttachment] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsMessageMailAttachment] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[mail_id],
		[file_no]
	) ON [PRIMARY]
GO
