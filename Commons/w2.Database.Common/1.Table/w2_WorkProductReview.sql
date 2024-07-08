if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_WorkProductReview]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_WorkProductReview]
GO
/*
=========================================================================================================
  Module      : 商品レビュー情報用ワークテーブル(w2_WorkProductReview)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_WorkProductReview] (
	[shop_id] [nvarchar] (10) NOT NULL,
	[product_id] [nvarchar] (30) NOT NULL,
	[review_no] [int] NOT NULL,
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[nick_name] [nvarchar] (40) NOT NULL DEFAULT (N''),
	[review_rating] [int] NOT NULL DEFAULT (1),
	[review_title] [ntext] NOT NULL DEFAULT (N''),
	[review_comment] [ntext] NOT NULL DEFAULT (N''),
	[open_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[checked_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_opened] [datetime],
	[date_checked] [datetime],
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkProductReview] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkProductReview] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[product_id],
		[review_no]
	) ON [PRIMARY]
GO
