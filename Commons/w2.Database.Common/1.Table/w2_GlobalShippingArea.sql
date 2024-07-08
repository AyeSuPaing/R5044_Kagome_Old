if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_GlobalShippingArea]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_GlobalShippingArea]
GO
/*
=========================================================================================================
  Module      : 海外配送エリア (w2_GlobalShippingArea.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_GlobalShippingArea] (
	[global_shipping_area_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[global_shipping_area_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[sort_no] [int] NOT NULL DEFAULT (0),
	[valid_flg] [nvarchar] NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_GlobalShippingArea] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_GlobalShippingArea] PRIMARY KEY  CLUSTERED
	(
		[global_shipping_area_id]
	) ON [PRIMARY]
GO
