if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_Point]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_Point]
GO
/*
=========================================================================================================
  Module      : ポイントマスタ(w2_Point.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_Point] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[point_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'01'),
	[point_exp_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'01'),
	[usable_unit] [bigint] NOT NULL DEFAULT (1),
	[exchange_rate] [decimal] (18,3) NOT NULL DEFAULT (1),
	[kbn1] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[kbn2] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[kbn3] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[kbn4] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[kbn5] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_Point] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_Point] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[point_kbn]
	) ON [PRIMARY]
GO

/*
■ 決済通貨対応
ALTER TABLE [w2_Point] ALTER COLUMN [exchange_rate] [decimal] (18,3) NOT NULL;
*/