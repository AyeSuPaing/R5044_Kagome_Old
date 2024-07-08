if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductSet]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductSet]
GO
/*
=========================================================================================================
  Module      : 商品セットマスタ(w2_ProductSet.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductSet] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_set_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[product_set_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[parent_min] [int],
	[parent_max] [int],
	[child_min] [int],
	[child_max] [int],
	[priority] [int] NOT NULL DEFAULT (1),
	[description_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[description] [ntext] NOT NULL DEFAULT (N''),
	[description_kbn_mobile] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[description_mobile] [ntext] NOT NULL DEFAULT (N''),
	[max_sell_quantity] [int] NOT NULL DEFAULT (1),
	[shipping_size_kbn] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[editable_flg] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductSet] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductSet] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[product_set_id]
	) ON [PRIMARY]
GO
