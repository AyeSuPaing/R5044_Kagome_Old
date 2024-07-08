if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_DispProductInfo]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_DispProductInfo]
GO
/*
=========================================================================================================
  Module      : 商品表示情報マスタ(w2_DispProductInfo.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_DispProductInfo] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[data_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (0),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[kbn1] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[kbn2] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[kbn3] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[kbn4] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[kbn5] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_DispProductInfo] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_DispProductInfo] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[data_kbn],
		[display_order],
		[product_id]
	) ON [PRIMARY]
GO
