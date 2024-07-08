if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_FixedPurchaseProductChangeSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_FixedPurchaseProductChangeSetting]
GO
/*
=========================================================================================================
  Module      : 定期商品変更設定(w2_FixedPurchaseProductChangeSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_FixedPurchaseProductChangeSetting] (
	[fixed_purchase_product_change_id] [nvarchar](50) NOT NULL DEFAULT (N''),
	[fixed_purchase_product_change_name] [nvarchar](100) NOT NULL DEFAULT (N''),
	[priority] [int] NOT NULL DEFAULT (1),
	[valid_flg] [nvarchar](1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar](20) NOT NULL DEFAULT (N'')
	) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FixedPurchaseProductChangeSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FixedPurchaseProductChangeSetting] PRIMARY KEY  CLUSTERED
	(
		[fixed_purchase_product_change_id]
	) ON [PRIMARY]
GO