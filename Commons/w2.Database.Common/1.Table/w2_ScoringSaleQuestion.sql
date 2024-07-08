/*
=========================================================================================================
  Module      : Scoring Sale Question (w2_ScoringSaleQuestion.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[w2_ScoringSaleQuestion]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_ScoringSaleQuestion]
GO

CREATE TABLE [dbo].[w2_ScoringSaleQuestion] (
	[question_id] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[question_title] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[score_axis_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[answer_type] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[question_statement] [nvarchar] (250) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ScoringSaleQuestion] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ScoringSaleQuestion] PRIMARY KEY CLUSTERED
	(
		[question_id]
	) ON [PRIMARY]
GO
