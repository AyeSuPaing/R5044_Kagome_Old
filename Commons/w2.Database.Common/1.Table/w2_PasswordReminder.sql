if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_PasswordReminder]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_PasswordReminder]
GO
/*
=========================================================================================================
  Module      : パスワードリマインダー情報(w2_PasswordReminder.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/

CREATE TABLE [dbo].[w2_PasswordReminder] (
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[authentication_key] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[change_trial_limit_count] [int] NOT NULL,
	[date_created] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_PasswordReminder] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_PasswordReminder] PRIMARY KEY  CLUSTERED
	(
		[user_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_PasswordReminder_1] ON [dbo].[w2_PasswordReminder]([authentication_key]) ON [PRIMARY]
GO
