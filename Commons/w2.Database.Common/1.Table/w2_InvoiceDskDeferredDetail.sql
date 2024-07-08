if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_InvoiceDskDeferredDetail]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_InvoiceDskDeferredDetail]
GO
/*
=========================================================================================================
  Module      : DSK後払い後払い請求書明細 (w2_InvoiceDskDeferredDetail.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_InvoiceDskDeferredDetail] (
	[order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[detail_no] [int] NOT NULL DEFAULT (1),
	[goods_name] [nvarchar] (150) NOT NULL DEFAULT (N''),
	[goods_price] [nvarchar] (6) NOT NULL DEFAULT (N''),
	[goods_num] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_InvoiceDskDeferredDetail] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_InvoiceDskDeferredDetail] PRIMARY KEY  CLUSTERED
	(
		[order_id],
		[detail_no]
	) ON [PRIMARY]
GO
