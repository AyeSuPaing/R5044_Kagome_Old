if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_FieldMemoSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_FieldMemoSetting]
GO
/*
=========================================================================================================
  Module      : НАЦ┌ГБГВР▌ТшГ}ГXГ^(w2_FieldMemoSetting.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_FieldMemoSetting] (
	[table_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[field_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[memo] [ntext] NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FieldMemoSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FieldMemoSetting] PRIMARY KEY  CLUSTERED
	(
		[table_name], 
		[field_name]
	) ON [PRIMARY]
GO
