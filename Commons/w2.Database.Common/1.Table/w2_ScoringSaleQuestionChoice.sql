/*
=========================================================================================================
  Module      : Scoring Sale Question Choice (w2_ScoringSaleQuestionChoice.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[w2_ScoringSaleQuestionChoice]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_ScoringSaleQuestionChoice]
GO

CREATE TABLE [dbo].[w2_ScoringSaleQuestionChoice] (
	[question_id] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[branch_no] [int] NOT NULL,
	[question_choice_statement] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[question_choice_statement_img_path] [nvarchar] (360),
	[axis_additional1] [int] NOT NULL,
	[axis_additional2] [int] NOT NULL,
	[axis_additional3] [int] NOT NULL,
	[axis_additional4] [int] NOT NULL,
	[axis_additional5] [int] NOT NULL,
	[axis_additional6] [int] NOT NULL,
	[axis_additional7] [int] NOT NULL,
	[axis_additional8] [int] NOT NULL,
	[axis_additional9] [int] NOT NULL,
	[axis_additional10] [int] NOT NULL,
	[axis_additional11] [int] NOT NULL,
	[axis_additional12] [int] NOT NULL,
	[axis_additional13] [int] NOT NULL,
	[axis_additional14] [int] NOT NULL,
	[axis_additional15] [int] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ScoringSaleQuestionChoice] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ScoringSaleQuestionChoice] PRIMARY KEY CLUSTERED
	(
		[question_id],
		[branch_no]
	) ON [PRIMARY]
GO
