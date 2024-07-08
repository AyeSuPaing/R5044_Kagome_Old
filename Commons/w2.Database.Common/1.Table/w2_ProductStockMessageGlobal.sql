if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductStockMessageGlobal]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductStockMessageGlobal]
GO
/*
=========================================================================================================
  Module      : 商品在庫文言グローバルマスタ (w2_ProductStockMessageGlobal.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductStockMessageGlobal] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[stock_message_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[language_code] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[language_locale_id] [nvarchar] (7) NOT NULL DEFAULT (N''),
	[stock_message_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message_def] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message4] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message5] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message6] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[stock_message7] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductStockMessageGlobal] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductStockMessageGlobal] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[stock_message_id],
		[language_code],
		[language_locale_id]
	) ON [PRIMARY]
GO
