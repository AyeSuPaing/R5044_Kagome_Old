if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MobileOriginalTag]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MobileOriginalTag]
GO
/*
=========================================================================================================
  Module      : モバイルオリジナルタグマスタ(w2_MobileOriginalTag.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MobileOriginalTag] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[orgtag_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[orgtag_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[orgtag_html] [ntext] NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MobileOriginalTag] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MobileOriginalTag] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[orgtag_id]
	) ON [PRIMARY]
GO
