if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsOperatorAuthority]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsOperatorAuthority]
GO
/*
=========================================================================================================
  Module      : CSオペレータ権限マスタ (w2_CsOperatorAuthority.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsOperatorAuthority] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[operator_authority_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[operator_authority_name] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[permit_edit_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[permit_mail_send_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[permit_approval_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[permit_unlock_flg] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[permit_edit_signature_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[receive_no_assign_warning_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[permit_permanent_delete_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsOperatorAuthority] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsOperatorAuthority] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[operator_authority_id]
	) ON [PRIMARY]
GO
