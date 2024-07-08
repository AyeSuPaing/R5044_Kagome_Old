﻿ if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductSubImageSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductSubImageSetting]
GO
/*
=========================================================================================================
  Module      : 商品サブ画像設定(w2_ProductSubImageSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductSubImageSetting] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_sub_image_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_sub_image_no] [int] NOT NULL DEFAULT (1),
	[product_sub_image_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[product_sub_image_discription] [ntext] NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductSubImageSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductSubImageSetting] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[product_sub_image_id],
		[product_sub_image_no]
	) ON [PRIMARY]
GO
