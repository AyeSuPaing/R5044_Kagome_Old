if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsMailAssignSettingItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsMailAssignSettingItem]
GO
/*
=========================================================================================================
  Module      : 受信メール振分設定アイテム(w2_CsMailAssignSettingItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsMailAssignSettingItem] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mail_assign_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[item_no] [int] NOT NULL DEFAULT (1),
	[matching_target] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[matching_value] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[matching_type] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[ignore_case] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsMailAssignSettingItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsMailAssignSettingItem] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[mail_assign_id],
		[item_no]
	) ON [PRIMARY]
GO
