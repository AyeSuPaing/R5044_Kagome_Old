if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ContentsTag]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ContentsTag]
GO
/*
=========================================================================================================
  Module      : コンテンツタグ (w2_ContentsTag.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ContentsTag] (
	[contents_tag_id] [bigint] IDENTITY (1, 1) NOT NULL,
	[contents_tag_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ContentsTag] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ContentsTag] PRIMARY KEY  CLUSTERED
	(
		[contents_tag_id]
	) ON [PRIMARY]
GO
