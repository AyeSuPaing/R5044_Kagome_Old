/*
=========================================================================================================
  Module      : ユーザ電子発票管理情報 (w2_TwWorkUserInvoice.sql)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[w2_TwWorkUserInvoice]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [dbo].[w2_TwWorkUserInvoice] (
		[user_id] [nvarchar] (30) NOT NULL,
		[tw_invoice_no] [int] NOT NULL DEFAULT (1),
		[tw_invoice_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
		[tw_uniform_invoice] [nvarchar] (10) NOT NULL DEFAULT (N'PERSONAL'),
		[tw_uniform_invoice_option1] [nvarchar] (10) NOT NULL DEFAULT (N''),
		[tw_uniform_invoice_option2] [nvarchar] (20) NOT NULL DEFAULT (N''),
		[tw_carry_type] [nvarchar] (16) NOT NULL DEFAULT (N''),
		[tw_carry_type_option] [nvarchar] (16) NOT NULL DEFAULT (N''),
		[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
		[date_changed] [datetime] NOT NULL DEFAULT (GETDATE())
	) ON [PRIMARY]

	ALTER TABLE [dbo].[w2_TwWorkUserInvoice] WITH NOCHECK ADD
		CONSTRAINT [PK_w2_TwWorkUserInvoice] PRIMARY KEY  CLUSTERED
		(
			[user_id],
			[tw_invoice_no]
		) ON [PRIMARY]
END