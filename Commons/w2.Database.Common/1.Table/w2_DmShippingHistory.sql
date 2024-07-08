/*
=========================================================================================================
  Module      : DMФнСЧЧЪЧЁГ}ГXГ^ (w2_DmShippingHistory.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_DmShippingHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_DmShippingHistory]
GO

CREATE TABLE [dbo].[w2_DmShippingHistory] (
	[user_id] [nvarchar] (30) NOT NULL,
	[dm_code] [nvarchar] (30) NOT NULL,
	[dm_shipping_date] [datetime] NOT NULL,
	[dm_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[valid_date_from] [datetime],
	[valid_date_to] [datetime],
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_DmShippingHistory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_DmShippingHistory] PRIMARY KEY  CLUSTERED
	(
		[user_id],
		[dm_code]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_DmShippingHistory_1] ON [dbo].[w2_DmShippingHistory]([dm_code]) ON [PRIMARY]
GO
