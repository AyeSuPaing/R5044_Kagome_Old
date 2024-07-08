if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MallPrdcnvFiles]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MallPrdcnvFiles]
GO
/*
=========================================================================================================
  Module      : モール商品コンバータ作成ファイル(w2_MallPrdcnvFiles.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MallPrdcnvFiles] (
	[adfiles_id] [int] IDENTITY(1,1) NOT NULL,
	[adto_id] [int] NOT NULL,
	[path] [nvarchar] (260) NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MallPrdcnvFiles] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MallPrdcnvFiles] PRIMARY KEY  CLUSTERED
	(
		[adfiles_id]
	) ON [PRIMARY]
GO
