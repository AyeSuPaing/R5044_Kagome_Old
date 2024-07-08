if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_Preview]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_Preview]
GO
/*
=========================================================================================================
  Module      : プレビューマスタ(w2_Preview.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_Preview] (
	[preview_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[preview_id1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[preview_id2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[preview_id3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[preview_id4] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[preview_id5] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[preview_data] [ntext] NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_Preview] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_Preview] PRIMARY KEY  CLUSTERED
	(
		[preview_kbn],
		[preview_id1],
		[preview_id2],
		[preview_id3],
		[preview_id4],
		[preview_id5]
	) ON [PRIMARY]
GO
