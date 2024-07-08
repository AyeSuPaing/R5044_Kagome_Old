if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_InvoiceDskDeferred]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_InvoiceDskDeferred]
GO
/*
=========================================================================================================
  Module      : DSKМуХеВвР┐ЛБПС (w2_InvoiceDskDeferred.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_InvoiceDskDeferred] (
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
	[invoice_start_date] [nvarchar] (11) NOT NULL DEFAULT (N''),
	[invoice_title] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[message1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[message2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[message3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[message4] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[invoice_shopsite_name] [nvarchar] (75) NOT NULL DEFAULT (N''),
	[shop_email] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[qa_url] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shop_order_date] [nvarchar] (11) NOT NULL DEFAULT (N''),
	[shop_code] [nvarchar] (9) NOT NULL DEFAULT (N''),
	[transaction_id] [nvarchar] (11) NOT NULL DEFAULT (N''),
	[shop_transaction_id1] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[shop_transaction_id2] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[shop_message1] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[shop_message2] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[shop_message3] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[shop_message4] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[shop_message5] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[yobi1] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[yobi2] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[yobi3] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[yobi4] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[yobi5] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[yobi6] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[yobi7] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_InvoiceDskDeferred] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_InvoiceDskDeferred] PRIMARY KEY  CLUSTERED
	(
		[order_id]
	) ON [PRIMARY]
GO
