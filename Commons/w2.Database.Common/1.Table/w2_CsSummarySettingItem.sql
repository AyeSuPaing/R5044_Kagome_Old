if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsSummarySettingItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsSummarySettingItem]
GO
/*
=========================================================================================================
  Module      : 集計区分アイテムマスタ(w2_CsSummarySettingItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsSummarySettingItem] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[summary_setting_no] [int] NOT NULL DEFAULT (1),
	[summary_setting_item_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[summary_setting_item_text] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (1),
	[valid_flg] [nvarchar] (10) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsSummarySettingItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsSummarySettingItem] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[summary_setting_no],
		[summary_setting_item_id]
	) ON [PRIMARY]
GO
