if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsOperator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsOperator]
GO
/*
=========================================================================================================
  Module      : CSオペレータマスタ(w2_CsOperator.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsOperator] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[operator_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[operator_authority_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mail_from_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[notify_info_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[notify_warn_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[mail_addr] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (1),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsOperator] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsOperator] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[operator_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_CsOperator_1] ON [dbo].[w2_CsOperator]([display_order]) ON [PRIMARY]
GO
