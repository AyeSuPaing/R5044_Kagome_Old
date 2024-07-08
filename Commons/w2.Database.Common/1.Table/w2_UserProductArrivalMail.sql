if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserProductArrivalMail]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserProductArrivalMail]
GO
/*
=========================================================================================================
  Module      : 入荷通知メール情報(w2_UserProductArrivalMail.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserProductArrivalMail] (
	[user_id] [nvarchar] (30) NOT NULL,
	[mail_no] [int] NOT NULL,
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[pcmobile_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'PC'),
	[arrival_mail_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'ARRIVAL'),
	[mail_send_status] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_expired] [datetime] NOT NULL DEFAULT (getdate()),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[guest_mail_addr] [nvarchar] (256) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserProductArrivalMail] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserProductArrivalMail] PRIMARY KEY  CLUSTERED
	(
		[user_id],
		[mail_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_UserProductArrivalMail_1] ON [dbo].[w2_UserProductArrivalMail]([shop_id], [product_id], [variation_id], [arrival_mail_kbn], [mail_send_status]) ON [PRIMARY]
GO
