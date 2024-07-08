if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsIncidentWarningIcon]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsIncidentWarningIcon]
GO
/*
=========================================================================================================
  Module      : CSインシデント警告アイコン (w2_CsIncidentWarningIcon.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsIncidentWarningIcon] (
	[dept_id] [nvarchar] (30) NOT NULL,
	[operator_id] [nvarchar] (20) NOT NULL,
	[incident_status] [nvarchar] (10) NOT NULL,
	[warning_level] [nvarchar] (1) NOT NULL,
	[term] [int],
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsIncidentWarningIcon] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsIncidentWarningIcon] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[operator_id],
		[incident_status],
		[warning_level]
	) ON [PRIMARY]
GO
