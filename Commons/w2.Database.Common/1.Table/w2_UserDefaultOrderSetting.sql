if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserDefaultOrderSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserDefaultOrderSetting]
GO
/*
=========================================================================================================
  Module      : ユーザーデフォルト注文方法設定(w2_UserDefaultOrderSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserDefaultOrderSetting] (
	[user_id] [nvarchar] (30) NOT NULL,
	[payment_id] [nvarchar] (10),
	[credit_branch_no] [int],
	[user_shipping_no] [int],
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[user_invoice_no] [int],
	[zeus_cvs_type] [nvarchar] (20),
	[paygent_cvs_type] [nvarchar] (20)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserDefaultOrderSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserDefaultOrderSetting] PRIMARY KEY  CLUSTERED
	(
		[user_id]
	) ON [PRIMARY]
GO

/*
IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'w2_UserDefaultOrderSetting' AND COLUMN_NAME = 'user_invoice_no')
	ALTER TABLE [w2_UserDefaultOrderSetting] ADD [user_invoice_no] [int];
GO
IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'w2_UserDefaultOrderSetting' AND COLUMN_NAME = 'zeus_cvs_type')
	ALTER TABLE [w2_UserDefaultOrderSetting] ADD [zeus_cvs_type] [nvarchar] (20);
GO
IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'w2_UserDefaultOrderSetting' AND COLUMN_NAME = 'paygent_cvs_type')
	ALTER TABLE [w2_UserDefaultOrderSetting] ADD [paygent_cvs_type] [nvarchar] (20);
GO
*/
