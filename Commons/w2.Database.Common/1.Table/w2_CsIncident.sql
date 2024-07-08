if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsIncident]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsIncident]
GO
/*
=========================================================================================================
  Module      : インシデントマスタ(w2_CsIncident.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsIncident] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[incident_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[incident_category_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[incident_title] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[status] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[voc_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[voc_memo] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[comment] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[importance] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[user_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[user_contact] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[cs_group_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[operator_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[lock_status] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[lock_operator_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[valid_flg] [nvarchar] (10) NOT NULL DEFAULT (N'1'),
	[date_last_received] [datetime] NOT NULL DEFAULT (getdate()),
	[date_completed] [datetime],
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsIncident] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsIncident] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[incident_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_CsIncident_1] ON [dbo].[w2_CsIncident]([user_id]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_CsIncident_2] ON [dbo].[w2_CsIncident]([dept_id], [status]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_CsIncident_3] ON [dbo].[w2_CsIncident]([dept_id], [valid_flg], [operator_id]) ON [PRIMARY]
GO
