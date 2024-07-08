if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductRanking]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductRanking]
GO
/*
=========================================================================================================
  Module      : 商品ランキング設定マスタ(w2_ProductRanking.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductRanking] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[ranking_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[total_type] [nvarchar] (10) NOT NULL DEFAULT (N'AUTO'),
	[total_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'PERIOD'),
	[total_from] [datetime] NOT NULL,
	[total_to] [datetime] NOT NULL,
	[total_days] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[category_kbn] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[category_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[desc1] [nvarchar] (250) NOT NULL DEFAULT (N''),
	[stock_kbn] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[brand_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[exclude_category_ids] [nvarchar] (max) NOT NULL DEFAULT (N''),
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductRanking] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductRanking] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[ranking_id]
	) ON [PRIMARY]
GO
