/*
=========================================================================================================
  Module      : Scoring Sale Question Page Item (w2_ScoringSaleQuestionPageItem.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[w2_ScoringSaleQuestionPageItem]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_ScoringSaleQuestionPageItem]
GO

CREATE TABLE [dbo].[w2_ScoringSaleQuestionPageItem] (
	[question_id] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[scoring_sale_id] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[page_no] int NOT NULL DEFAULT (1),
	[branch_no] [int] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ScoringSaleQuestionPageItem] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ScoringSaleQuestionPageItem] PRIMARY KEY CLUSTERED
	(
		[scoring_sale_id],
		[page_no],
		[branch_no]
	) ON [PRIMARY]
GO
