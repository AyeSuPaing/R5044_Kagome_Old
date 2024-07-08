/*
=========================================================================================================
  Module      : Cs incident work table(w2_WorkCsIncident.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[w2_WorkCsIncident]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_WorkCsIncident]
GO

CREATE TABLE [dbo].[w2_WorkCsIncident] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[incident_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[incident_category_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[incident_title] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[status] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[voc_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[voc_memo] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[comment] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[importance] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[user_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[user_contact] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[cs_group_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[operator_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[lock_status] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[lock_operator_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[valid_flg] [nvarchar] (10) NOT NULL DEFAULT (N'1'),
	[date_last_received] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_completed] [datetime],
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[date_message_last_send_received] [datetime] NOT NULL DEFAULT (GETDATE())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkCsIncident] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkCsIncident] PRIMARY KEY CLUSTERED
	(
		[dept_id],
		[incident_id]
	) ON [PRIMARY]
GO
