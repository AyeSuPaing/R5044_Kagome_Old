/*
=========================================================================================================
  Module      : LINEЙ╝ЙяИїГeБ[ГuГЛ (w2_LineTemporaryUser.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_LineTemporaryUser]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_LineTemporaryUser]
GO

CREATE TABLE [dbo].[w2_LineTemporaryUser] (
	[line_user_id] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[temporary_user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[temporary_user_registration_date] [datetime] NOT NULL DEFAULT (getdate()),
	[regular_user_registration_flag] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[regular_user_registration_date] [datetime],
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_LineTemporaryUser] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_LineTemporaryUser] PRIMARY KEY  CLUSTERED
	(
		[line_user_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_LineTemporaryUser_1] ON [dbo].[w2_LineTemporaryUser]([temporary_user_id]) ON [PRIMARY]
GO
