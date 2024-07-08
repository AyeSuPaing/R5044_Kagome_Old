if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_InvoiceAtodeneDetail]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_InvoiceAtodeneDetail]
GO
/*
=========================================================================================================
  Module      : AtodeneМуХеВвР┐ЛБПСЦ╛Н╫ (w2_InvoiceAtodeneDetail.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_InvoiceAtodeneDetail] (
	[order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[detail_no] [int] NOT NULL DEFAULT (1),
	[goods] [nvarchar] (150) NOT NULL DEFAULT (N''),
	[goodsAmount] [nvarchar] (6) NOT NULL DEFAULT (N''),
	[goodsPrice] [nvarchar] (7) NOT NULL DEFAULT (N''),
	[goodsSubtotal] [nvarchar] (7) NOT NULL DEFAULT (N''),
	[goodsExpand] [nvarchar] (6) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_InvoiceAtodeneDetail] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_InvoiceAtodeneDetail] PRIMARY KEY  CLUSTERED
	(
		[order_id],
		[detail_no]
	) ON [PRIMARY]
GO
