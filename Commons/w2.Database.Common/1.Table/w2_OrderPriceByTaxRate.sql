if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[[w2_OrderPriceByTaxRate]]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_OrderPriceByTaxRate]
GO
/*
=========================================================================================================
  Module      : Р┼ЧжЦИВ╠ТНХ╢ЛрКzПюХё (w2_OrderPriceByTaxRate.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_OrderPriceByTaxRate] (
	[order_id] [nvarchar] (30) NOT NULL,
	[key_tax_rate] [decimal] (5,2) NOT NULL DEFAULT (0),
	[price_subtotal_by_rate] [decimal] NOT NULL DEFAULT (0),
	[price_shipping_by_rate] [decimal] NOT NULL DEFAULT (0),
	[price_payment_by_rate] [decimal] NOT NULL DEFAULT (0),
	[price_total_by_rate] [decimal] NOT NULL DEFAULT (0),
	[tax_price_by_rate] [decimal] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[return_price_correction_by_tax_rate] [decimal] NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_OrderPriceByTaxRate] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_OrderPriceByTaxRate] PRIMARY KEY  CLUSTERED
	(
		[order_id],
		[key_tax_rate]
	) ON [PRIMARY]
GO