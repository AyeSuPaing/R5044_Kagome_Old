if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MallPrdcnvRule]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MallPrdcnvRule]
GO
/*
=========================================================================================================
  Module      : モール商品コンバータ文字変換ルール(w2_MallPrdcnvRule.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MallPrdcnvRule] (
	[adconv_id] [int] IDENTITY(1,1) NOT NULL,
	[adto_id] [int] NOT NULL,
	[convertFrom] [nvarchar] (100),
	[convertTo] [nvarchar] (100),
	[target] [int]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MallPrdcnvRule] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MallPrdcnvRule] PRIMARY KEY  CLUSTERED
	(
		[adconv_id]
	) ON [PRIMARY]
GO
