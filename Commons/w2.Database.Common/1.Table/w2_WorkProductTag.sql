/*
=========================================================================================================
  Module      : 商品タグマスタワーク用情報(w2_WorkProductTag.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_WorkProductTag]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_WorkProductTag]
GO

CREATE TABLE [dbo].[w2_WorkProductTag] (
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkProductTag] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkProductTag] PRIMARY KEY  CLUSTERED
	(
		[product_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_WorkProductTag_1] ON [dbo].[w2_WorkProductTag]([product_id]) ON [PRIMARY]
GO