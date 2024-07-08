if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_NoveltyGrantItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_NoveltyGrantItem]
GO
/*
=========================================================================================================
  Module      : ノベルティ付与アイテム(w2_NoveltyGrantItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_NoveltyGrantItem] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[novelty_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[condition_no] [int] NOT NULL DEFAULT (1),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[sort_no] [int] NOT NULL DEFAULT (1),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_NoveltyGrantItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_NoveltyGrantItem] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[novelty_id],
		[condition_no],
		[product_id]
	) ON [PRIMARY]
GO
