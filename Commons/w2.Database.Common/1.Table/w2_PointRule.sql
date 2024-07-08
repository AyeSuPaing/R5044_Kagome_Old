if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_PointRule]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_PointRule]
GO
/*
=========================================================================================================
  Module      : ポイントルールマスタ(w2_PointRule.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_PointRule] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[point_rule_id] [nvarchar] (10) NOT NULL,
	[point_rule_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[point_rule_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'01'),
	[point_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'01'),
	[use_temp_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[point_inc_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[inc_type] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[inc_num] [decimal] NOT NULL DEFAULT (0),
	[inc_rate] [decimal] NOT NULL DEFAULT (0),
	[point_exp_extend] [nvarchar] (10) NOT NULL DEFAULT (N'+000000'),
	[exp_bgn] [datetime],
	[exp_end] [datetime],
	[campaign_term_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[campaign_term_value] [nvarchar] (10),
	[priority] [int] NOT NULL DEFAULT (100),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[kbn1] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[kbn2] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[kbn3] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[kbn4] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[kbn5] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[fixed_purchase_inc_type] [nvarchar](10) DEFAULT (N''),
	[fixed_purchase_inc_num] [decimal],
	[fixed_purchase_inc_rate] [decimal],
	[effective_offset] [int],
	[effective_offset_type] [nvarchar] (2),
	[term] [int],
	[term_type] [nvarchar] (2),
	[period_begin] [datetime],
	[period_end] [datetime],
	[allow_duplicate_apply_flg] [nvarchar] (2) NOT NULL DEFAULT N'0'
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_PointRule] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_PointRule] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[point_rule_id]
	) ON [PRIMARY]
GO
