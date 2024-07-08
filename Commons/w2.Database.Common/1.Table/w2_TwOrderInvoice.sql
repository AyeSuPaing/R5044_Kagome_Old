/*
=========================================================================================================
  Module      : ТНХ╢УdОqФнХ[ПюХё (w2_TwOrderInvoice.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[w2_TwOrderInvoice]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[w2_TwOrderInvoice] (
		[order_id] [nvarchar] (30) NOT NULL,
		[order_shipping_no] [int] NOT NULL DEFAULT (1),
		[tw_uniform_invoice] [nvarchar] (10) NOT NULL DEFAULT (N'PERSONAL'),
		[tw_uniform_invoice_option1] [nvarchar] (10) NOT NULL DEFAULT (N''),
		[tw_uniform_invoice_option2] [nvarchar] (20) NOT NULL DEFAULT (N''),
		[tw_carry_type] [nvarchar] (16) NOT NULL DEFAULT (N''),
		[tw_carry_type_option] [nvarchar] (16) NOT NULL DEFAULT (N''),
		[tw_invoice_no] [nvarchar] (16) NOT NULL DEFAULT (N''),
		[tw_invoice_date] [datetime],
		[tw_invoice_status] [nvarchar] (2) NOT NULL DEFAULT (N''),
		[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
		[date_changed] [datetime] NOT NULL DEFAULT (GETDATE())
	) ON [PRIMARY]

	ALTER TABLE [dbo].[w2_TwOrderInvoice] WITH NOCHECK ADD
		CONSTRAINT [PK_w2_TwOrderInvoice] PRIMARY KEY  CLUSTERED
		(
			[order_id],
			[order_shipping_no]
		) ON [PRIMARY]

	CREATE INDEX [IX_w2_TwOrderInvoice_1] ON [dbo].[w2_TwOrderInvoice]([tw_invoice_status]) ON [PRIMARY]
END
