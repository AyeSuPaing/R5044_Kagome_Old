if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_FixedPurchaseHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_FixedPurchaseHistory]
GO
/*
=========================================================================================================
  Module      : ТшК·НwУ№ЧЪЧЁПюХё(w2_FixedPurchaseHistory.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_FixedPurchaseHistory] (
	[fixed_purchase_id] [nvarchar] (30) NOT NULL,
	[fixed_purchase_history_no] [bigint] NOT NULL,
	[fixed_purchase_history_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'10'),
	[base_tel_no] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[update_order_count] [int],
	[update_shipped_count] [int],
	[update_order_count_result] [int],
	[update_shipped_count_result] [int]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FixedPurchaseHistory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FixedPurchaseHistory] PRIMARY KEY  CLUSTERED
	(
		[fixed_purchase_id],
		[fixed_purchase_history_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_FixedPurchaseHistory_1] ON [dbo].[w2_FixedPurchaseHistory]([user_id]) ON [PRIMARY]
GO

/*
-- v5.11
ALTER TABLE [w2_FixedPurchaseHistory] ADD [update_order_count] [int];
ALTER TABLE [w2_FixedPurchaseHistory] ADD [update_shipped_count] [int];
ALTER TABLE [w2_FixedPurchaseHistory] ADD [update_order_count_result] [int];
ALTER TABLE [w2_FixedPurchaseHistory] ADD [update_shipped_count_result] [int];
*/