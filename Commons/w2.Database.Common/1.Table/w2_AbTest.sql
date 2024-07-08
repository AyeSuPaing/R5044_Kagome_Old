if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AbTest]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AbTest]
GO
/*
=========================================================================================================
  Module      : ABテスト (w2_AbTest.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_AbTest] (
	[ab_test_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[ab_test_title] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[public_status] [nvarchar] (20) NOT NULL DEFAULT (N'UNPUBLISHED'),
	[public_start_datetime] [datetime] NOT NULL DEFAULT (getdate()),
	[public_end_datetime] [datetime],
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AbTest] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AbTest] PRIMARY KEY  CLUSTERED
	(
		[ab_test_id]
	) ON [PRIMARY]
GO
