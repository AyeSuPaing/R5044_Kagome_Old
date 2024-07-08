if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_NoveltyGrantConditions]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_NoveltyGrantConditions]
GO
/*
=========================================================================================================
  Module      : ГmГxГЛГeГBХtЧ^ПЁМП(w2_NoveltyGrantConditions.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_NoveltyGrantConditions] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[novelty_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[condition_no] [int] NOT NULL DEFAULT (1),
	[user_rank_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[amount_begin] [decimal] (18,3) NOT NULL DEFAULT (0),
	[amount_end] [decimal] (18,3),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_NoveltyGrantConditions] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_NoveltyGrantConditions] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[novelty_id],
		[condition_no]
	) ON [PRIMARY]
GO

/*
Бб МИН╧Т╩Й▌С╬ЙЮ
ALTER TABLE [w2_NoveltyGrantConditions] ALTER COLUMN [amount_begin] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_NoveltyGrantConditions] ALTER COLUMN [amount_end] [decimal] (18,3);
*/