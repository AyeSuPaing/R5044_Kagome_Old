if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_PageDesignGroup]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_PageDesignGroup]
GO
/*
=========================================================================================================
  Module      : ページデザイン グループマスタ (w2_PageDesignGroup.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_PageDesignGroup] (
	[group_id] [bigint] IDENTITY (1, 1) NOT NULL,
	[group_name] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[group_sort_number] [int] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_PageDesignGroup] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_PageDesignGroup] PRIMARY KEY  CLUSTERED
	(
		[group_id]
	) ON [PRIMARY]
GO
