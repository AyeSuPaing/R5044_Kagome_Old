if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_InvoiceVeritrans]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_InvoiceVeritrans]
GO
/*
=========================================================================================================
  Module      : ベリトランス後払い請求書印字データ(w2_InvoiceVeritrans.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_InvoiceVeritrans] (
	[order_id] [nvarchar](30) NOT NULL DEFAULT (N''),
	[service_type] [nvarchar](10) NOT NULL DEFAULT (N''),
	[m_status] [nvarchar](32) NOT NULL DEFAULT (N''),
	[v_result_code] [nvarchar](16) NOT NULL DEFAULT (N''),
	[m_err_msg] [nvarchar](max) NOT NULL DEFAULT (N''),
	[m_arch_txn] [nvarchar](100) NOT NULL DEFAULT (N''),
	[payment_order_id] [nvarchar](50) NOT NULL DEFAULT (N''),
	[cust_txn] [nvarchar](100) NOT NULL DEFAULT (N''),
	[txn_version] [nvarchar](5) NOT NULL DEFAULT (N''),
	[invoice_bar_code] [nvarchar](44) NOT NULL DEFAULT (N''),
	[invoice_code] [nvarchar](16),
	[invoice_kbn] [nvarchar](1),
	[history_seq] [nvarchar](1),
	[reminded_kbn] [nvarchar](2),
	[company_name] [nvarchar](35),
	[department] [nvarchar](35),
	[customer_name] [nvarchar](25) NOT NULL DEFAULT (N''),
	[customer_zip] [nvarchar](7),
	[customer_address1] [nvarchar](4),
	[customer_address2] [nvarchar](31),
	[customer_address3] [nvarchar](35),
	[shop_zip] [nvarchar](7),
	[shop_address1] [nvarchar](4),
	[shop_address2] [nvarchar](31),
	[shop_address3] [nvarchar](35),
	[shop_tel] [nvarchar](17),
	[shop_fax] [nvarchar](17),
	[billed_amount] [nvarchar](6) NOT NULL DEFAULT (N''),
	[tax] [nvarchar](6),
	[time_of_receipts] [datetime],
	[invoice_start_date] [datetime],
	[invoice_title] [nvarchar](3) NOT NULL DEFAULT (N''),
	[nissen_message1] [nvarchar](30),
	[nissen_message2] [nvarchar](30),
	[nissen_message3] [nvarchar](30),
	[nissen_message4] [nvarchar](30),
	[invoice_shopsite_name] [nvarchar](75) NOT NULL DEFAULT (N''),
	[shop_email] [nvarchar](100),
	[nissen_name] [nvarchar](50),
	[nissen_qa_url] [nvarchar](50),
	[shop_order_date] [datetime] NOT NULL DEFAULT (N''),
	[shop_code] [nvarchar](9) NOT NULL DEFAULT (N''),
	[nissen_transaction_id] [nvarchar](11) NOT NULL DEFAULT (N''),
	[shop_transaction_id1] [nvarchar](20),
	[shop_transaction_id2] [nvarchar](20),
	[shop_message1] [nvarchar](60),
	[shop_message2] [nvarchar](60),
	[shop_message3] [nvarchar](60),
	[shop_message4] [nvarchar](60),
	[shop_message5] [nvarchar](60),
	[invoice_form] [nvarchar](1),
	[postal_account_number] [nvarchar](100) NOT NULL DEFAULT (N''),
	[postal_account_holder_name] [nvarchar](100) NOT NULL DEFAULT (N''),
	[postal_font_top_row] [nvarchar](39) NOT NULL DEFAULT (N''),
	[postal_font_bottom_row] [nvarchar](44) NOT NULL DEFAULT (N''),
	[remittance_address] [nvarchar](100) NOT NULL DEFAULT (N''),
	[x_symbol] [nvarchar](100) NOT NULL DEFAULT (N''),
	[reserve1] [nvarchar](100),
	[reserve2] [nvarchar](100),
	[reserve3] [nvarchar](100),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar](20) NOT NULL DEFAULT (N'')
	) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_InvoiceVeritrans] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_InvoiceVeritrans] PRIMARY KEY  CLUSTERED
	(
		[order_id]
	) ON [PRIMARY]
GO
