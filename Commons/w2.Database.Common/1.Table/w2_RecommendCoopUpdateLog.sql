if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_RecommendCoopUpdateLog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_RecommendCoopUpdateLog]
GO
/*
=========================================================================================================
  Module      : 外部レコメンド連携更新ログマスタ(w2_RecommendCoopUpdateLog.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_RecommendCoopUpdateLog] (
	[log_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[master_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[master_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[action_status] [nvarchar] (10) NOT NULL DEFAULT (N'READY'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_RecommendCoopUpdateLog] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_RecommendCoopUpdateLog] PRIMARY KEY  NONCLUSTERED
	(
		[log_no]
	) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_RecommendCoopUpdateLog_1] ON [dbo].[w2_RecommendCoopUpdateLog]([shop_id], [master_kbn], [master_id]) ON [PRIMARY]
GO
