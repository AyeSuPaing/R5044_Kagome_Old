if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MobilePictorialSymbol]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MobilePictorialSymbol]
GO
/*
=========================================================================================================
  Module      : モバイル絵文字マスタ(w2_MobilePictorialSymbol.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MobilePictorialSymbol] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[symbol_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[symbol_tag] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[symbol_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[symbol_code1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[symbol_code2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[symbol_code3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[symbol_code4] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[symbol_code5] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[symbol_code6] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MobilePictorialSymbol] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MobilePictorialSymbol] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[symbol_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_MobilePictorialSymbol_1] ON [dbo].[w2_MobilePictorialSymbol]([symbol_tag]) ON [PRIMARY]
GO
