if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_News]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_News]
GO
/*
=========================================================================================================
  Module      : 新着情報マスタ(w2_News.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_News] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[news_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[display_date_from] [datetime] NOT NULL,
	[news_text_kbn] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[news_text] [ntext] NOT NULL DEFAULT (N''),
	[news_text_kbn_mobile] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[news_text_mobile] [ntext] NOT NULL DEFAULT (N''),
	[disp_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[mobile_disp_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[display_order] [int] NOT NULL DEFAULT (1),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[brand_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[display_date_to] [datetime]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_News] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_News] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[news_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_News_1] ON [dbo].[w2_News]([display_order]) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_News] ADD [display_date_to] [datetime];
EXEC SP_RENAME 'w2_News.display_date', 'display_date_from', 'COLUMN';
*/