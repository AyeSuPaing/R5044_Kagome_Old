if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_WorkStaff]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_WorkStaff]
GO
/*
=========================================================================================================
  Module      : ГXГ^ГbГt (w2_WorkStaff.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_WorkStaff] (
	[staff_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[staff_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[staff_profile] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[staff_height] [int] NOT NULL DEFAULT (0),
	[staff_instagram_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[staff_sex] [nvarchar] (10) NOT NULL DEFAULT (N'UNKNOWN'),
	[model_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[operator_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[real_shop_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkStaff] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkStaff] PRIMARY KEY  CLUSTERED
	(
		[staff_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_WorkStaff_1] ON [dbo].[w2_WorkStaff]([staff_height], [staff_type], [real_shop_id]) ON [PRIMARY]
GO
