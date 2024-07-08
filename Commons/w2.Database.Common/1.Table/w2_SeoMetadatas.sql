if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_SeoMetadatas]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_SeoMetadatas]
GO
/*
=========================================================================================================
  Module      : SEOメタデータマスタ(w2_SeoMetadatas.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_SeoMetadatas] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[data_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[html_title] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
    [metadata_keywords] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
    [metadata_desc] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[comment] [ntext] NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
    [default_text] [nvarchar](200) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_SeoMetadatas] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_SeoMetadatas] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[data_kbn]
	) ON [PRIMARY]
GO
