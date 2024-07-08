if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MallPrdcnv]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MallPrdcnv]
GO
/*
=========================================================================================================
  Module      : モール商品コンバータマスタ(w2_MallPrdcnv.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MallPrdcnv] (
	[adto_id] [int] IDENTITY(1,1) NOT NULL,
	[shop_id] [nvarchar] (10) NOT NULL,
	[adto_name] [nvarchar] (100) NOT NULL,
	[separater] [char] (1) NOT NULL,
	[characterCodeType] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[newLineType] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[quote] [char] (1),
	[isQuote] [bit] NOT NULL,
	[isHeader] [bit] NOT NULL,
	[taskId] [int] NOT NULL DEFAULT (0),
	[lastCreated] [datetime],
	[createdRecordCount] [int] NOT NULL DEFAULT (0),
	[filename] [varchar] (50) NOT NULL,
	[path] [varchar] (256) NOT NULL,
	[sourceTableName] [varchar] (256),
	[extractionPattern] [int] NOT NULL DEFAULT (0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MallPrdcnv] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MallPrdcnv] PRIMARY KEY  CLUSTERED
	(
		[adto_id]
	) ON [PRIMARY]
GO
