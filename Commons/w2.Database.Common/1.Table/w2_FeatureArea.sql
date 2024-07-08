if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_FeatureArea]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_FeatureArea]
GO
/*
=========================================================================================================
  Module      : 特集エリアマスタ (w2_FeatureArea.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_FeatureArea] (
	[area_id] [nvarchar] (30) NOT NULL,
	[area_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[area_type_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[internal_memo] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[side_max_count] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[side_turn] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[slider_count] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[slider_scroll_count] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[slider_scroll_auto] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[slider_scroll_interval] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[slider_arrow] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[slider_dot] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FeatureArea] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FeatureArea] PRIMARY KEY  CLUSTERED
	(
		[area_id]
	) ON [PRIMARY]
GO
