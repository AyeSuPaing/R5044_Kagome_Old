/*
=========================================================================================================
  Module      : 商品タグマスタ情報(w2_ProductTag.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductTag]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductTag]
GO

CREATE TABLE [dbo].[w2_ProductTag] (
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductTag] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductTag] PRIMARY KEY  CLUSTERED
	(
		[product_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_ProductTag_1] ON [dbo].[w2_ProductTag]([product_id]) ON [PRIMARY]
GO