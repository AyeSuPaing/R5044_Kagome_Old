if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_OrderWorkflowSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_OrderWorkflowSetting]
GO
/*
=========================================================================================================
  Module      : 注文ワークフロー設定(w2_OrderWorkflowSetting.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_OrderWorkflowSetting] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[workflow_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[workflow_no] [int] NOT NULL DEFAULT (1),
	[workflow_ref_no] [int] NOT NULL DEFAULT (0),
	[workflow_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[desc1] [nvarchar] (250) NOT NULL DEFAULT (N''),
	[desc2] [nvarchar] (250) NOT NULL DEFAULT (N''),
	[desc3] [nvarchar] (250) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (1),
	[display_count] [int] NOT NULL DEFAULT (1),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[workflow_detail_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'NORMAL'),
	[display_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'LINE'),
	[additional_search_flg] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[search_setting] [ntext] NOT NULL DEFAULT (N''),
	[order_status_change] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[product_realstock_change] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[payment_status_change] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[external_payment_action] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[demand_status_change] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[return_exchange_status_change] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[repayment_status_change] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[order_extend_status_change1] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change2] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change3] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change4] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change5] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change6] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change7] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change8] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change9] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change10] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mail_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_default_select] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[cassette_no_update] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[cassette_order_status_change] [ntext] NOT NULL DEFAULT (N''),
	[cassette_product_realstock_change] [ntext] NOT NULL DEFAULT (N''),
	[cassette_payment_status_change] [ntext] NOT NULL DEFAULT (N''),
	[cassette_external_payment_action] [ntext] NOT NULL DEFAULT (N''),
	[cassette_demand_status_change] [ntext] NOT NULL DEFAULT (N''),
	[cassette_return_exchange_status_change] [ntext] NOT NULL DEFAULT (N''),
	[cassette_repayment_status_change] [ntext] NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change1] [ntext] NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change2] [ntext] NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change3] [ntext] NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change4] [ntext] NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change5] [ntext] NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change6] [ntext] NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change7] [ntext] NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change8] [ntext] NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change9] [ntext] NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change10] [ntext] NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[order_extend_status_change11] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change12] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change13] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change14] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change15] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change16] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change17] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change18] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change19] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change20] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change21] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change22] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change23] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change24] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change25] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change26] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change27] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change28] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change29] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change30] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change11] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change12] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change13] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change14] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change15] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change16] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change17] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change18] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change19] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change20] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change21] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change22] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change23] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change24] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change25] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change26] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change27] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change28] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change29] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change30] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[order_extend_status_change31] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change32] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change33] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change34] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change35] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change36] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change37] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change38] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change39] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change40] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change31] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change32] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change33] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change34] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change35] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change36] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change37] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change38] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change39] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change40] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[scheduled_shipping_date_action] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[shipping_date_action] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[return_action] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[return_reason_kbn] [nvarchar] (2) NOT NULL DEFAULT (N'01'),
	[return_reason_memo] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[cassette_return_action] [nvarchar] (30) NOT NULL DEFAULT (N'0'),
	[receipt_output_flg_change] [nvarchar] (1) NOT NULL DEFAULT (N''),
	[cassette_receipt_output_flg_change] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[tw_invoice_status_change] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[tw_invoice_status_api] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[tw_cassette_invoice_status_change] [ntext] NOT NULL DEFAULT (N''),
	[tw_cassette_invoice_status_api] [ntext] NOT NULL DEFAULT (N''),
	[tw_external_order_info_action] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[tw_cassette_external_order_info_action] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[order_extend_status_change41] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change42] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change43] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change44] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change45] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change46] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change47] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change48] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change49] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[order_extend_status_change50] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change41] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change42] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change43] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change44] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change45] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change46] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change47] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change48] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change49] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[cassette_order_extend_status_change50] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[storepickup_status_change] [nvarchar] (30) NOT NULL DEFAULT (N'')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_OrderWorkflowSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_OrderWorkflowSetting] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[workflow_kbn],
		[workflow_no]
	) ON [PRIMARY]
GO

/*
--V5.12
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change31] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change32] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change33] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change34] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change35] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change36] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change37] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change38] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change39] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change40] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change31] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change32] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change33] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change34] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change35] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change36] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change37] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change38] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change39] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change40] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [scheduled_shipping_date_action] [nvarchar] (1) NOT NULL DEFAULT (N'0');
*/

/*
-- V5.13
ALTER TABLE [w2_OrderWorkflowSetting] ADD [return_action] [nvarchar] (2) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [return_reason_kbn] [nvarchar] (2) NOT NULL DEFAULT (N'01');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [return_reason_memo] [nvarchar] (MAX) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_return_action] [nvarchar] (30) NOT NULL DEFAULT (N'0');
*/

/*
-- V5.14
ALTER TABLE [w2_OrderWorkflowSetting] ADD [receipt_output_flg_change] [nvarchar] (1) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_receipt_output_flg_change] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [tw_invoice_status_change] [nvarchar] (2) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [tw_invoice_status_api] [nvarchar] (2) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [tw_cassette_invoice_status_change] [ntext] NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [tw_cassette_invoice_status_api] [ntext] NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [tw_external_order_info_action] [nvarchar] (2) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [tw_cassette_external_order_info_action] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [storepickup_status_change] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_storepickup_status_change] [nvarchar] (30) NOT NULL DEFAULT (N'');
*/

/*
-- Order extend status change 41 ~ 50
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change41] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change42] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change43] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change44] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change45] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change46] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change47] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change48] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change49] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [order_extend_status_change50] [nvarchar] (10) NOT NULL DEFAULT (N'0');

-- Cassette order extend status change 41 ~ 50
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change41] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change42] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change43] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change44] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change45] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change46] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change47] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change48] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change49] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_OrderWorkflowSetting] ADD [cassette_order_extend_status_change50] [nvarchar] (max) NOT NULL DEFAULT (N'');
*/
