if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_PaymentPrice]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_PaymentPrice]
GO
/*
=========================================================================================================
  Module      : МИН╧ОшРФЧ┐Г}ГXГ^(w2_PaymentPrice.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_PaymentPrice] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[payment_group_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[payment_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[payment_price_no] [int] NOT NULL DEFAULT (1),
	[tgt_price_bgn] [decimal] (18,3) NOT NULL DEFAULT (0),
	[tgt_price_end] [decimal] (18,3) NOT NULL DEFAULT (0),
	[payment_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_PaymentPrice] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_PaymentPrice] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[payment_group_id],
		[payment_id],
		[payment_price_no]
	) ON [PRIMARY]
GO

/*
Бб МИН╧Т╩Й▌С╬ЙЮ
ALTER TABLE [w2_PaymentPrice] ALTER COLUMN [tgt_price_bgn] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_PaymentPrice] ALTER COLUMN [tgt_price_end] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_PaymentPrice] ALTER COLUMN [payment_price] [decimal] (18,3) NOT NULL;
*/
