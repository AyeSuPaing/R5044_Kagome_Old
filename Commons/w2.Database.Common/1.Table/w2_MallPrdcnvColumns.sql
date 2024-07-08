if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MallPrdcnvColumns]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MallPrdcnvColumns]
GO
/*
=========================================================================================================
  Module      : モール商品コンバータカラム(w2_MallPrdcnvColumns.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MallPrdcnvColumns] (
	[adcolumn_id] [int] IDENTITY(1,1) NOT NULL,
	[adto_id] [int] NOT NULL,
	[column_no] [int] NOT NULL,
	[column_name] [nvarchar] (50) NOT NULL,
	[output_format] [nvarchar] (512)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MallPrdcnvColumns] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MallPrdcnvColumns] PRIMARY KEY  CLUSTERED
	(
		[adcolumn_id]
	) ON [PRIMARY]
GO
