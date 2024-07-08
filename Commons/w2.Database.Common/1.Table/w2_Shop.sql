if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_Shop]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_Shop]
GO
/*
=========================================================================================================
  Module      : УXХ▄Г}ГXГ^(w2_Shop.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_Shop] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[corporation_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[name_kana] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[desc_short] [nvarchar] (255) NOT NULL DEFAULT (N''),
	[desc_medium] [ntext] NOT NULL DEFAULT (N''),
	[desc_long] [ntext] NOT NULL DEFAULT (N''),
	[zip] [nvarchar] (8) NOT NULL DEFAULT (N''),
	[zip1] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[zip2] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[addr] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[addr1] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[addr2] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[addr3] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[addr4] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[tel] [nvarchar] (16),
	[tel_1] [nvarchar] (6),
	[tel_2] [nvarchar] (4),
	[tel_3] [nvarchar] (4),
	[fax] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[fax_1] [nvarchar] (6) NOT NULL DEFAULT (N''),
	[fax_2] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[fax_3] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[url] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[mail_addr] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[mail_addr_batch] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_Shop] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_Shop] PRIMARY KEY  CLUSTERED
	(
		[shop_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_Shop_1] ON [dbo].[w2_Shop]([name]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_Shop_2] ON [dbo].[w2_Shop]([name_kana]) ON [PRIMARY]
GO
