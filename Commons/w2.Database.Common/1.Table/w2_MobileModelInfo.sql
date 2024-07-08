if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_MobileModelInfo]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_MobileModelInfo]
GO
/*
=========================================================================================================
  Module      : モバイル機種情報マスタ(w2_MobileModelInfo.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_MobileModelInfo] (
	[career] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[model_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[maker] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[nickname] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[sale_date] [nvarchar] (20),
	[series] [nvarchar] (20),
	[markup_language] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[browser_ver] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[transmission_speed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[img_gradation] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[disp_gif_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[disp_jpeg_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[disp_png_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[disp_bmp2_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[disp_bmp4_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[disp_mng_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[disp_gif_anime_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[disp_gif_trans_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[communication_method] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[ssl_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[camera_pixel] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[camera_pixel2] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[app_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[app_type] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[app_ver] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[app_size] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[memory_type] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[chords_num] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[chakuuta_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[chakuuta_full_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[chakumovie_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[qr_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[felica_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[bluetooth_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[flash_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[rootca_verisign_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[rootca_entrust_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[rootca_cybertrust_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[rootca_geotrust_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[rootca_rsasecurity_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[qvga_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fullbrowser_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[infrared_rays_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[file_viewer_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[gps_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[movie_type] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[fullbrowser_ver] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[fullbrowser_uagent] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[mail_rcv_size] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[flash_ver] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cache_size] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[oneseg_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[tv_phone_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[officedoc_dl_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[officedoc_disp_size] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[coolie_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[deicedoce_send_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mobile_suica_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mail_url_size] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[bookmark_url_size] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[browser_url_size] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[pushinfo_send_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mail_subject_size] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mail_send_size] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[html_mail_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[kisekae_flg] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[browser_img_size] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[info_update_date] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_MobileModelInfo] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_MobileModelInfo] PRIMARY KEY  CLUSTERED
	(
		[career],
		[model_name]
	) ON [PRIMARY]
GO
