/*
=========================================================================================================
  Module      : User credit card work table(w2_WorkUserCreditCard.sql)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[w2_WorkUserCreditCard]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_WorkUserCreditCard]
GO

CREATE TABLE [dbo].[w2_WorkUserCreditCard] (
	[user_id] [nvarchar] (30) NOT NULL,
	[branch_no] [int] NOT NULL DEFAULT (1),
	[cooperation_id] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[card_disp_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[last_four_digit] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[expiration_month] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[expiration_year] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[author_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[disp_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[company_code] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cooperation_id2] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[register_action_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[register_status] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[register_target_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[before_order_status] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cooperation_type] [nvarchar] (10) NOT NULL DEFAULT (N'CREDIT')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkUserCreditCard] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkUserCreditCard] PRIMARY KEY CLUSTERED
	(
		[user_id],
		[branch_no]
	) ON [PRIMARY]
GO
