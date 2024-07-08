if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_NoveltyTargetItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_NoveltyTargetItem]
GO
/*
=========================================================================================================
  Module      : ノベルティ対象アイテム(w2_NoveltyTargetItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_NoveltyTargetItem] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[novelty_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[novelty_target_item_no] [int] NOT NULL DEFAULT (1),
	[novelty_target_item_type] [nvarchar] (30) NOT NULL DEFAULT (N'ALL'),
	[novelty_target_item_value] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[novelty_target_item_type_sort_no] [int] NOT NULL DEFAULT (1),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_NoveltyTargetItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_NoveltyTargetItem] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[novelty_id],
		[novelty_target_item_no],
		[novelty_target_item_type],
		[novelty_target_item_value]
	) ON [PRIMARY]
GO
