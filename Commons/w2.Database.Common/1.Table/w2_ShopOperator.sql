if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ShopOperator]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ShopOperator]
GO
/*
=========================================================================================================
  Module      : УXХ▄К╟ЧЭО╥Г}ГXГ^(w2_ShopOperator.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ShopOperator] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[operator_id] [nvarchar] (20) NOT NULL,
	[name] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[mail_addr] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[menu_access_level1] [int],
	[menu_access_level2] [int],
	[menu_access_level3] [int],
	[menu_access_level4] [int],
	[menu_access_level5] [int],
	[menu_access_level6] [int],
	[menu_access_level7] [int],
	[menu_access_level8] [int],
	[menu_access_level9] [int],
	[menu_access_level10] [int],
	[login_id] [nvarchar] (20) COLLATE Japanese_CS_AS_KS_WS NOT NULL DEFAULT (N''),
	[password] [nvarchar] (20) COLLATE Japanese_CS_AS_KS_WS NOT NULL DEFAULT (N''),
	[odbc_user_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[odbc_password] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[usable_advcode_nos_in_report] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[usable_advcode_media_type_ids] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[usable_output_locations] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[date_last_loggedin] [datetime] NOT NULL DEFAULT ('1753/01/01 00:00:00'),
	[remote_addr] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[authentication_code] [nvarchar] (8) NOT NULL DEFAULT (N''),
	[date_code_send] [datetime]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ShopOperator] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ShopOperator] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[operator_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_ShopOperator_1] ON [dbo].[w2_ShopOperator]([shop_id], [login_id], [password]) ON [PRIMARY]
GO

/*
ALTER TABLE [dbo].[w2_ShopOperator] ADD [date_last_loggedin] [datetime] NOT NULL DEFAULT ('1753/01/01 00:00:00');
ALTER TABLE [dbo].[w2_ShopOperator] ADD [remote_addr] [nvarchar] (20) NOT NULL DEFAULT (N'');
ALTER TABLE [dbo].[w2_ShopOperator] ADD [authentication_code] [nvarchar] (8) NOT NULL DEFAULT (N'');
ALTER TABLE [dbo].[w2_ShopOperator] ADD [date_code_send] [datetime];
*/
