/*
=========================================================================================================
  Module      : ТшК·НwУ№УdОqФнХ[ПюХё (w2_TwFixedPurchaseInvoice.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[w2_TwFixedPurchaseInvoice]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[w2_TwFixedPurchaseInvoice] (
		[fixed_purchase_id] [nvarchar] (30) NOT NULL,
		[fixed_purchase_shipping_no] [int] NOT NULL DEFAULT (1),
		[tw_uniform_invoice] [nvarchar] (10) NOT NULL DEFAULT (N'PERSONAL'),
		[tw_uniform_invoice_option1] [nvarchar] (10) NOT NULL DEFAULT (N''),
		[tw_uniform_invoice_option2] [nvarchar] (20) NOT NULL DEFAULT (N''),
		[tw_carry_type] [nvarchar] (16) NOT NULL DEFAULT (N''),
		[tw_carry_type_option] [nvarchar] (16) NOT NULL DEFAULT (N''),
		[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
		[date_changed] [datetime] NOT NULL DEFAULT (GETDATE())
	) ON [PRIMARY]

	ALTER TABLE [dbo].[w2_TwFixedPurchaseInvoice] WITH NOCHECK ADD
		CONSTRAINT [PK_w2_TwFixedPurchaseInvoice] PRIMARY KEY  CLUSTERED
		(
			[fixed_purchase_id],
			[fixed_purchase_shipping_no]
		) ON [PRIMARY]
END