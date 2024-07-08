if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserAttribute]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserAttribute]
GO
/*
=========================================================================================================
  Module      : ユーザー属性マスタ (w2_UserAttribute.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserAttribute] (
	[user_id] [nvarchar] (30) NOT NULL,
	[first_order_date] [datetime],
	[second_order_date] [datetime],
	[last_order_date] [datetime],
	[enrollment_days] AS (DATEDIFF(hour, first_order_date, last_order_date)/24),
	[away_days] AS (DATEDIFF(hour, last_order_date, GETDATE())/24),
	[order_amount_order_all] [decimal] (18,3) NOT NULL DEFAULT (0),
	[order_amount_order_fp] [decimal] (18,3) NOT NULL DEFAULT (0),
	[order_count_order_all] [int] NOT NULL DEFAULT (0),
	[order_count_order_fp] [int] NOT NULL DEFAULT (0),
	[order_amount_ship_all] [decimal] (18,3) NOT NULL DEFAULT (0),
	[order_amount_ship_fp] [decimal] (18,3) NOT NULL DEFAULT (0),
	[order_count_ship_all] [int] NOT NULL DEFAULT (0),
	[order_count_ship_fp] [int] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[cpm_cluster_attribute1] [nvarchar] (15) NOT NULL DEFAULT (N''),
	[cpm_cluster_attribute2] [nvarchar] (15) NOT NULL DEFAULT (N''),
	[cpm_cluster_id] AS cpm_cluster_attribute1 + cpm_cluster_attribute2 PERSISTED,
	[cpm_cluster_attribute1_before] [nvarchar] (15) NOT NULL DEFAULT (N''),
	[cpm_cluster_attribute2_before] [nvarchar] (15) NOT NULL DEFAULT (N''),
	[cpm_cluster_id_before] AS cpm_cluster_attribute1_before + cpm_cluster_attribute2_before PERSISTED,
	[cpm_cluster_changed_date] [datetime]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserAttribute] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserAttribute] PRIMARY KEY  CLUSTERED
	(
		[user_id]
	) ON [PRIMARY]
GO
/*
ALTER TABLE [w2_UserAttribute] ADD [cpm_cluster_attribute1] [nvarchar] (15) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_UserAttribute] ADD [cpm_cluster_attribute2] [nvarchar] (15) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_UserAttribute] ADD [cpm_cluster_id] AS cpm_cluster_attribute1 + cpm_cluster_attribute2 PERSISTED;
ALTER TABLE [w2_UserAttribute] ADD [cpm_cluster_attribute1_before] [nvarchar] (15) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_UserAttribute] ADD [cpm_cluster_attribute2_before] [nvarchar] (15) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_UserAttribute] ADD [cpm_cluster_id_before] AS cpm_cluster_attribute1_before + cpm_cluster_attribute2_before PERSISTED;
ALTER TABLE [w2_UserAttribute] ADD [cpm_cluster_changed_date] [datetime];
*/

/*
■ 決済通貨対応
ALTER TABLE [w2_UserAttribute] ALTER COLUMN [order_amount_order_all] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_UserAttribute] ALTER COLUMN [order_amount_order_fp] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_UserAttribute] ALTER COLUMN [order_amount_ship_all] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_UserAttribute] ALTER COLUMN [order_amount_ship_fp] [decimal] (18,3) NOT NULL;
*/