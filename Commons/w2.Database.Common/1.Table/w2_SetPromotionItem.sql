if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_SetPromotionItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_SetPromotionItem]
GO
/*
=========================================================================================================
  Module      : セットプロモーションアイテムマスタ(w2_SetPromotionItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_SetPromotionItem] (
	[setpromotion_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[setpromotion_item_no] [int] NOT NULL DEFAULT (1),
	[setpromotion_item_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[setpromotion_items] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[setpromotion_item_quantity] [int] NOT NULL DEFAULT (1),
	[setpromotion_item_quantity_more_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_SetPromotionItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_SetPromotionItem] PRIMARY KEY  CLUSTERED
	(
		[setpromotion_id],
		[setpromotion_item_no]
	) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_SetPromotionItem] ADD [setpromotion_item_quantity_more_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
*/
