if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductStockMessage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductStockMessage]
GO
/*
=========================================================================================================
  Module      : 商品在庫文言マスタ(w2_ProductStockMessage.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductStockMessage] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[stock_message_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message_def] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_datum1] [int],
	[stock_message1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_datum2] [int],
	[stock_message2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_datum3] [int],
	[stock_message3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_datum4] [int],
	[stock_message4] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_datum5] [int],
	[stock_message5] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_datum6] [int],
	[stock_message6] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_datum7] [int],
	[stock_message7] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_datum8] [int],
	[stock_message8] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_datum9] [int],
	[stock_message9] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[stock_message_def_mobile] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message_mobile1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message_mobile2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message_mobile3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message_mobile4] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message_mobile5] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message_mobile6] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message_mobile7] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message_mobile8] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message_mobile9] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[language_code] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[language_locale_id] [nvarchar] (7) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductStockMessage] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductStockMessage] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[stock_message_id]
	) ON [PRIMARY]
GO
