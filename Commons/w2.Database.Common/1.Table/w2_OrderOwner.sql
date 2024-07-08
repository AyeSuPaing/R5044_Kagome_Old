if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_OrderOwner]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_OrderOwner]
GO
/*
=========================================================================================================
  Module      : ТНХ╢О╥ПюХё(w2_OrderOwner.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_OrderOwner] (
	[order_id] [nvarchar] (30) NOT NULL,
	[owner_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'PC_USER'),
	[owner_name] [nvarchar] (40) NOT NULL DEFAULT (N''),
	[owner_name_kana] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[owner_mail_addr] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[owner_zip] [nvarchar] (8) NOT NULL DEFAULT (N''),
	[owner_addr1] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[owner_addr2] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[owner_addr3] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[owner_addr4] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[owner_tel1] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[owner_tel2] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[owner_tel3] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[owner_fax] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[owner_sex] [nvarchar] (10) NOT NULL DEFAULT (N'UNKNOWN'),
	[owner_birth] [datetime],
	[owner_company_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[owner_company_post_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[owner_company_exective_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[owner_name1] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[owner_name2] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[owner_name_kana1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[owner_name_kana2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[owner_mail_addr2] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[access_country_iso_code] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[access_language_code] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[access_language_locale_id] [nvarchar] (7) NOT NULL DEFAULT (N''),
	[access_currency_code] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[access_currency_locale_id] [nvarchar] (7) NOT NULL DEFAULT (N''),
	[owner_addr_country_iso_code] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[owner_addr_country_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[owner_addr5] [nvarchar] (50) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_OrderOwner] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_OrderOwner] PRIMARY KEY  CLUSTERED
	(
		[order_id]
	) ON [PRIMARY]
GO
