if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MobilePage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MobilePage]
GO
/*
=========================================================================================================
  Module      : モバイルページマスタ(w2_MobilePage.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MobilePage] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[page_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[page_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[career_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mobile_group_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[title] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[html] [ntext] NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[brand_id] [nvarchar] (30) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MobilePage] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MobilePage] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[page_id],
		[career_id],
		[mobile_group_id],
		[brand_id]
	) ON [PRIMARY]
GO
