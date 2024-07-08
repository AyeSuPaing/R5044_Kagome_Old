/*
=========================================================================================================
  Module      : УdОqФнХ[ПюХё (w2_TwInvoice.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[w2_TwInvoice]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_TwInvoice]
GO

CREATE TABLE [dbo].[w2_TwInvoice] (
	[tw_invoice_id] [nvarchar] (50) NOT NULL,
	[tw_invoice_date_start] [datetime] NOT NULL,
	[tw_invoice_date_end] [datetime] NOT NULL,
	[tw_invoice_code] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[tw_invoice_type_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[tw_invoice_code_name] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[tw_invoice_no_start] [decimal] NOT NULL,
	[tw_invoice_no_end] [decimal] NOT NULL,
	[tw_invoice_no] [decimal],
	[tw_invoice_alert_count] [int] NOT NULL DEFAULT (100),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_TwInvoice] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_TwInvoice] PRIMARY KEY  CLUSTERED
	(
		[tw_invoice_id]
	) ON [PRIMARY]

/*
IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'w2_TwInvoice' AND COLUMN_NAME = 'tw_invoice_alert_count')
	ALTER TABLE [w2_TwInvoice] ADD [tw_invoice_alert_count] [int] NOT NULL DEFAULT (100)
GO
*/