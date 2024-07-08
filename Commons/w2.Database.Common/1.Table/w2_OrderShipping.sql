if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_OrderShipping]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_OrderShipping]
GO
/*
=========================================================================================================
  Module      : ТНХ╢ФzСЧРцПюХё(w2_OrderShipping.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_OrderShipping] (
	[order_id] [nvarchar] (30) NOT NULL,
	[order_shipping_no] [int] NOT NULL DEFAULT (1),
	[shipping_name] [nvarchar] (40) NOT NULL DEFAULT (N''),
	[shipping_name_kana] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[shipping_zip] [nvarchar] (8) NOT NULL DEFAULT (N''),
	[shipping_addr1] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_addr2] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_addr3] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_addr4] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_tel1] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[shipping_tel2] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[shipping_tel3] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[shipping_fax] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[shipping_company] [nvarchar] (30) NOT NULL DEFAULT (N'0'),
	[shipping_date] [datetime],
	[shipping_time] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_check_no] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[shipping_name1] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[shipping_name2] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[shipping_name_kana1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_name_kana2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[sender_name] [nvarchar] (40) NOT NULL DEFAULT (N''),
	[sender_name1] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[sender_name2] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[sender_name_kana] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[sender_name_kana1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[sender_name_kana2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[sender_zip] [nvarchar] (8) NOT NULL DEFAULT (N''),
	[sender_addr1] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[sender_addr2] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[sender_addr3] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[sender_addr4] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[sender_tel1] [nvarchar] (16) NOT NULL DEFAULT (N''),
	[wrapping_paper_type] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[wrapping_paper_name] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[wrapping_bag_type] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_company_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_company_post_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[sender_company_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[sender_company_post_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[private_shipping_flg] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[another_shipping_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[shipping_method] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[delivery_company_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[shipping_country_iso_code] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[shipping_country_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[shipping_addr5] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[sender_country_iso_code] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[sender_country_name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[sender_addr5] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[scheduled_shipping_date] [datetime],
	[external_shipping_cooperation_id] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[shipping_receiving_store_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[shipping_receiving_store_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[shipping_external_deliverty_status] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_status] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[shipping_status_update_date] [datetime],
	[shipping_receiving_mail_date] [datetime],
	[shipping_receiving_store_type] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_status_code] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_office_name] [nvarchar] (15) NOT NULL DEFAULT (N''),
	[shipping_handy_time] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[shipping_current_status] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_status_detail] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[storepickup_real_shop_id] [nvarchar] (30) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_OrderShipping] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_OrderShipping] PRIMARY KEY  CLUSTERED
	(
		[order_id],
		[order_shipping_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_OrderShipping_1] ON [dbo].[w2_OrderShipping]([shipping_external_deliverty_status]) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_OrderShipping] ADD [sender_name] [nvarchar] (40) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [sender_name1] [nvarchar] (20) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [sender_name2] [nvarchar] (20) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [sender_name_kana] [nvarchar] (60) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [sender_name_kana1] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [sender_name_kana2] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [sender_zip] [nvarchar] (8) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [sender_addr1] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [sender_addr2] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [sender_addr3] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [sender_addr4] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [sender_tel1] [nvarchar] (16) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [wrapping_paper_type] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [wrapping_paper_name] [nvarchar] (200) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [wrapping_bag_type] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [scheduled_shipping_date] [datetime];
IF NOT EXISTS (SELECT column_name FROM INFORMATION_SCHEMA.columns WHERE table_name = 'w2_OrderShipping' AND column_name = 'another_shipping_flg')
	ALTER TABLE [w2_OrderShipping] ADD [another_shipping_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
GO
ALTER TABLE [w2_OrderShipping] ADD [external_shipping_cooperation_id] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [shipping_receiving_store_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderShipping] ADD [shipping_receiving_store_id] [nvarchar] (20) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [shipping_external_deliverty_status] [nvarchar] (5) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [shipping_status] [nvarchar] (10) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [shipping_status_update_date] [datetime];
ALTER TABLE [w2_OrderShipping] ADD [shipping_receiving_mail_date] [datetime];
ALTER TABLE [w2_OrderShipping] ADD [shipping_receiving_store_type] [nvarchar] (5) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [shipping_status_code] [nvarchar] (5) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [shipping_office_name] [nvarchar] (15) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [shipping_handy_time] [nvarchar] (20) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [shipping_current_status] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [shipping_status_detail] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderShipping] ADD [storepickup_real_shop_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
*/
