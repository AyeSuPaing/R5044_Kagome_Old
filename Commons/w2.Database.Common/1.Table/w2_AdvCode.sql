/*
=========================================================================================================
  Module      : 広告コードマスタ(w2_AdvCode.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AdvCode]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AdvCode]
GO

CREATE TABLE [dbo].[w2_AdvCode] (
	[advcode_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[advertisement_code] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[media_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[valid_flg] [nvarchar] (10) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[advertisement_date] [datetime],
	[media_cost] [decimal] (18,3),
	[memo] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[publication_date_from] [datetime],
	[publication_date_to] [datetime],
	[advcode_media_type_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[member_rank_id_granted_at_account_registration] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[user_management_level_id_granted_at_account_registration] [nvarchar] (30) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AdvCode] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AdvCode] PRIMARY KEY  CLUSTERED
	(
		[advcode_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_AdvCode_1] ON [dbo].[w2_AdvCode]([dept_id], [advertisement_code]) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_AdvCode] ADD [advertisement_date] [datetime];
ALTER TABLE [w2_AdvCode] ADD [media_cost] [decimal];
ALTER TABLE [w2_AdvCode] ADD [memo] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_AdvCode] ADD [publication_date_from] [datetime];
ALTER TABLE [w2_AdvCode] ADD [publication_date_to] [datetime];
ALTER TABLE [w2_AdvCode] ADD [advcode_media_type_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_AdvCode] ADD [member_rank_id_granted_at_account_registration] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_AdvCode] ADD [user_management_level_id_granted_at_account_registration] [nvarchar] (30) NOT NULL DEFAULT (N'');
*/

/*
■ 決済通貨対応
ALTER TABLE [w2_AdvCode] ALTER COLUMN [media_cost] [decimal] (18,3);
*/