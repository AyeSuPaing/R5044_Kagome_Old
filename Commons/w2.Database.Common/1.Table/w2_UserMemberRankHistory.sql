if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserMemberRankHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserMemberRankHistory]
GO
/*
=========================================================================================================
  Module      : 会員ランク更新履歴情報([w2_UserMemberRankHistory].sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserMemberRankHistory] (
	[history_no] [int] IDENTITY (1, 1) NOT NULL,
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[before_rank_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[after_rank_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mail_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[changed_by] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserMemberRankHistory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserMemberRankHistory] PRIMARY KEY  NONCLUSTERED
	(
		[history_no]
	) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_UserMemberRankHistory_1] ON [dbo].[w2_UserMemberRankHistory]([user_id], [date_created]) ON [PRIMARY]
GO
