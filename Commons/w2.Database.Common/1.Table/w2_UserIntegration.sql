if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserIntegration]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserIntegration]
GO
/*
=========================================================================================================
  Module      : ГЖБ[ГUБ[УЭНЗПюХё (w2_UserIntegration.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserIntegration] (
	[user_integration_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[status] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserIntegration] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserIntegration] PRIMARY KEY  CLUSTERED
	(
		[user_integration_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_UserIntegration_1] ON [dbo].[w2_UserIntegration]([status]) ON [PRIMARY]
GO