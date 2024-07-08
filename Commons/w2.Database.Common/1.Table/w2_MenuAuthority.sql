if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MenuAuthority]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MenuAuthority]
GO
/*
=========================================================================================================
  Module      : メニュー権限管理マスタ(w2_MenuAuthority.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MenuAuthority] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[pkg_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[menu_authority_level] [int] NOT NULL DEFAULT (0),
	[menu_authority_name] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[menu_path] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[function_level] [int] NOT NULL DEFAULT (0),
	[default_disp_flg] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MenuAuthority] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MenuAuthority] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[pkg_kbn],
		[menu_authority_level],
		[menu_path]
	) ON [PRIMARY]
GO

/*
-- v5.11
ALTER TABLE [w2_MenuAuthority] ALTER COLUMN [menu_path] [nvarchar] (100) NOT NULL;

-- V5.12
UPDATE [w2_MenuAuthority] SET [function_level] = 1 WHERE [menu_path] = 'Form/Top/';
*/