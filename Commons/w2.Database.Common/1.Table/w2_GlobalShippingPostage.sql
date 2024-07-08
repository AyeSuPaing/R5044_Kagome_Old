if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_GlobalShippingPostage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_GlobalShippingPostage]
GO
/*
=========================================================================================================
  Module      : КCКOФzСЧЧ┐ЛрХ\ (w2_GlobalShippingPostage.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_GlobalShippingPostage] (
	[seq] [bigint] IDENTITY (1, 1) NOT NULL,
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[shipping_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[delivery_company_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[global_shipping_area_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[weight_gram_greater_than_or_equal_to] [bigint] NOT NULL DEFAULT (0),
	[weight_gram_less_than] [bigint] NOT NULL DEFAULT (0),
	[global_shipping_postage] [decimal] (18,3) NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_GlobalShippingPostage] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_GlobalShippingPostage] PRIMARY KEY  CLUSTERED
	(
		[seq]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_GlobalShippingPostage_1] ON [dbo].[w2_GlobalShippingPostage]([shop_id], [shipping_id], [delivery_company_id], [global_shipping_area_id]) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_GlobalShippingPostage] ADD [delivery_company_id] [nvarchar] (10) NOT NULL DEFAULT (N'');

*/
