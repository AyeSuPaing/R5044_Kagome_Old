if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_LandingPageProduct]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_LandingPageProduct]
GO
/*
=========================================================================================================
  Module      : Lpページ商品 (w2_LandingPageProduct.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_LandingPageProduct] (
	[page_id] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[quantity] [int] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[branch_no] [int] NOT NULL DEFAULT (0),
	[variation_sort_number] [int] NOT NULL DEFAULT (0),
	[buy_type] [nvarchar] (20) NOT NULL DEFAULT (N'NORMAL')
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_w2_LandingPageProduct_1 ON w2_LandingPageProduct (page_id,branch_no,variation_sort_number)
GO

ALTER TABLE [dbo].[w2_LandingPageProduct] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_LandingPageProduct] PRIMARY KEY  CLUSTERED
	(
		[page_id],
		[shop_id],
		[product_id],
		[variation_id],
		[branch_no]
	) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_LandingPageProduct] ADD [variation_sort_number] [int] NOT NULL DEFAULT (0);
*/