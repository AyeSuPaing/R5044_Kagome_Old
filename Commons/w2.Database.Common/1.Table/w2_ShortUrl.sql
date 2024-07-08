/*
=========================================================================================================
  Module      : ショートURLマスタ(w2_ShortUrl.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ShortUrl]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ShortUrl]
GO

CREATE TABLE [dbo].[w2_ShortUrl] (
	[surl_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[short_url] [nvarchar] (255) NOT NULL DEFAULT (N''),
	[long_url] [nvarchar] (2000) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ShortUrl] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ShortUrl] PRIMARY KEY  CLUSTERED
	(
		[surl_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_ShortUrl_1] ON [dbo].[w2_ShortUrl]([shop_id], [short_url]) ON [PRIMARY]
GO
