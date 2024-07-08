if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_GlobalShippingAreaComponent]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_GlobalShippingAreaComponent]
GO
/*
=========================================================================================================
  Module      : 海外配送エリア構成 (w2_GlobalShippingAreaComponent.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_GlobalShippingAreaComponent] (
	[seq] [bigint] IDENTITY (1, 1) NOT NULL,
	[global_shipping_area_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[country_iso_code] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[condition_addr5] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[condition_addr4] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[condition_addr3] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[condition_addr2] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[condition_zip] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_GlobalShippingAreaComponent] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_GlobalShippingAreaComponent] PRIMARY KEY  CLUSTERED
	(
		[seq]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_GlobalShippingAreaComponent_1] ON [dbo].[w2_GlobalShippingAreaComponent]([global_shipping_area_id]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_GlobalShippingAreaComponent_2] ON [dbo].[w2_GlobalShippingAreaComponent]([country_iso_code]) ON [PRIMARY]
GO
