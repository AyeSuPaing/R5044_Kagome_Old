if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_Number]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_Number]
GO
/*
=========================================================================================================
  Module      : Н╠Ф╘Г}ГXГ^(w2_Number.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_Number] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[number_code] [nvarchar] (30) COLLATE Japanese_CS_AS_KS_WS NOT NULL,
	[number] [bigint] NOT NULL DEFAULT (0),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_Number] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_Number] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[number_code]
	) ON [PRIMARY]
GO
