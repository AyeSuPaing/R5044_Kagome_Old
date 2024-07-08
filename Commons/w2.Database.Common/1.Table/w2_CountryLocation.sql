if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CountryLocation]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CountryLocation]
GO
/*
=========================================================================================================
  Module      : 国ISOコードのマスタテーブル (w2_CountryLocation.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CountryLocation] (
	[geoname_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[country_iso_code] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[country_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CountryLocation] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CountryLocation] PRIMARY KEY  CLUSTERED
	(
		[geoname_id]
	) ON [PRIMARY]
GO
