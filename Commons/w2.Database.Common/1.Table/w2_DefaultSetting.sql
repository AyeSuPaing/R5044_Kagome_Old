IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[w2_DefaultSetting]'))
	DROP TABLE [dbo].[w2_DefaultSetting]
GO
/*
=========================================================================================================
  Module      : Default Setting (w2_DefaultSetting.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_DefaultSetting] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[classification] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[init_data] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]

ALTER TABLE [dbo].[w2_DefaultSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_DefaultSetting] PRIMARY KEY CLUSTERED
	(
		[shop_id],
		[classification]
	) ON [PRIMARY]
GO
