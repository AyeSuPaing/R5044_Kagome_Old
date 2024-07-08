if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserCreditCard]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserCreditCard]
GO
/*
=========================================================================================================
  Module      : ユーザクレジットカードマスタ(w2_UserCreditCard.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserCreditCard] (
	[user_id] [nvarchar] (30) NOT NULL,
	[branch_no] [int] NOT NULL DEFAULT (1),
	[cooperation_id] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[card_disp_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[last_four_digit] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[expiration_month] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[expiration_year] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[author_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[disp_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
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

ALTER TABLE [dbo].[w2_UserCreditCard] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserCreditCard] PRIMARY KEY  CLUSTERED
	(
		[user_id],
		[branch_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_UserCreditCard_1] ON [dbo].[w2_UserCreditCard]([cooperation_id], [cooperation_id2]) ON [PRIMARY]
GO
/*
V5.11
ALTER TABLE [w2_UserCreditCard] ADD [company_code] [nvarchar] (30) NOT NULL DEFAULT (N'')
ALTER TABLE [w2_UserCreditCard] ADD [cooperation_id2] [nvarchar] (100) NOT NULL DEFAULT (N'')
ALTER TABLE [w2_UserCreditCard] ADD [cooperation_type] [nvarchar] (10) NOT NULL DEFAULT (N'CREDIT');
*/