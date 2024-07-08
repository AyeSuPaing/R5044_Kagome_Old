if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_Novelty]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_Novelty]
GO
/*
=========================================================================================================
  Module      : ГmГxГЛГeГBР▌Тш(w2_Novelty.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_Novelty] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[novelty_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[novelty_disp_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[novelty_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[discription] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[date_begin] [datetime] NOT NULL DEFAULT (getdate()),
	[date_end] [datetime],
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[auto_additional_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_Novelty] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_Novelty] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[novelty_id]
	) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_Novelty] ADD [auto_additional_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
*/
