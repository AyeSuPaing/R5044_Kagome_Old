if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductGroupItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductGroupItem]
GO
/*
=========================================================================================================
  Module      : 商品グループアイテムマスタ (w2_ProductGroupItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductGroupItem] (
	[product_group_id] [nvarchar] (30) NOT NULL,
	[item_no] [int] NOT NULL DEFAULT (1),
	[item_type] [nvarchar] (30) NOT NULL DEFAULT (N'PRODUCT'),
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[master_id] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductGroupItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductGroupItem] PRIMARY KEY  CLUSTERED
	(
		[product_group_id],
		[item_no]
	) ON [PRIMARY]
GO
