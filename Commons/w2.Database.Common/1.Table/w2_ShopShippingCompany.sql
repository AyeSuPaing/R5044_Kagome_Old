if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ShopShippingCompany]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ShopShippingCompany]
GO
/*
=========================================================================================================
  Module      : 配送種別配送会社マスタ (w2_ShopShippingCompany.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ShopShippingCompany] (
	[shipping_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[shipping_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[delivery_company_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[default_delivery_company] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ShopShippingCompany] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ShopShippingCompany] PRIMARY KEY  CLUSTERED
	(
		[shipping_id],
		[shipping_kbn],
		[delivery_company_id]
	) ON [PRIMARY]
GO
