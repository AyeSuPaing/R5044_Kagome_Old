﻿if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UpdateHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UpdateHistory]
GO
/*
=========================================================================================================
  Module      : 更新履歴情報 (w2_UpdateHistory.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UpdateHistory] (
	[update_history_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[update_history_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[master_id] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[update_history_action] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[update_data] [varbinary] (max) NOT NULL,
	[update_data_hash] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UpdateHistory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UpdateHistory] PRIMARY KEY  CLUSTERED
	(
		[update_history_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_UpdateHistory_1] ON [dbo].[w2_UpdateHistory]([update_history_kbn], [user_id], [master_id], [update_history_no]) ON [PRIMARY]
GO
