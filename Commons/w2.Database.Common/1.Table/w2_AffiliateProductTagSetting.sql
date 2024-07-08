if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AffiliateProductTagSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AffiliateProductTagSetting]
GO
/*
=========================================================================================================
  Module      : アフィリエイト商品タグ設定マスタ (w2_AffiliateProductTagSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_AffiliateProductTagSetting] (
	[affiliate_product_tag_id] [int] IDENTITY(1,1) NOT NULL,
	[tag_name] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[tag_content] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[tag_delimiter] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AffiliateProductTagSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AffiliateProductTagSetting] PRIMARY KEY  CLUSTERED
	(
		[affiliate_product_tag_id]
	) ON [PRIMARY]
GO
