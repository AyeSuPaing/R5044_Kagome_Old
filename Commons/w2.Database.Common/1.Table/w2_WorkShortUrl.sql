if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_WorkShortUrl]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_WorkShortUrl]
GO
/*
=========================================================================================================
  Module      : ショートURLマスタ用ワークテーブル(w2_WorkShortUrl)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_WorkShortUrl] (
	[surl_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[short_url] [nvarchar] (255) NOT NULL DEFAULT (N''),
	[long_url] [nvarchar] (2000) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkShortUrl] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkShortUrl] PRIMARY KEY  CLUSTERED
	(
		[surl_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_WorkShortUrl_1] ON [dbo].[w2_WorkShortUrl]([shop_id], [short_url]) ON [PRIMARY]
GO
