if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CountryIpv4]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CountryIpv4]
GO
/*
=========================================================================================================
  Module      : リージョン判定IP範囲テーブル (w2_CountryIpv4.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CountryIpv4] (
	[ip_numeric] [int] NOT NULL DEFAULT (0),
	[ip_broadcast_numeric] [int] NOT NULL DEFAULT (0),
	[ip] [nvarchar] (15) NOT NULL DEFAULT (N''),
	[ip_broadcast] [nvarchar] (15) NOT NULL DEFAULT (N''),
	[geoname_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CountryIpv4] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CountryIpv4] PRIMARY KEY  CLUSTERED
	(
		[ip_numeric],
		[ip_broadcast_numeric]
	) ON [PRIMARY]
GO