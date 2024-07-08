if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_WorkCoordinate]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_WorkCoordinate]
GO
/*
=========================================================================================================
  Module      : ワークコーディネートマスタ (w2_WorkCoordinate.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_WorkCoordinate] (
	[coordinate_id] [nvarchar] (30) NOT NULL,
	[coordinate_title] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[coordinate_url] [nvarchar] (2000) NOT NULL DEFAULT (N''),
	[coordinate_summary] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[internal_memo] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[staff_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[real_shop_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[html_title] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[metadata_desc] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[display_kbn] [nvarchar] (20) NOT NULL DEFAULT (N'DRAFT'),
	[display_date] [datetime],
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkCoordinate] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkCoordinate] PRIMARY KEY  CLUSTERED
	(
		[coordinate_id]
	) ON [PRIMARY]
GO