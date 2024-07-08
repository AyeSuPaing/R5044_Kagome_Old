if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_OgpTagSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_OgpTagSetting]
GO
/*
=========================================================================================================
  Module      : OGPГ^ГOР▌Тш (w2_OgpTagSetting.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_OgpTagSetting] (
	[data_kbn] [nvarchar] (25) NOT NULL,
	[site_title] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[page_title] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[description] [nvarchar] (120) NOT NULL DEFAULT (N''),
	[image_url] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_OgpTagSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_OgpTagSetting] PRIMARY KEY  CLUSTERED
	(
		[data_kbn]
	) ON [PRIMARY]
GO
