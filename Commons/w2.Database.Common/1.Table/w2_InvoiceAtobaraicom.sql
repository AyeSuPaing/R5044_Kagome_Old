IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[w2_InvoiceAtobaraicom]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_InvoiceAtobaraicom]
GO
/*
=========================================================================================================
  Module      : МуХеВв.comР┐ЛБПС (w2_InvoiceAtobaraicom.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_InvoiceAtobaraicom] (
	[order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[use_amount] [nvarchar] (8) NOT NULL DEFAULT (N''),
	[tax_amount] [nvarchar] (8) NOT NULL DEFAULT (N''),
	[limit_date] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[name_kj] [nvarchar] (40) NOT NULL DEFAULT (N''),
	[cv_barcode_data] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[cv_barcode_string1] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[cv_barcode_string2] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[yu_mt_ocr_code1] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[yu_mt_ocr_code2] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[yu_account_name] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[yu_account_no] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[yu_load_kind] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[cvs_company_name] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[cvs_user_name] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[bk_code] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[bk_branch_code] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[bk_name] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[bk_branch_name] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[bk_account_kind] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[bk_account_no] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[bk_account_name] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[bk_account_kana] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[mypage_pwd] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mypage_url] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[credit_deadline] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_InvoiceAtobaraicom] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_InvoiceAtobaraicom] PRIMARY KEY CLUSTERED
	(
		[order_id]
	) ON [PRIMARY]
GO
