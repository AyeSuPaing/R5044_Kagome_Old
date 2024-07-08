if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ManagerListDispSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_ManagerListDispSetting]
GO
/*
=========================================================================================================
  Module      : Х\ОжР▌ТшК╟ЧЭ (w2_ManagerListDispSetting.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_ManagerListDispSetting] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[disp_setting_kbn] [nvarchar] (30) NOT NULL,
	[disp_colmun_name] [nvarchar] (40) NOT NULL,
	[disp_flag] [nvarchar] (1) DEFAULT (N'1'),
	[disp_order] [int] NOT NULL,
	[colmun_width] [int] NOT NULL,
	[colmun_align] [nvarchar] (15) NOT NULL,
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ManagerListDispSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ManagerListDispSetting] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[disp_setting_kbn],
		[disp_colmun_name]
	) ON [PRIMARY]
GO
