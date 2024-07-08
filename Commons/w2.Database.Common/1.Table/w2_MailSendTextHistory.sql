if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MailSendTextHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MailSendTextHistory]
GO
/*
=========================================================================================================
  Module      : ГББ[ГЛФzРMОЮХ╢П═ЧЪЧЁ (w2_MailSendTextHistory.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MailSendTextHistory] (
	[text_history_no] [bigint] IDENTITY (1, 1),
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mailtext_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[maildist_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[maildist_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mail_body] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[mail_body_html] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[mailtext_body_mobile] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[mailtext_decome] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MailSendTextHistory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MailSendTextHistory] PRIMARY KEY  CLUSTERED
	(
		[text_history_no]
	) ON [PRIMARY]
GO
