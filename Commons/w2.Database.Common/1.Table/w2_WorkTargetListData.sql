if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_WorkTargetListData]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_WorkTargetListData]
GO
/*
=========================================================================================================
  Module      : ターゲットリストデータ(w2_WorkTargetListData.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_WorkTargetListData](
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

ALTER TABLE [dbo].w2_WorkTargetListData WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkTargetListData] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[target_kbn],
		[master_id],
		[data_no]
	) ON [PRIMARY]
GO