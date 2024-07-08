if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MallCooperationUpdateLog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MallCooperationUpdateLog]
GO
/*
=========================================================================================================
  Module      : モール連携更新ログ情報(w2_MallCooperationUpdateLog.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MallCooperationUpdateLog] (
	[log_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mall_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[master_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_kbn] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[action_status] [nvarchar] (2) NOT NULL DEFAULT (N'00'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MallCooperationUpdateLog] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MallCooperationUpdateLog] PRIMARY KEY  NONCLUSTERED
	(
		[log_no]
	) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_MallCooperationUpdateLog_1] ON [dbo].[w2_MallCooperationUpdateLog]([shop_id], [mall_id], [master_kbn], [date_created]) ON [PRIMARY]
GO
