if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_PointRuleDate]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_PointRuleDate]
GO
/*
=========================================================================================================
  Module      : ポイントルール日付マスタ(w2_PointRuleDate.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_PointRuleDate] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[point_rule_id] [nvarchar] (10) NOT NULL,
	[tgt_date] [datetime] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_PointRuleDate] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_PointRuleDate] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[point_rule_id],
		[tgt_date]
	) ON [PRIMARY]
GO
