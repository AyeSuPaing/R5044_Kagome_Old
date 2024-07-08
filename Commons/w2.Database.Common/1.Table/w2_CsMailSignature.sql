if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsMailSignature]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsMailSignature]
GO
/*
=========================================================================================================
  Module      : メール署名マスタ(w2_CsMailSignature.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsMailSignature] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mail_signature_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[signature_title] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[signature_text] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[owner_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (1),
	[valid_flg] [nvarchar] (10) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsMailSignature] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsMailSignature] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[mail_signature_id]
	) ON [PRIMARY]
GO
