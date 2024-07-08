if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_InvoiceVeritransDetail]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_InvoiceVeritransDetail]
GO
/*
=========================================================================================================
  Module      : ГxГКГgГЙГУГXМуХеВвМуХеВвР┐ЛБПСЦ╛Н╫ (w2_InvoiceVeritransDetail.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_InvoiceVeritransDetail] (
	[order_id] [nvarchar](30) NOT NULL DEFAULT (N''),
	[detail_no] [int] NOT NULL DEFAULT (1),
	[goods_name] [nvarchar](150) NOT NULL DEFAULT (N''),
	[goods_price] [nvarchar](6) NOT NULL DEFAULT (N''),
	[goods_num] [nvarchar](4) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_InvoiceVeritransDetail] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_InvoiceVeritransDetail] PRIMARY KEY  CLUSTERED
	(
		[order_id],
		[detail_no]
	) ON [PRIMARY]
GO
