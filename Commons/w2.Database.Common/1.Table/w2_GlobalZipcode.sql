IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[w2_GlobalZipcode]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_GlobalZipcode]
GO
/*
=========================================================================================================
  Module      : Global Zipcode (w2_GlobalZipcode.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_GlobalZipcode] (
	[country_iso_code] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[zipcode] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[country] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[province] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[city] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[address] [nvarchar] (200) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_GlobalZipcode_1] ON [dbo].[w2_GlobalZipcode]([country_iso_code], [zipcode]) ON [PRIMARY]
GO
