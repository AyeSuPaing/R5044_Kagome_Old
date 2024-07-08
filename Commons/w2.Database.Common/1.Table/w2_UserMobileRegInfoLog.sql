if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserMobileRegInfoLog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserMobileRegInfoLog]
GO
/*
=========================================================================================================
  Module      : ГЖБ[ГUГВГoГCГЛУoШ^ПєЛ╡ГНГO(w2_UserMobileRegInfoLog.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserMobileRegInfoLog] (
	[log_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[reg_date] [nvarchar] (20) NOT NULL DEFAULT (''),
	[reg_time] [nvarchar] (20) NOT NULL DEFAULT (''),
	[career_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mobile_uid] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[site_id] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[reg_flg] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[attribute1] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute2] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute3] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute4] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute5] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[log_no_regist] [bigint],
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserMobileRegInfoLog] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserMobileRegInfoLog] PRIMARY KEY  NONCLUSTERED
	(
		[log_no]
	) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_UserMobileRegInfoLog_1] ON [dbo].[w2_UserMobileRegInfoLog]([reg_date], [reg_time]) ON [PRIMARY]
GO
