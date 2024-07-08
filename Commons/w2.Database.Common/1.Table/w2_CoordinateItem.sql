﻿if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CoordinateItem]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CoordinateItem]
GO
/*
=========================================================================================================
  Module      : コーディネートアイテム (w2_CoordinateItem.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CoordinateItem] (
	[coordinate_id] [nvarchar] (30) NOT NULL,
	[item_no] [int] IDENTITY (1, 1) NOT NULL,
	[item_kbn] [nvarchar] (15) NOT NULL DEFAULT (N''),
	[item_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[item_id2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[item_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[date_created] [datetime] DEFAULT (getdate()),
	[date_changed] [datetime] DEFAULT (getdate()),
	[last_changed] [nvarchar](20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CoordinateItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CoordinateItem] PRIMARY KEY  CLUSTERED
	(
		[coordinate_id],
		[item_no]
	) ON [PRIMARY]
GO