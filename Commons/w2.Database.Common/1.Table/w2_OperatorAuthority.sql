/*
=========================================================================================================
  Module      : オペレータ権限テーブル (w2_OperatorAuthority.sql)
  ････････････････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[w2_OperatorAuthority]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[w2_OperatorAuthority]
GO

CREATE TABLE [dbo].[w2_OperatorAuthority] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[operator_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[condition_type] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[permission] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[condition_value] [nvarchar] (30) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_OperatorAuthority] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_OperatorAuthority] PRIMARY KEY CLUSTERED
	(
		[shop_id] ASC,
		[operator_id] ASC,
		[condition_value] ASC
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_OperatorAuthority_1] ON [dbo].[w2_OperatorAuthority]([shop_id], [operator_id], [condition_type]) ON [PRIMARY]
GO
