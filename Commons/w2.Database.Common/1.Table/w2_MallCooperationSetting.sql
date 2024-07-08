if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MallCooperationSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MallCooperationSetting]
GO
/*
=========================================================================================================
  Module      : モール連携設定マスタ(w2_MallCooperationSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MallCooperationSetting] (
	[shop_id] [nvarchar] (10) NOT NULL,
	[mall_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mall_kbn] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[mall_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[mall_exhibits_config] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[tgt_mail_addr] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[pop_server] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[pop_port] [int] NOT NULL DEFAULT (110),
	[pop_user_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[pop_password] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[pop_apop_flg] [nvarchar] (10) NOT NULL DEFAULT (N'1'),
	[ftp_host] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[ftp_user_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[ftp_password] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[ftp_upload_dir] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[order_import_setting] [ntext] NOT NULL DEFAULT (N''),
	[other_setting] [ntext] NOT NULL DEFAULT (N''),	
	[cnvid_rtn_n_ins_itemcsv] [int],
	[cnvid_rtn_n_upd_itemcsv] [int],
	[cnvid_rtn_n_stk_itemcsv] [int],
	[cnvid_rtn_v_ins_itemcsv] [int],
	[cnvid_rtn_v_ins_selectcsv] [int],
	[cnvid_rtn_v_upd_itemcsv] [int],
	[cnvid_rtn_v_stk_selectcsv] [int],
	[cnvid_rtn_itemcatcsv] [int],
	[cnvid_yho_n_ins_datacsv] [int],
	[cnvid_yho_n_ins_stockcsv] [int],
	[cnvid_yho_n_upd_datacsv] [int],
	[cnvid_yho_n_stk_datacsv] [int],
	[cnvid_yho_v_ins_datacsv] [int],
	[cnvid_yho_v_ins_stockcsv] [int],
	[cnvid_yho_v_upd_datacsv] [int],
	[cnvid_yho_v_stk_datacsv] [int],
	[maintenance_date_from] [datetime],
	[maintenance_date_to] [datetime],
	[valid_flg] [nvarchar] (10) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[last_product_log_no] [bigint],
	[last_productvariation_log_no] [bigint],
	[last_productstock_log_no] [bigint],
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[rakuten_api_user_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[rakuten_api_shop_url] [nvarchar] (255) NOT NULL DEFAULT (N''),
	[rakuten_api_service_secret] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[rakuten_api_license_key] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[amazon_merchant_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[amazon_marketplace_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[amazon_aws_accesskey_id] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[amazon_secret_key] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[amazon_mws_authtoken] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[stock_update_use_flg] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[lohaco_private_key] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[sftp_host] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[sftp_user_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[sftp_pass_phrase] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[sftp_port] [int] NOT NULL DEFAULT (22),
	[sftp_private_key_file_path] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[andmall_tenant_code] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[andmall_base_store_code] [nvarchar] (8) NOT NULL DEFAULT (N''),
	[andmall_shop_no] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[andmall_cooperation] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[andmall_variation_cooperation] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[andmall_site_code] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[andmall_signature_key] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[next_engine_stock_store_account] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[next_engine_stock_auth_key] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[facebook_catalog_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[facebook_access_token] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[yahoo_api_public_key] [nvarchar] (340) NOT NULL DEFAULT (N''),
	[yahoo_api_public_key_version] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[yahoo_api_last_public_key_authorized_at] [datetime],
	[rakuten_sku_management_id_output_format_for_normal] [nvarchar] (512) NOT NULL DEFAULT (N''),
	[rakuten_sku_management_id_output_format_for_variation] [nvarchar] (512) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MallCooperationSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MallCooperationSetting] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[mall_id]
	) ON [PRIMARY]
GO

/*
ALTER TABLE w2_MallCooperationSetting ADD other_setting ntext NOT NULL DEFAULT ''
GO
*/

/*
ALTER TABLE w2_MallCooperationSetting ADD [sftp_host] [nvarchar] (256) NOT NULL DEFAULT (N'');
ALTER TABLE w2_MallCooperationSetting ADD [sftp_user_name] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE w2_MallCooperationSetting ADD [sftp_pass_phrase] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE w2_MallCooperationSetting ADD [sftp_private_key_file_path] [nvarchar] (256) NOT NULL DEFAULT (N'');
ALTER TABLE w2_MallCooperationSetting ADD [sftp_port] [int] NOT NULL DEFAULT (22);
ALTER TABLE w2_MallCooperationSetting ADD [andmall_tenant_code] [nvarchar] (5) NOT NULL DEFAULT (N'');
ALTER TABLE w2_MallCooperationSetting ADD [andmall_base_store_code] [nvarchar] (8) NOT NULL DEFAULT (N'');
ALTER TABLE w2_MallCooperationSetting ADD [andmall_shop_no] [nvarchar] (32) NOT NULL DEFAULT (N'');
ALTER TABLE w2_MallCooperationSetting ADD [andmall_cooperation] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE w2_MallCooperationSetting ADD [andmall_variation_cooperation] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE w2_MallCooperationSetting ADD [andmall_site_code] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE w2_MallCooperationSetting ADD [andmall_signature_key] [nvarchar] (30) NOT NULL DEFAULT (N'');
*/

/*
ALTER TABLE w2_MallCooperationSetting ADD [next_engine_stock_store_account] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE w2_MallCooperationSetting ADD [next_engine_stock_auth_key] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE w2_MallCooperationSetting ADD [facebook_catalog_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE w2_MallCooperationSetting ADD [facebook_access_token] [nvarchar] (256) NOT NULL DEFAULT (N'');
*/

/*
ALTER TABLE w2_MallCooperationSetting ADD yahoo_api_public_key NVARCHAR(340) NOT NULL DEFAULT (N'');
ALTER TABLE w2_MallCooperationSetting ADD yahoo_api_public_key_version NVARCHAR(30) NOT NULL DEFAULT (N'');
ALTER TABLE w2_MallCooperationSetting ADD yahoo_api_last_public_key_authorized_at DATETIME;
*/

/*
ALTER TABLE w2_MallCooperationSetting ADD rakuten_sku_management_id_output_format_for_normal NVARCHAR(512) NOT NULL DEFAULT (N'');
ALTER TABLE w2_MallCooperationSetting ADD rakuten_sku_management_id_output_format_for_variation NVARCHAR(512) NOT NULL DEFAULT (N'');
*/
