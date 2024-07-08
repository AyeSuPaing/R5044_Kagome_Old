if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_WorkArticleCategory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_WorkArticleCategory]
GO
/*
=========================================================================================================
  Module      : ТНХ╢ПюХёНXРVЧЪЧЁ(w2_WorkArticleCategory.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_WorkArticleCategory](
	[category_id] [nvarchar](30) NOT NULL,
	[parent_category_id] [nvarchar](30) NOT NULL,
	[category_name] [nvarchar](50) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (0),
	[valid_flg] [nvarchar](1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NULL DEFAULT (getdate()),
	[date_changed] [datetime] NULL DEFAULT (getdate()),
	[last_changed] [nvarchar](20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkArticleCategory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkArticleCategory] PRIMARY KEY  CLUSTERED
	(
		[category_id]
	) ON [PRIMARY]
GO