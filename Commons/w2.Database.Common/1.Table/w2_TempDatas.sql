if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_TempDatas]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_TempDatas]
GO
/*
=========================================================================================================
  Module      : �e���|�����f�[�^�e�[�u��(w2_TempDatas.sql)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_TempDatas] (
	[temp_type] [nvarchar] (50) NOT NULL,
	[temp_key] [nvarchar] (200) NOT NULL,
	[temp_data] [varbinary] (MAX) NOT NULL,
	[date_created] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_TempDatas] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_TempDatas] PRIMARY KEY  CLUSTERED
	(
		[temp_type],
		[temp_key]
	) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_TempDatas] ALTER COLUMN [temp_key] [nvarchar] (200) NOT NULL;
ALTER TABLE [w2_TempDatas] ALTER COLUMN [temp_type] [nvarchar] (50) NOT NULL;
*/