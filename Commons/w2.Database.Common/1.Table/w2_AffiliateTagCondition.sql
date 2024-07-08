if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AffiliateTagCondition]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AffiliateTagCondition]
GO
/*
=========================================================================================================
  Module      : アフィリエイトタグの出力条件管理 (w2_AffiliateTagCondition.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_AffiliateTagCondition] (
	[affiliate_id] [int] NOT NULL,
	[condition_type] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[condition_sort_no] [int] NOT NULL,
	[condition_value] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[match_type] [nvarchar] (20) NOT NULL DEFAULT (N'PERFECT')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AffiliateTagCondition] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AffiliateTagCondition] PRIMARY KEY  CLUSTERED
	(
		[affiliate_id],
		[condition_type],
		[condition_sort_no]
	) ON [PRIMARY]
GO
