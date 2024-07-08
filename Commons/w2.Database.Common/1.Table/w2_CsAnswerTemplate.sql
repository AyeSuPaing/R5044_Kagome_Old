if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsAnswerTemplate]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsAnswerTemplate]
GO
/*
=========================================================================================================
  Module      : ЙёУЪЧсГ}ГXГ^(w2_CsAnswerTemplate.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsAnswerTemplate] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[answer_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[answer_category_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[answer_title] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[answer_text] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (1),
	[valid_flg] [nvarchar] (10) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[answer_mail_title] [nvarchar] (50) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsAnswerTemplate] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsAnswerTemplate] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[answer_id]
	) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_CsAnswerTemplate] ADD [answer_mail_title] [nvarchar] (50) NOT NULL DEFAULT (N'');
*/