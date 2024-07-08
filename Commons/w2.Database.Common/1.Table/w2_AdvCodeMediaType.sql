if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AdvCodeMediaType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AdvCodeMediaType]
GO
/*
=========================================================================================================
  Module      : НLНРФ}С╠ЛцХкГ}ГXГ^ (w2_AdvCodeMediaType.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_AdvCodeMediaType] (
	[advcode_media_type_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[advcode_media_type_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AdvCodeMediaType] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AdvCodeMediaType] PRIMARY KEY  CLUSTERED
	(
		[advcode_media_type_id]
	) ON [PRIMARY]
GO
