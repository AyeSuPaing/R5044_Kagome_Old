if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_Holidays]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_Holidays]
GO
/*
=========================================================================================================
  Module      : ЛxУ· (w2_Holidays.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_Holidays] (
	[year_month] [nvarchar] (6) NOT NULL,
	[days] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_Holidays] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_Holidays] PRIMARY KEY CLUSTERED
	(
		[year_month]
	) ON [PRIMARY]
GO
