if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsMailAssignSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsMailAssignSetting]
GO
/*
=========================================================================================================
  Module      : ОєРMГББ[ГЛРUХкР▌Тш(w2_CsMailAssignSetting.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsMailAssignSetting] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mail_assign_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[assign_priority] [int] NOT NULL DEFAULT (1),
	[logical_operator] [nvarchar] (10) NOT NULL DEFAULT (N'AND'),
	[stop_filtering] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[assign_incident_category_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[assign_operator_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[assign_cs_group_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[assign_importance] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[assign_status] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[trash] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[auto_response] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[auto_response_from] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[auto_response_cc] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[auto_response_bcc] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[auto_response_subject] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[auto_response_body] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[valid_flg] [nvarchar] (max) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[mail_assign_name] [nvarchar] (256) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsMailAssignSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsMailAssignSetting] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[mail_assign_id]
	) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_CsMailAssignSetting] ADD [mail_assign_name] [nvarchar] (256) NOT NULL DEFAULT (N'');
*/