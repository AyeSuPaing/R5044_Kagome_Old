/*
=========================================================================================================
  Module      : Scoring Sale Result Condition (w2_ScoringSaleResultCondition.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[w2_ScoringSaleResultCondition]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_ScoringSaleResultCondition]
GO

CREATE TABLE [dbo].[w2_ScoringSaleResultCondition] (
	[scoring_sale_id] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[branch_no] [int] NOT NULL,
	[condition_branch_no] [int] NOT NULL,
	[score_axis_axis_no] [int] NOT NULL,
	[score_axis_axis_value_from] [int] NOT NULL,
	[score_axis_axis_value_to] [int] NOT NULL,
	[condition] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[group_no] [int] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ScoringSaleResultCondition] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ScoringSaleResultCondition] PRIMARY KEY CLUSTERED
	(
		[scoring_sale_id],
		[branch_no],
		[condition_branch_no]
	) ON [PRIMARY]
GO
