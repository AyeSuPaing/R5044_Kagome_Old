if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AbTestItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AbTestItem]
GO
/*
=========================================================================================================
  Module      : ABテストアイテム (w2_AbTestItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_AbTestItem] (
	[ab_test_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[item_no] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[page_id] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[distribution_rate] [int] NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AbTestItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AbTestItem] PRIMARY KEY  CLUSTERED
	(
		[ab_test_id],
		[item_no]
	) ON [PRIMARY]
GO
