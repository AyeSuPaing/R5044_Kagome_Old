if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MobileModelUserAgent]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MobileModelUserAgent]
GO
/*
=========================================================================================================
  Module      : モバイルユーザーエージェントマスタ(w2_MobileModelUserAgent.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MobileModelUserAgent] (
	[career] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[model_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[device_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[user_agent] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[user_agent_scrap] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[info_update_date] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MobileModelUserAgent] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MobileModelUserAgent] PRIMARY KEY  CLUSTERED
	(
		[career],
		[model_name],
		[device_name],
		[user_agent],
		[user_agent_scrap]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_MobileModelUserAgent_1] ON [dbo].[w2_MobileModelUserAgent]([user_agent], [user_agent_scrap]) ON [PRIMARY]
GO
