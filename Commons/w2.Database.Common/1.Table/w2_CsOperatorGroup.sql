if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsOperatorGroup]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsOperatorGroup]
GO
/*
=========================================================================================================
  Module      : CSオペレータ所属グループ(w2_CsOperatorGroup.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsOperatorGroup] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cs_group_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[operator_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsOperatorGroup] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsOperatorGroup] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[cs_group_id],
		[operator_id]
	) ON [PRIMARY]
GO
