/*
=========================================================================================================
  Module      : Scoring Sale Product (w2_ScoringSaleProduct.sql)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[w2_ScoringSaleProduct]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_ScoringSaleProduct]
GO

CREATE TABLE [dbo].[w2_ScoringSaleProduct] (
	[scoring_sale_id] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[branch_no] [int] NOT NULL,
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[quantity] [int] NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ScoringSaleProduct] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ScoringSaleProduct] PRIMARY KEY CLUSTERED
	(
		[scoring_sale_id],
		[branch_no]
	) ON [PRIMARY]
GO
