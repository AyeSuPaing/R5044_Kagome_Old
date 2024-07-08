if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_LandingPageDesign]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_LandingPageDesign]
GO
/*
=========================================================================================================
  Module      : Lpページデザイン (w2_LandingPageDesign.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_LandingPageDesign] (
	[page_id] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[page_title] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[page_file_name] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[public_status] [nvarchar] (20) NOT NULL DEFAULT (N'0'),
	[public_start_datetime] [datetime],
	[public_end_datetime] [datetime],
	[product_choose_type] [nvarchar] (20) NOT NULL DEFAULT (N'DONOTCHOOSE'),
	[user_registration_type] [nvarchar] (20) NOT NULL DEFAULT (N'AUTO'),
	[login_form_type] [nvarchar] (20) NOT NULL DEFAULT (N'VISIBLE'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[management_title] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[metadata_desc] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[social_login_use_type] [nvarchar] (20) NOT NULL DEFAULT (N'ALL'),
	[social_login_list] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[efo_cube_use_flg] [nvarchar] (20) NOT NULL DEFAULT (N'ON'),
	[order_confirm_page_skip_flg] [nvarchar] (20) NOT NULL DEFAULT (N'OFF'),
	[mail_address_confirm_form_use_flg] [nvarchar] (20) NOT NULL DEFAULT (N'ON'),
	[unpermitted_payment_ids] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[tag_setting_list] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[default_payment_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[novelty_use_flg] [nvarchar] (20) NOT NULL DEFAULT (N'OFF'),
	[design_mode] [nvarchar] (20) NOT NULL DEFAULT (N'DEFAULT'),
	[payment_choose_type] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[personal_authentication_use_flg] [nvarchar] (10) NOT NULL DEFAULT (N'OFF')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_LandingPageDesign] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_LandingPageDesign] PRIMARY KEY  CLUSTERED
	(
		[page_id]
	) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_LandingPageDesign] ADD [novelty_use_flg] [nvarchar] (20) NOT NULL DEFAULT (N'OFF');
-- v5.14
ALTER TABLE [w2_LandingPageDesign] ADD [design_mode] [nvarchar] (20) NOT NULL DEFAULT (N'DEFAULT');
*/

/*
ALTER TABLE [w2_LandingPageDesign] ADD [personal_authentication_use_flg] [nvarchar] (10) NOT NULL DEFAULT (N'OFF');
*/