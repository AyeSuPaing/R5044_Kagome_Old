if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_InvoiceScoreDetail]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_InvoiceScoreDetail]
GO
/*
=========================================================================================================
  Module      : スコア後払い後払い請求書明細 (w2_InvoiceScoreDetail.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_InvoiceScoreDetail] (
	[order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[detail_no] [int] NOT NULL DEFAULT (1),
	[goods_name] [nvarchar] (150) NOT NULL DEFAULT (N''),
	[goods_price] [nvarchar] (6) NOT NULL DEFAULT (N''),
	[goods_num] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_InvoiceScoreDetail] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_InvoiceScoreDetail] PRIMARY KEY  CLUSTERED
	(
		[order_id],
		[detail_no]
	) ON [PRIMARY]
GO
