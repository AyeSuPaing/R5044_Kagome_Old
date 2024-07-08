if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AffiliateTagSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AffiliateTagSetting]
GO
/*
=========================================================================================================
  Module      : アフィリエイトタグ設定マスタ(w2_AffiliateTagSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_AffiliateTagSetting] (
	[affiliate_id] [int] IDENTITY(1,1) NOT NULL,
	[affiliate_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[affiliate_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'PC/SP'),
	[session_name1] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[session_name2] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[user_agent_coop_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (10),
	[affiliate_tag1] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[affiliate_tag2] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[affiliate_tag3] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[affiliate_tag4] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[affiliate_tag5] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[affiliate_tag6] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[affiliate_tag7] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[affiliate_tag8] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[affiliate_tag9] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[affiliate_tag10] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[affiliate_product_tag_id] [int],
	[output_location] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AffiliateTagSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AffiliateTagSetting] PRIMARY KEY  CLUSTERED
	(
		[affiliate_id]
	) ON [PRIMARY]
GO
