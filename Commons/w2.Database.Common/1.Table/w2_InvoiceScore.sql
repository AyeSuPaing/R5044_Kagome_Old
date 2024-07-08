if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_InvoiceScore]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_InvoiceScore]
GO
/*
=========================================================================================================
  Module      : スコア後払い請求書印字データ(w2_InvoiceScore.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_InvoiceScore] (
	[order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[invoice_bar_code] [nvarchar] (44) NOT NULL DEFAULT (N''),
	[invoice_code] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[invoice_kbn] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[history_seq] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[reminded_kbn] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[company_name] [nvarchar] (35) NOT NULL DEFAULT (N''),
	[department] [nvarchar] (35) NOT NULL DEFAULT (N''),
	[customer_name] [nvarchar] (25) NOT NULL DEFAULT (N''),
	[customer_zip] [nvarchar] (7) NOT NULL DEFAULT (N''),
	[customer_address1] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[customer_address2] [nvarchar] (31) NOT NULL DEFAULT (N''),
	[customer_address3] [nvarchar] (35) NOT NULL DEFAULT (N''),
	[shop_zip] [nvarchar] (7) NOT NULL DEFAULT (N''),
	[shop_address1] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[shop_address2] [nvarchar] (31) NOT NULL DEFAULT (N''),
	[shop_address3] [nvarchar] (35) NOT NULL DEFAULT (N''),
	[shop_tel] [nvarchar] (17) NOT NULL DEFAULT (N''),
	[shop_fax] [nvarchar] (17) NOT NULL DEFAULT (N''),
	[billed_amount] [nvarchar] (6) NOT NULL DEFAULT (N''),
	[tax] [nvarchar] (6) NOT NULL DEFAULT (N''),
	[time_of_receipts] [nvarchar] (11) NOT NULL DEFAULT (N''),
	[invoice_start_date] [nvarchar] (11)  NOT NULL DEFAULT (N''),
	[invoice_title] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[nissen_message1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[nissen_message2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[nissen_message3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[nissen_message4] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[invoice_shopsite_name] [nvarchar] (75) NOT NULL DEFAULT (N''),
	[shop_email] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[nissen_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[nissen_qa_url] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shop_order_date] [nvarchar] (11) NOT NULL DEFAULT (N''),
	[shop_code] [nvarchar] (9) NOT NULL DEFAULT (N''),
	[nissen_transaction_id] [nvarchar] (11) NOT NULL DEFAULT (N''),
	[shop_transaction_id1] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[shop_transaction_id2] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[shop_message1] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[shop_message2] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[shop_message3] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[shop_message4] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[shop_message5] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[invoice_form] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[postal_account_number] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[postal_account_holder_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[postal_font_top_row] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[postal_font_bottom_row] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[remittance_address] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[x_symbol] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[reserve1] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[reserve2] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[reserve3] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_InvoiceScore] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_InvoiceScore] PRIMARY KEY  CLUSTERED
	(
		[order_id]
	) ON [PRIMARY]
GO
