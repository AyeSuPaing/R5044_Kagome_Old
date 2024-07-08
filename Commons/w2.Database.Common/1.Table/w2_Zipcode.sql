if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_Zipcode]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_Zipcode]
GO
/*
=========================================================================================================
  Module      : ЧXХ╓Ф╘НЖГ}ГXГ^(w2_Zipcode.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_Zipcode] (
	[local_pub_code] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[zipcode_old] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[zipcode] [nvarchar] (7) NOT NULL DEFAULT (N''),
	[prefecture_kana] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[city_kana] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[town_kana] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[prefecture] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[city] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[town] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[flg1] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[flg2] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[flg3] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[flg4] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[flg5] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[flg6] [nvarchar] (1) NOT NULL DEFAULT (N'0')
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_Zipcode_1] ON [dbo].[w2_Zipcode]([zipcode]) ON [PRIMARY]
GO
