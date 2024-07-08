/*
=========================================================================================================
  Module      : Fixed purchase shipping work table(w2_WorkFixedPurchaseShipping.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[w2_WorkFixedPurchaseShipping]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_WorkFixedPurchaseShipping]
GO

CREATE TABLE [dbo].[w2_WorkFixedPurchaseShipping] (
	[fixed_purchase_id] [nvarchar] (30) NOT NULL,
	[fixed_purchase_shipping_no] [int] NOT NULL DEFAULT (1),
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
	[shipping_date] [datetime],
	[shipping_time] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[shipping_name1] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[shipping_name2] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[shipping_name_kana1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_name_kana2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_company_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_company_post_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_method] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[delivery_company_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[shipping_country_iso_code] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[shipping_country_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[shipping_addr5] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_receiving_store_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[shipping_receiving_store_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[shipping_receiving_store_type] [nvarchar] (5) NOT NULL DEFAULT (N'')
) ON [PRIMARY]

ALTER TABLE [dbo].[w2_WorkFixedPurchaseShipping] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkFixedPurchaseShipping] PRIMARY KEY CLUSTERED
	(
		[fixed_purchase_id],
		[fixed_purchase_shipping_no]
	) ON [PRIMARY]
GO
