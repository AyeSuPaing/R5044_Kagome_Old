if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsIncidentSummaryValue]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsIncidentSummaryValue]
GO
/*
=========================================================================================================
  Module      : インシデント集計区分値(w2_CsIncidentSummaryValue.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsIncidentSummaryValue] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[incident_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[summary_no] [int] NOT NULL DEFAULT (1),
	[value] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsIncidentSummaryValue] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsIncidentSummaryValue] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[incident_id],
		[summary_no]
	) ON [PRIMARY]
GO
