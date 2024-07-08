if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_Favorite]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_Favorite]
GO
/*
=========================================================================================================
  Module      : お気に入り商品マスタ(w2_Favorite.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_Favorite] (
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_Favorite] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_Favorite] PRIMARY KEY  CLUSTERED
	(
		[user_id],
		[shop_id],
		[product_id],
		[variation_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_Favorite_1] ON [dbo].[w2_Favorite]([shop_id], [product_id], [variation_id]) ON [PRIMARY]
GO