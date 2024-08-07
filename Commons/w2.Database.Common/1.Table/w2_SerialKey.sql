/*
=========================================================================================================
  Module      : シリアルキー情報(w2_SerialKey.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 20 All Rights Reserved.
=========================================================================================================
*/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_SerialKey]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_SerialKey]
GO

CREATE TABLE [dbo].[w2_SerialKey] (
	[serial_key] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[order_item_no] [int] NOT NULL DEFAULT (0),
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[status] [nvarchar] (20) NOT NULL DEFAULT (N'NOTRESERVED'),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[date_delivered] [datetime],
	[download_count] [int] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_SerialKey] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_SerialKey] PRIMARY KEY  CLUSTERED
	(
		[serial_key],
		[product_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_SerialKey_1] ON [dbo].[w2_SerialKey]([order_id]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_SerialKey_2] ON [dbo].[w2_SerialKey]([product_id], [variation_id]) ON [PRIMARY]
GO
