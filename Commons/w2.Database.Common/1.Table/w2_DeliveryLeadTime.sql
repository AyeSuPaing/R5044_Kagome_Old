/*
=========================================================================================================
  Module      : 配送リードタイムマスタ (w2_DeliveryLeadTime.sql)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[w2_DeliveryLeadTime]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_DeliveryLeadTime]
GO

CREATE TABLE [dbo].[w2_DeliveryLeadTime] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[delivery_company_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[lead_time_zone_no] [int] NOT NULL DEFAULT (N'0'),
	[lead_time_zone_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[zip] [ntext] NOT NULL DEFAULT (N''),
	[shipping_lead_time] [int] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_DeliveryLeadTime] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_DeliveryLeadTime] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[delivery_company_id],
		[lead_time_zone_no]
	) ON [PRIMARY]
GO