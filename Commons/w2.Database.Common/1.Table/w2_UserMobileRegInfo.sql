if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserMobileRegInfo]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserMobileRegInfo]
GO
/*
=========================================================================================================
  Module      : ГЖБ[ГUГВГoГCГЛУoШ^ПєЛ╡(w2_UserMobileRegInfo.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserMobileRegInfo] (
	[career_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mobile_uid] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[site_id] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[reg_flg] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[attribute1] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute2] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute3] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute4] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute5] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[date_reg] [datetime],
	[date_unreg] [datetime],
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserMobileRegInfo] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserMobileRegInfo] PRIMARY KEY  CLUSTERED
	(
		[career_id],
		[mobile_uid],
		[site_id]
	) ON [PRIMARY]
GO
