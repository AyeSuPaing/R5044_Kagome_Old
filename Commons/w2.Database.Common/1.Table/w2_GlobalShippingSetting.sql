if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_GlobalShippingSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_GlobalShippingSetting]
GO
/*
=========================================================================================================
  Module      : 海外配送先設定テーブル (w2_GlobalShippingSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_GlobalShippingSetting](
	[country_iso_code] [nvarchar](3) NOT NULL,
	[shipping_availavle_flg] [nvarchar](1) NOT NULL DEFAULT (N'0'),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_GlobalShippingSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_GlobalShippingSetting] PRIMARY KEY  NONCLUSTERED
	(
		[country_iso_code]
	) ON [PRIMARY]
GO