if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_WorkRealShop]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_WorkRealShop]
GO
/*
=========================================================================================================
  Module      : リアル店舗情報ワークテーブル(w2_WorkRealShop.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_WorkRealShop] (
	[real_shop_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[name_kana] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[desc1_kbn_pc] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[desc1_pc] [ntext] NOT NULL DEFAULT (N''),
	[desc2_kbn_pc] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[desc2_pc] [ntext] NOT NULL DEFAULT (N''),
	[desc1_kbn_sp] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[desc1_sp] [ntext] NOT NULL DEFAULT (N''),
	[desc2_kbn_sp] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[desc2_sp] [ntext] NOT NULL DEFAULT (N''),
	[desc1_kbn_mb] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[desc1_mb] [ntext] NOT NULL DEFAULT (N''),
	[desc2_kbn_mb] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[desc2_mb] [ntext] NOT NULL DEFAULT (N''),
	[zip] [nvarchar] (8) NOT NULL DEFAULT (N''),
	[zip1] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[zip2] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[addr] [nvarchar] (400) NOT NULL DEFAULT (N''),
	[addr1] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[addr2] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[addr3] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[addr4] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[tel] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[tel_1] [nvarchar] (6) NOT NULL DEFAULT (N''),
	[tel_2] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[tel_3] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[fax] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[fax_1] [nvarchar] (6) NOT NULL DEFAULT (N''),
	[fax_2] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[fax_3] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[url] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[mail_addr] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[opening_hours] [ntext] NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (1),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[brand_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[area_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[longitude] [decimal] (9,6),
	[latitude] [decimal] (9,6)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkRealShop] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkRealShop] PRIMARY KEY  CLUSTERED
	(
		[real_shop_id]
	) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_WorkRealShop] ADD [brand_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_WorkRealShop] ADD [area_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_WorkRealShop] ADD [longitude] [decimal] (9,6);
ALTER TABLE [w2_WorkRealShop] ADD [latitude] [decimal] (9,6);
*/
