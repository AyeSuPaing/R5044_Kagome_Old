if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsShareInfo]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsShareInfo]
GO
/*
=========================================================================================================
  Module      : 共有情報マスタ(w2_CsShareInfo.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsShareInfo] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[info_no] [bigint] NOT NULL DEFAULT (1),
	[info_text_kbn] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[info_text] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[info_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[info_importance] [int] NOT NULL DEFAULT (3),
	[sender] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsShareInfo] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsShareInfo] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[info_no]
	) ON [PRIMARY]
GO
