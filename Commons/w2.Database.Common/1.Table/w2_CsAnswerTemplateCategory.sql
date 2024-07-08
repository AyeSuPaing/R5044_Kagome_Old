if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsAnswerTemplateCategory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsAnswerTemplateCategory]
GO
/*
=========================================================================================================
  Module      : 回答例カテゴリマスタ(w2_CsAnswerTemplateCategory.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsAnswerTemplateCategory] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[category_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[parent_category_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[category_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (1),
	[valid_flg] [nvarchar] (10) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsAnswerTemplateCategory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsAnswerTemplateCategory] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[category_id]
	) ON [PRIMARY]
GO
