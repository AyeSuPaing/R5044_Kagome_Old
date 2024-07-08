if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductRankingItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductRankingItem]
GO
/*
=========================================================================================================
  Module      : 商品ランキングアイテム設定マスタ(w2_ProductRankingItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductRankingItem] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[ranking_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[rank] [int] NOT NULL DEFAULT (1),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[fixation_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductRankingItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductRankingItem] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[ranking_id],
		[rank]
	) ON [PRIMARY]
GO