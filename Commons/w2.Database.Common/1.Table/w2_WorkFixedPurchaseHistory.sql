/*
=========================================================================================================
  Module      : Fixed purchase history work table(w2_WorkFixedPurchaseHistory.sql)
  еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[w2_WorkFixedPurchaseHistory]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_WorkFixedPurchaseHistory]
GO

CREATE TABLE [dbo].[w2_WorkFixedPurchaseHistory] (
	[fixed_purchase_id] [nvarchar] (30) NOT NULL,
	[fixed_purchase_history_no] [bigint] NOT NULL,
	[fixed_purchase_history_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'10'),
	[base_tel_no] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[update_order_count] [int],
	[update_shipped_count] [int],
	[update_order_count_result] [int],
	[update_shipped_count_result] [int],
	[external_payment_cooperation_log] [nvarchar] (MAX) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_WorkFixedPurchaseHistory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_WorkFixedPurchaseHistory] PRIMARY KEY CLUSTERED
	(
		[fixed_purchase_id],
		[fixed_purchase_history_no]
	) ON [PRIMARY]
GO
