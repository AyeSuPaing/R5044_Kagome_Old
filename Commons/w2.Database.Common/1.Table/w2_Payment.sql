if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_Payment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_Payment]
GO
/*
=========================================================================================================
  Module      : МИН╧ОэХ╩Г}ГXГ^(w2_Payment.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_Payment] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[payment_group_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[payment_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[payment_alt_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[payment_name] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[payment_name_mobile] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[payment_price_kbn] [nvarchar] (3) NOT NULL DEFAULT (N'0'),
	[payment_price] [decimal] NOT NULL DEFAULT (0),
	[usable_price_min] [decimal] (18,3),
	[usable_price_max] [decimal] (18,3),
	[mobile_disp_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[display_order] [int] NOT NULL DEFAULT (1),		
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[user_management_level_not_use] [nvarchar] (256) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_Payment] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_Payment] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[payment_group_id],
		[payment_id]
	) ON [PRIMARY]
GO

/*
ALTER TABLE [dbo].[w2_Payment] ADD [user_management_level_not_use] [nvarchar] (256) NOT NULL DEFAULT (N'');
*/

/*
Бб МИН╧Т╩Й▌С╬ЙЮ
ALTER TABLE [w2_Payment] ALTER COLUMN [usable_price_min] [decimal] (18,3);
ALTER TABLE [w2_Payment] ALTER COLUMN [usable_price_max] [decimal] (18,3);
*/