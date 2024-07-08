/*
=========================================================================================================
  Module      : 商品タグ設定マスタ情報(w2_ProductTagSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductTagSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductTagSetting]
GO

CREATE TABLE [dbo].[w2_ProductTagSetting] (
	[tag_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[tag_id] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[tag_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[tag_discription] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[tag_valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductTagSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductTagSetting] PRIMARY KEY  CLUSTERED
	(
		[tag_no]
	) ON [PRIMARY]
GO