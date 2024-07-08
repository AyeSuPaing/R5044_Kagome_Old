IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[w2_DailyOrderShipmentForecast]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_DailyOrderShipmentForecast]
GO
/*
=========================================================================================================
  Module      : ПoЙ╫РФПWМv (w2_DailyOrderShipmentForecast.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_DailyOrderShipmentForecast] (
	[shipment_date] [datetime] NOT NULL,
	[shipment_order_count] [bigint] NOT NULL DEFAULT (0),
	[total_order_price_subtotal] [decimal] (18,3) NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar](20) NOT NULL DEFAULT ('N')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_DailyOrderShipmentForecast] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_DailyOrderShipmentForecast] PRIMARY KEY  CLUSTERED
	(
		[shipment_date]
	) ON [PRIMARY]
GO
