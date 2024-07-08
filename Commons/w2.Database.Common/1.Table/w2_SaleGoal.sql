IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[w2_SaleGoal]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_SaleGoal]
GO
/*
=========================================================================================================
  Module      : Sale Goal (w2_SaleGoal.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_SaleGoal] (
	[year] [nvarchar] (4) NOT NULL DEFAULT (''),
	[annual_goal] [decimal] (18,3) NOT NULL DEFAULT (0),
	[applicable_month] [int] NOT NULL DEFAULT (1),
	[monthly_goal_1] [decimal] (18,3) NOT NULL DEFAULT (0),
	[monthly_goal_2] [decimal] (18,3) NOT NULL DEFAULT (0),
	[monthly_goal_3] [decimal] (18,3) NOT NULL DEFAULT (0),
	[monthly_goal_4] [decimal] (18,3) NOT NULL DEFAULT (0),
	[monthly_goal_5] [decimal] (18,3) NOT NULL DEFAULT (0),
	[monthly_goal_6] [decimal] (18,3) NOT NULL DEFAULT (0),
	[monthly_goal_7] [decimal] (18,3) NOT NULL DEFAULT (0),
	[monthly_goal_8] [decimal] (18,3) NOT NULL DEFAULT (0),
	[monthly_goal_9] [decimal] (18,3) NOT NULL DEFAULT (0),
	[monthly_goal_10] [decimal] (18,3) NOT NULL DEFAULT (0),
	[monthly_goal_11] [decimal] (18,3) NOT NULL DEFAULT (0),
	[monthly_goal_12] [decimal] (18,3) NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT ('')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_SaleGoal] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_SaleGoal] PRIMARY KEY  CLUSTERED
	(
		[year]
	) ON [PRIMARY]
GO