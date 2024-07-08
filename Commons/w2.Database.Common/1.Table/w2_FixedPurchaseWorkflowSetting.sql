IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[w2_FixedPurchaseWorkflowSetting]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_FixedPurchaseWorkflowSetting]
GO
/*
=========================================================================================================
  Module      : FixedPurchase Workflow Setting Table (w2_FixedPurchaseWorkflowSetting.sql)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_FixedPurchaseWorkflowSetting] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[workflow_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[workflow_no] [int] NOT NULL DEFAULT (1),
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
	[fixed_purchase_is_alive_change] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[fixed_purchase_payment_status_change] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[next_shipping_date_change] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[next_next_shipping_date_change] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change1] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change2] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change3] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change4] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change5] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change6] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change7] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change8] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change9] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change10] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_default_select] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[cassette_no_update] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[cassette_fixed_purchase_is_alive_change] [nvarchar] (MAX) NOT NULL DEFAULT (N'0'),
	[cassette_fixed_purchase_payment_status_change] [nvarchar] (MAX) NOT NULL DEFAULT (N'0'),
	[cassette_next_shipping_date_change] [nvarchar] (MAX) NOT NULL DEFAULT (N'0'),
	[cassette_next_next_shipping_date_change] [nvarchar] (MAX) NOT NULL DEFAULT (N'0'),
	[cassette_fixed_purchase_extend_status_change1] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change2] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change3] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change4] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change5] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change6] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change7] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change8] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change9] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change10] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change11] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change12] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change13] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change14] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change15] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change16] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change17] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change18] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change19] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change20] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change21] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change22] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change23] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change24] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change25] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change26] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change27] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change28] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change29] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change30] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change11] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change12] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change13] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change14] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change15] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change16] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change17] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change18] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change19] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change20] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change21] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change22] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change23] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change24] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change25] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change26] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change27] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change28] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change29] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change30] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change31] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change32] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change33] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change34] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change35] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change36] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change37] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change38] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change39] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change40] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change31] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change32] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change33] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change34] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change35] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change36] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change37] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change38] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change39] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change40] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[cancel_reason_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cancel_memo] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change41] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change42] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change43] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change44] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change45] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change46] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change47] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change48] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change49] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_extend_status_change50] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change41] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change42] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change43] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change44] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change45] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change46] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change47] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change48] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change49] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_extend_status_change50] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_stop_unavailable_shipping_area_change] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[cassette_fixed_purchase_stop_unavailable_shipping_area_change] [nvarchar] (MAX) NOT NULL DEFAULT (N'0'),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FixedPurchaseWorkflowSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FixedPurchaseWorkflowSetting] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[workflow_kbn],
		[workflow_no]
	) ON [PRIMARY]
GO

/*
-- Fixed purchase extend status change 41 ~ 50
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [fixed_purchase_extend_status_change41] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [fixed_purchase_extend_status_change42] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [fixed_purchase_extend_status_change43] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [fixed_purchase_extend_status_change44] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [fixed_purchase_extend_status_change45] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [fixed_purchase_extend_status_change46] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [fixed_purchase_extend_status_change47] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [fixed_purchase_extend_status_change48] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [fixed_purchase_extend_status_change49] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [fixed_purchase_extend_status_change50] [nvarchar] (10) NOT NULL DEFAULT (N'0');

-- Cassette fixed purchase extend status change 41 ~ 50
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [cassette_fixed_purchase_extend_status_change41] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [cassette_fixed_purchase_extend_status_change42] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [cassette_fixed_purchase_extend_status_change43] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [cassette_fixed_purchase_extend_status_change44] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [cassette_fixed_purchase_extend_status_change45] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [cassette_fixed_purchase_extend_status_change46] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [cassette_fixed_purchase_extend_status_change47] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [cassette_fixed_purchase_extend_status_change48] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [cassette_fixed_purchase_extend_status_change49] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchaseWorkflowSetting] ADD [cassette_fixed_purchase_extend_status_change50] [nvarchar] (max) NOT NULL DEFAULT (N'');
*/