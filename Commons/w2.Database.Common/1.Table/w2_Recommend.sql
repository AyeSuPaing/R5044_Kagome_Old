if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_Recommend]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_Recommend]
GO
/*
=========================================================================================================
  Module      : ГМГRГБГУГhР▌Тш (w2_Recommend.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_Recommend] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[recommend_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[recommend_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[discription] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[recommend_display_page] [nvarchar] (50) NOT NULL DEFAULT (N'ORDER_CONFIRM'),
	[recommend_kbn] [nvarchar] (30) NOT NULL DEFAULT (N'UP_SELL'),
	[date_begin] [datetime] NOT NULL DEFAULT (getdate()),
	[date_end] [datetime],
	[priority] [int] NOT NULL DEFAULT (1),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[recommend_display_kbn_pc] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[recommend_display_pc] [nvarchar] ( max) NOT NULL DEFAULT (N''),
	[recommend_display_kbn_sp] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[recommend_display_sp] [nvarchar] ( max) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[onetime_flg] [nvarchar] (10) NOT NULL DEFAULT (N'INVALID'),
	[chatbot_use_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_Recommend] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_Recommend] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[recommend_id]
	) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_Recommend] ADD [chatbot_use_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
*/
