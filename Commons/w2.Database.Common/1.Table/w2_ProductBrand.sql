if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductBrand]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductBrand]
GO
/*
=========================================================================================================
  Module      : 商品ブランドマスタ(w2_ProductBrand.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductBrand] (
	[brand_id] [nvarchar] (30) NOT NULL,
	[brand_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[brand_title] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[additional_design_tag] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[seo_keyword] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[default_flg] [nvarchar] (10) NOT NULL DEFAULT (N'OFF'),
	[valid_flg] [nvarchar] (10) NOT NULL DEFAULT (N'ON'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductBrand] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductBrand] PRIMARY KEY  CLUSTERED
	(
		[brand_id]
	) ON [PRIMARY]
GO
