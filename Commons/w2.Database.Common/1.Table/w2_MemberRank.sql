if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MemberRank]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MemberRank]
GO
/*
=========================================================================================================
  Module      : 会員ランクマスタ(w2_MemberRank.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MemberRank] (
	[member_rank_id] [nvarchar] (30) NOT NULL,
	[member_rank_order] [int] NOT NULL,
	[member_rank_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[order_discount_type] [nvarchar] (10) NOT NULL DEFAULT (N'NONE'),
	[order_discount_value] [decimal] (18,3),
	[order_discount_threshold_price] [decimal] (18,3),
	[point_add_type] [nvarchar] (10) NOT NULL DEFAULT (N'NONE'),
	[point_add_value] [decimal],
	[shipping_discount_type] [nvarchar] (10) NOT NULL DEFAULT (N'NONE'),
	[shipping_discount_value] [decimal] (18,3),
	[default_member_rank_setting_flg] [nvarchar] (10) NOT NULL DEFAULT (N'OFF'),
	[valid_flg] [nvarchar] (10) NOT NULL DEFAULT (N'ON'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[member_rank_memo] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[fixed_purchase_member_discount_rate] [decimal] NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MemberRank] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MemberRank] PRIMARY KEY  NONCLUSTERED
	(
		[member_rank_id]
	) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_MemberRank_1] ON [dbo].[w2_MemberRank]([member_rank_order]) ON [PRIMARY]
GO

/*
IF NOT EXISTS (SELECT column_name FROM INFORMATION_SCHEMA.columns WHERE table_name = 'w2_MemberRank' AND column_name = 'member_rank_memo')
	ALTER TABLE [w2_MemberRank] ADD [member_rank_memo] [nvarchar] (max) NOT NULL DEFAULT (N'');
*/

/*
■ 決済通貨対応
ALTER TABLE [w2_MemberRank] ALTER COLUMN [order_discount_value] [decimal] (18,3);
ALTER TABLE [w2_MemberRank] ALTER COLUMN [order_discount_threshold_price] [decimal] (18,3);
ALTER TABLE [w2_MemberRank] ALTER COLUMN [shipping_discount_value] [decimal] (18,3);
*/