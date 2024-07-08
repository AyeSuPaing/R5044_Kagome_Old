 if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_WorkUserShipping]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_WorkUserShipping]
GO
/*
=========================================================================================================
  Module      : ユーザ配送先情報ワークテーブル(w2_WorkUserShipping.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_WorkUserShipping] (
	[user_id] [nvarchar] (30) NOT NULL,
	[shipping_no] [int] NOT NULL DEFAULT (1),
	[name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_name] [nvarchar] (40) NOT NULL DEFAULT (N''),
	[shipping_name_kana] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[shipping_zip] [nvarchar] (8) NOT NULL DEFAULT (N''),
	[shipping_addr1] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_addr2] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_addr3] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_addr4] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_tel1] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[shipping_tel2] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[shipping_tel3] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[shipping_fax] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[shipping_company] [nvarchar] (30) NOT NULL DEFAULT (N'0'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[shipping_name1] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[shipping_name2] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[shipping_name_kana1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_name_kana2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_company_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_company_post_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_country_iso_code] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[shipping_country_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[shipping_addr5] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_receiving_store_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[shipping_receiving_store_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[shipping_receiving_store_type] [nvarchar] (5) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkUserShipping] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkUserShipping] PRIMARY KEY  CLUSTERED
	(
		[user_id],
		[shipping_no]
	) ON [PRIMARY]
GO
/*
ALTER TABLE [w2_WorkUserShipping] ADD [shipping_receiving_store_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_WorkUserShipping] ADD [shipping_receiving_store_id] [nvarchar] (20) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_WorkUserShipping] ADD [shipping_receiving_store_type] [nvarchar] (5) NOT NULL DEFAULT (N'');
*/