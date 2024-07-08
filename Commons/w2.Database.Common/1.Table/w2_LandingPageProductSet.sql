if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_LandingPageProductSet]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_LandingPageProductSet]
GO
/*
=========================================================================================================
  Module      : Lpページ商品セット (w2_LandingPageProductSet.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_LandingPageProductSet] (
	[page_id] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[branch_no] [int] NOT NULL DEFAULT (0),
	[set_name] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[subscription_box_course_id] [nvarchar] (30) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_LandingPageProductSet] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_LandingPageProductSet] PRIMARY KEY  CLUSTERED
	(
		[page_id],
		[branch_no]
	) ON [PRIMARY]
GO
