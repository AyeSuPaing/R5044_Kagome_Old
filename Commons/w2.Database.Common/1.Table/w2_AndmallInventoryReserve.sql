if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AndmallInventoryReserve]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AndmallInventoryReserve]
GO
/*
=========================================================================================================
  Module      : БХmallН▌М╔И°УЦ (w2_AndmallInventoryReserve.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_AndmallInventoryReserve] (
	[identification_code] [nvarchar] (32) NOT NULL,
	[sku_id] [nvarchar] (30) NOT NULL,
	[andmall_base_store_code] [nvarchar] (8) NOT NULL,
	[product_id] [nvarchar] (30) NOT NULL,
	[variation_id] [nvarchar] (60) NOT NULL,
	[quantity] [int] NOT NULL DEFAULT (0),
	[status] [nvarchar] (20) NOT NULL,
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[cancel_date] [datetime]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AndmallInventoryReserve] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AndmallInventoryReserve] PRIMARY KEY  CLUSTERED
	(
		[identification_code],
		[sku_id],
		[andmall_base_store_code]
	) ON [PRIMARY]
GO
