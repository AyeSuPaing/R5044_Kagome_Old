if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductExtendSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ProductExtendSetting]
GO
/*
=========================================================================================================
  Module      : 商品拡張項目設定マスタ(w2_ProductExtendSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ProductExtendSetting] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend_no] [int] NOT NULL DEFAULT (1),
	[extend_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend_discription] [ntext] NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ProductExtendSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ProductExtendSetting] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[extend_no]
	) ON [PRIMARY]
GO
