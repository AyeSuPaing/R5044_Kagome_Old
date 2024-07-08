/*
=========================================================================================================
  Module      : Scoring Sale Question Page (w2_ScoringSaleQuestionPage.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[w2_ScoringSaleQuestionPage]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_ScoringSaleQuestionPage]
GO

CREATE TABLE [dbo].[w2_ScoringSaleQuestionPage] (
	[scoring_sale_id] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[page_no] [int] NOT NULL DEFAULT (1),
	[previous_page_btn_caption] [nvarchar] (40) NOT NULL DEFAULT (N''),
	[next_page_btn_caption] [nvarchar] (40) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ScoringSaleQuestionPage] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ScoringSaleQuestionPage] PRIMARY KEY CLUSTERED
	(
		[scoring_sale_id],
		[page_no]
	) ON [PRIMARY]
GO
