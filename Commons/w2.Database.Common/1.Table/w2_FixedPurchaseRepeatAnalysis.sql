if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_FixedPurchaseRepeatAnalysis]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_FixedPurchaseRepeatAnalysis]
GO
/*
=========================================================================================================
  Module      : ТшК·НwУ№МpС▒ХкР═ГeБ[ГuГЛ (w2_FixedPurchaseRepeatAnalysis.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_FixedPurchaseRepeatAnalysis] (
	[data_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[count] [int] NOT NULL,
	[order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[fixed_purchase_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[status] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FixedPurchaseRepeatAnalysis] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FixedPurchaseRepeatAnalysis] PRIMARY KEY  CLUSTERED
	(
		[data_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_FixedPurchaseRepeatAnalysis_1] ON [dbo].[w2_FixedPurchaseRepeatAnalysis]([user_id], [product_id], [variation_id]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_FixedPurchaseRepeatAnalysis_2] ON [dbo].[w2_FixedPurchaseRepeatAnalysis]([product_id], [variation_id]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_FixedPurchaseRepeatAnalysis_3] ON [dbo].[w2_FixedPurchaseRepeatAnalysis]([variation_id]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_FixedPurchaseRepeatAnalysis_4] ON [dbo].[w2_FixedPurchaseRepeatAnalysis]([order_id]) ON [PRIMARY]
GO
