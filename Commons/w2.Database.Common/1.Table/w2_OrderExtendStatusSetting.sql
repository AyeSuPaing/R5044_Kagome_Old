 if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_OrderExtendStatusSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_OrderExtendStatusSetting]
GO
/*
=========================================================================================================
  Module      : 注文拡張ステータス(w2_OrderExtendStatusSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_OrderExtendStatusSetting] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend_status_no] [int] NOT NULL DEFAULT (1),
	[extend_status_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[extend_status_discription] [ntext] NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_OrderExtendStatusSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_OrderExtendStatusSetting] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[extend_status_no]
	) ON [PRIMARY]
GO
