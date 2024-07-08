if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_FixedPurchaseCancelReason]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_FixedPurchaseCancelReason]
GO
/*
=========================================================================================================
  Module      : 定期解約理由区分設定 (w2_FixedPurchaseCancelReason.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_FixedPurchaseCancelReason] (
	[cancel_reason_id] [nvarchar] (30) NOT NULL,
	[cancel_reason_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (1),
	[display_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FixedPurchaseCancelReason] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FixedPurchaseCancelReason] PRIMARY KEY  CLUSTERED
	(
		[cancel_reason_id]
	) ON [PRIMARY]
GO