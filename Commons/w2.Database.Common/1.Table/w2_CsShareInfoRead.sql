if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsShareInfoRead]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsShareInfoRead]
GO
/*
=========================================================================================================
  Module      : 共有情報既読管理マスタ(w2_CsShareInfoRead.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsShareInfoRead] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[info_no] [bigint] NOT NULL DEFAULT (1),
	[operator_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[read_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[pinned_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsShareInfoRead] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsShareInfoRead] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[info_no],
		[operator_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_CsShareInfoRead_1] ON [dbo].[w2_CsShareInfoRead]([operator_id], [read_flg]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_CsShareInfoRead_2] ON [dbo].[w2_CsShareInfoRead]([operator_id], [pinned_flg]) ON [PRIMARY]
GO
