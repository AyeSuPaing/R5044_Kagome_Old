if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MallOrderMail]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MallOrderMail]
GO
/*
=========================================================================================================
  Module      : モール注文メールマスタ(w2_MallOrderMail.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MallOrderMail] (
	[order_mail_id] [int] IDENTITY(1,1) NOT NULL,
	[mail_to] [nvarchar] (1024),
	[mail_message] [ntext],
	[recived_datetime] [datetime] NOT NULL,
	[order_entry_flg] [bit] NOT NULL DEFAULT (0)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
