if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserManagementLevel]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserManagementLevel]
GO

/*
=========================================================================================================
  Module      : ユーザー管理レベルマスタ(w2_UserManagementLevel.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserManagementLevel] (
	[seq_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[user_management_level_id] [nvarchar] (30) NOT NULL,
	[user_management_level_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (1),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserManagementLevel] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserManagementLevel] PRIMARY KEY  CLUSTERED
	(
		[user_management_level_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_UserManagementLevel_1] ON [dbo].[w2_UserManagementLevel]([display_order]) ON [PRIMARY]
GO
