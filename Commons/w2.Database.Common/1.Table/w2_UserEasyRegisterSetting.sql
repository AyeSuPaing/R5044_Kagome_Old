if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_UserEasyRegisterSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_UserEasyRegisterSetting]
GO
/*
=========================================================================================================
  Module      : ВйВёВ╜ВёЙяИїУoШ^Р▌ТшГ}ГXГ^ (w2_UserEasyRegisterSetting.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_UserEasyRegisterSetting] (
	[item_id] [nvarchar] (30) NOT NULL,
	[display_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_UserEasyRegisterSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_UserEasyRegisterSetting] PRIMARY KEY  CLUSTERED
	(
		[item_id]
	) ON [PRIMARY]
GO
