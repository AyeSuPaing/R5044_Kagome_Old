if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductTaxCategory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductTaxCategory]
GO
/*
=========================================================================================================
  Module      : 商品税率カテゴリマスタ (w2_ProductTaxCategory.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductTaxCategory] (
	[tax_category_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[tax_category_name] [nvarchar] (40) NOT NULL DEFAULT (N''),
	[tax_rate] [decimal] (5,2) NOT NULL DEFAULT (0),
	[display_order] [int] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductTaxCategory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductTaxCategory] PRIMARY KEY  CLUSTERED
	(
		[tax_category_id]
	) ON [PRIMARY]
GO

INSERT INTO [w2_ProductTaxCategory] (
	tax_category_id,
	tax_category_name,
	tax_rate,
	display_order
	)
VALUES (
	'default',
	'デフォルト税率',
	8,
	0
	)
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_Product]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
UPDATE [w2_Product] SET [tax_category_id] = 'default'
GO