if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_TargetListData]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_TargetListData]
GO
/*
=========================================================================================================
  Module      : ターゲットリストデータ(w2_TargetListData.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_TargetListData] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[target_kbn] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[master_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[data_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mail_addr] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[mail_addr_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_TargetListData] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_TargetListData] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[target_kbn],
		[master_id],
		[data_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_TargetListData_1] ON [dbo].[w2_TargetListData] ([target_kbn],[user_id]) ON [PRIMARY]