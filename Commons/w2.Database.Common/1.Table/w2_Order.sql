if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_Order]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_Order]
GO
/*
=========================================================================================================
  Module      : 注文情報(w2_Order.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_Order] (
	[order_id] [nvarchar] (30) NOT NULL,
	[order_id_org] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[order_group_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[order_no] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[bundle_child_order_ids] [ntext] NOT NULL DEFAULT (N''),
	[bundle_parent_order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[bundle_order_bak] [ntext] NOT NULL DEFAULT (N''),
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[supplier_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[order_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'PC'),
	[mall_id] [nvarchar] (30) NOT NULL DEFAULT (N'OWN_SITE'),
	[order_payment_kbn] [nvarchar] (10) DEFAULT (N'K90'),
	[order_status] [nvarchar] (30) NOT NULL DEFAULT (N'TMP'),
	[order_date] [datetime] DEFAULT (getdate()),
	[order_recognition_date] [datetime],
	[order_stockreserved_status] [nvarchar] (10) NOT NULL DEFAULT (N'00'),
	[order_stockreserved_date] [datetime],
	[order_shipping_date] [datetime],
	[order_shipped_status] [nvarchar] (10) NOT NULL DEFAULT (N'00'),
	[order_shipped_date] [datetime],
	[order_delivering_date] [datetime],
	[order_cancel_date] [datetime],
	[order_return_date] [datetime],
	[order_payment_status] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[order_payment_date] [datetime],
	[demand_status] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[demand_date] [datetime],
	[order_return_exchange_status] [nvarchar] (10) NOT NULL DEFAULT (N'00'),
	[order_return_exchange_receipt_date] [datetime],
	[order_return_exchange_arrival_date] [datetime],
	[order_return_exchange_complete_date] [datetime],
	[order_repayment_status] [nvarchar] (10) NOT NULL DEFAULT (N'00'),
	[order_repayment_date] [datetime],
	[order_item_count] [int] NOT NULL DEFAULT (0),
	[order_product_count] [int] NOT NULL DEFAULT (0),
	[order_price_subtotal] [decimal] (18,3) NOT NULL DEFAULT (0),
	[order_price_pack] [decimal] NOT NULL DEFAULT (0),
	[order_price_tax] [decimal] (18,3) NOT NULL DEFAULT (0),
	[order_price_shipping] [decimal] (18,3) NOT NULL DEFAULT (0),
	[order_price_exchange] [decimal] (18,3) NOT NULL DEFAULT (0),
	[order_price_regulation] [decimal] (18,3) NOT NULL DEFAULT (0),
	[order_price_repayment] [decimal] (18,3) NOT NULL DEFAULT (0),
	[order_price_total] [decimal] (18,3) NOT NULL DEFAULT (0),
	[order_discount_set_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[order_point_use] [decimal] NOT NULL DEFAULT (0),
	[order_point_use_yen] [decimal] (18,3) NOT NULL DEFAULT (0),
	[order_point_add] [decimal] NOT NULL DEFAULT (0),
	[order_point_rate] [decimal] NOT NULL DEFAULT (0),
	[order_coupon_use] [decimal] (18,3) NOT NULL DEFAULT (0),
	[card_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[card_instruments] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[card_tran_id] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[shipping_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[fixed_purchase_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[advcode_first] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[advcode_new] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipped_changed_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'01'),
	[return_exchange_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'00'),
	[return_exchange_reason_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'00'),
	[attribute1] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute2] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute3] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute4] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute5] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute6] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute7] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute8] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute9] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[attribute10] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[extend_status1] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date1] [datetime],
	[extend_status2] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date2] [datetime],
	[extend_status3] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date3] [datetime],
	[extend_status4] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date4] [datetime],
	[extend_status5] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date5] [datetime],
	[extend_status6] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date6] [datetime],
	[extend_status7] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date7] [datetime],
	[extend_status8] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date8] [datetime],
	[extend_status9] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date9] [datetime],
	[extend_status10] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date10] [datetime],
	[career_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mobile_uid] [nvarchar] (50) COLLATE Japanese_CS_AS_KS_WS NOT NULL DEFAULT (N''),
	[remote_addr] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[memo] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[wrapping_memo] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[payment_memo] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[management_memo] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[relation_memo] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[return_exchange_reason_memo] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[regulation_memo] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[repayment_memo] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[member_rank_discount_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[member_rank_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[credit_branch_no] [int],
	[affiliate_session_name1] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[affiliate_session_value1] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[affiliate_session_name2] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[affiliate_session_value2] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[user_agent] [nvarchar] (512) NOT NULL DEFAULT (N''),
	[gift_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[digital_contents_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[card_3dsecure_tran_id] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[card_3dsecure_auth_url] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[card_3dsecure_auth_key] [nvarchar] (1000) NOT NULL DEFAULT (N''),
	[shipping_price_separate_estimates_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[order_tax_included_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[order_tax_rate] [decimal] (5,2) NOT NULL DEFAULT (0),
	[order_tax_round_type] [nvarchar] (20) NOT NULL DEFAULT (N'ROUNDDOWN'),
	[extend_status11] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date11] [datetime],
	[extend_status12] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date12] [datetime],
	[extend_status13] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date13] [datetime],
	[extend_status14] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date14] [datetime],
	[extend_status15] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date15] [datetime],
	[extend_status16] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date16] [datetime],
	[extend_status17] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date17] [datetime],
	[extend_status18] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date18] [datetime],
	[extend_status19] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date19] [datetime],
	[extend_status20] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date20] [datetime],
	[extend_status21] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date21] [datetime],
	[extend_status22] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date22] [datetime],
	[extend_status23] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date23] [datetime],
	[extend_status24] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date24] [datetime],
	[extend_status25] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date25] [datetime],
	[extend_status26] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date26] [datetime],
	[extend_status27] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date27] [datetime],
	[extend_status28] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date28] [datetime],
	[extend_status29] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date29] [datetime],
	[extend_status30] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date30] [datetime],
	[card_installments_code] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[setpromotion_product_discount_amount] [decimal] (18,3) NOT NULL DEFAULT (0),
	[setpromotion_shipping_charge_discount_amount] [decimal] (18,3) NOT NULL DEFAULT (0),
	[setpromotion_payment_charge_discount_amount] [decimal] (18,3) NOT NULL DEFAULT (0),
	[online_payment_status] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[fixed_purchase_order_count] [int],
	[fixed_purchase_shipped_count] [int],
	[payment_order_id] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[fixed_purchase_discount_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[fixed_purchase_member_discount_amount] [decimal] (18,3) NOT NULL DEFAULT (0),
	[last_billed_amount] [decimal] (18,3) NOT NULL DEFAULT (0),
	[external_payment_status] [nvarchar] (30) NOT NULL DEFAULT (N'NONE'),
	[external_payment_error_message] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[external_payment_auth_date] [datetime],
	[extend_status31] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date31] [datetime],
	[extend_status32] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date32] [datetime],
	[extend_status33] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date33] [datetime],
	[extend_status34] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date34] [datetime],
	[extend_status35] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date35] [datetime],
	[extend_status36] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date36] [datetime],
	[extend_status37] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date37] [datetime],
	[extend_status38] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date38] [datetime],
	[extend_status39] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date39] [datetime],
	[extend_status40] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date40] [datetime],
	[last_order_point_use] [decimal] NOT NULL DEFAULT (0),
	[last_order_point_use_yen] [decimal] (18,3) NOT NULL DEFAULT (0),
	[external_order_id] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[external_import_status] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[combined_org_order_ids] [nvarchar] (max) DEFAULT (N''),
	[last_auth_flg] [nvarchar] (1),
	[mall_link_status] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[order_price_subtotal_tax] [decimal] (18,3) NOT NULL DEFAULT (0),
	[order_price_shipping_tax] [decimal] (18,3) NOT NULL DEFAULT (0),
	[order_price_exchange_tax] [decimal] (18,3) NOT NULL DEFAULT (0),
	[settlement_currency] [nvarchar] (3) NOT NULL DEFAULT (N'JPY'),
	[settlement_rate] [decimal] (24,12) NOT NULL DEFAULT (1),
	[settlement_amount] [decimal] (18,3) NOT NULL DEFAULT (0),
	[shipping_memo] [nvarchar] (MAX) DEFAULT (N''),
	[settlement_amount] [decimal] (18,3) NOT NULL DEFAULT (0),
	[shipping_tax_rate] [decimal](5, 2) NOT NULL DEFAULT (0),
	[payment_tax_rate] [decimal](5, 2) NOT NULL DEFAULT (0),
	[order_count_order] [int],
	[invoice_bundle_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[receipt_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[receipt_output_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[receipt_address] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[receipt_proviso] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[delivery_tran_id] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[online_delivery_status] [nvarchar] (2) NOT NULL DEFAULT (N''),
	[external_payment_type] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[logi_cooperation_status] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend_status41] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date41] [datetime],
	[extend_status42] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date42] [datetime],
	[extend_status43] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date43] [datetime],
	[extend_status44] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date44] [datetime],
	[extend_status45] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date45] [datetime],
	[extend_status46] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date46] [datetime],
	[extend_status47] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date47] [datetime],
	[extend_status48] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date48] [datetime],
	[extend_status49] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date49] [datetime],
	[extend_status50] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date50] [datetime],
	[card_tran_pass] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[subscription_box_course_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[subscription_box_fixed_amount] [decimal] (18,3),
	[subscription_box_order_count] [int],
	[storepickup_status] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[storepickup_store_arrived_date] [datetime],
	[storepickup_delivered_complete_date] [datetime],
	[storepickup_return_date] [datetime],
	[order_opportunity_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_base_id] [nvarchar] (2) NOT NULL DEFAULT (N'01')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_Order] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_Order] PRIMARY KEY  NONCLUSTERED
	(
		[order_id]
	) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_Order_1] ON [dbo].[w2_Order]([order_date]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_Order_2] ON [dbo].[w2_Order]([user_id]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_Order_3] ON [dbo].[w2_Order]([order_status]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_Order_4] ON [dbo].[w2_Order]([order_id_org]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_Order_5] ON [dbo].[w2_Order]([order_shipped_date], [order_delivering_date]) ON [PRIMARY]
GO
/*
ALTER TABLE [w2_Order] ADD [gift_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0');

-- v5.11
ALTER TABLE [w2_Order] ADD [fixed_purchase_order_count] [int];
ALTER TABLE [w2_Order] ADD [fixed_purchase_shipped_count] [int];
-- v5.12
ALTER TABLE [w2_Order] ADD [payment_order_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Order] ADD [last_billed_amount] [decimal] NOT NULL DEFAULT (0);
ALTER TABLE [w2_Order] ADD [external_payment_status] [nvarchar] (30) NOT NULL DEFAULT (N'NONE');
ALTER TABLE [w2_Order] ADD [external_payment_error_message] [nvarchar] (MAX) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Order] ADD [external_payment_auth_date] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status31] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status32] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status33] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status34] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status35] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status36] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status37] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status38] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status39] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status40] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status_date31] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status_date32] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status_date33] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status_date34] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status_date35] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status_date36] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status_date37] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status_date38] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status_date39] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status_date40] [datetime];
ALTER TABLE [w2_Order] ADD [last_order_point_use] [decimal] NOT NULL DEFAULT (0);
ALTER TABLE [w2_Order] ADD [last_order_point_use_yen] [decimal] NOT NULL DEFAULT (0);
ALTER TABLE [w2_Order] ADD [external_order_id] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Order] ADD [external_import_status] [nvarchar] (10) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Order] ADD [last_auth_flg] [nvarchar] (1) NULL;
ALTER TABLE [w2_Order] ADD [mall_link_status] [nvarchar] (20) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Order] ADD [logi_cooperation_status] [nvarchar] (10) NOT NULL DEFAULT (N'');

EXEC w2_AlterColumnTypeSp 'w2_Order', 'memo', 'nvarchar(MAX) NOT NULL';
EXEC w2_AlterColumnTypeSp 'w2_Order', 'wrapping_memo', 'nvarchar(MAX) NOT NULL';
EXEC w2_AlterColumnTypeSp 'w2_Order', 'payment_memo', 'nvarchar(MAX) NOT NULL';
EXEC w2_AlterColumnTypeSp 'w2_Order', 'management_memo', 'nvarchar(MAX) NOT NULL';
EXEC w2_AlterColumnTypeSp 'w2_Order', 'relation_memo', 'nvarchar(MAX) NOT NULL';
EXEC w2_AlterColumnTypeSp 'w2_Order', 'return_exchange_reason_memo', 'nvarchar(MAX) NOT NULL';
EXEC w2_AlterColumnTypeSp 'w2_Order', 'regulation_memo', 'nvarchar(MAX) NOT NULL';
EXEC w2_AlterColumnTypeSp 'w2_Order', 'repayment_memo', 'nvarchar(MAX) NOT NULL';
*/

/*
-- w2_Order
DECLARE @TABLE_NAME NVARCHAR(256)
DECLARE @COLUMN_NAME NVARCHAR(256)
DECLARE @TABLE_ID INTEGER 
DECLARE @COLUMN_ID INTEGER 
DECLARE @CONSTRAINT_NAME NVARCHAR(256)
SET @TABLE_NAME = 'w2_Order'
SET @COLUMN_NAME = 'card_3dsecure_auth_url'

--削除したいテーブルのシステムidを取得する
SELECT @TABLE_ID = id FROM sys.sysobjects 
WHERE xtype = 'U' AND name = @TABLE_NAME

--削除したいカラムのシステムidを取得する
SELECT @COLUMN_ID = column_id FROM sys.columns 
WHERE object_id = @TABLE_ID AND name = @COLUMN_NAME

--削除したい制約名を取得する
SELECT @CONSTRAINT_NAME = name FROM sys.sysobjects 
WHERE id = (SELECT constid FROM sys.sysconstraints WHERE id = @TABLE_ID AND colid = @COLUMN_ID)

--確認 => テーブル->制約->制約名->制約をスクリプト化であっているか確認できます。
SELECT @CONSTRAINT_NAME

--制約を削除する
EXEC('ALTER TABLE '+ @TABLE_NAME + ' DROP CONSTRAINT ' + @CONSTRAINT_NAME)

ALTER TABLE [w2_Order] ALTER COLUMN [card_3dsecure_auth_url] [nvarchar] (MAX) NOT NULL;
ALTER TABLE [w2_Order] ADD DEFAULT (N'') FOR [card_3dsecure_auth_url] ;
*/

/*
■ 決済通貨対応
ALTER TABLE [w2_Order] ALTER COLUMN [order_price_subtotal] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Order] ALTER COLUMN [order_price_shipping] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Order] ALTER COLUMN [order_price_exchange] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Order] ALTER COLUMN [order_price_regulation] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Order] ALTER COLUMN [order_price_repayment] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Order] ALTER COLUMN [order_price_total] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Order] ALTER COLUMN [order_discount_set_price] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Order] ALTER COLUMN [order_point_use_yen] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Order] ALTER COLUMN [order_coupon_use] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Order] ALTER COLUMN [member_rank_discount_price] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Order] ALTER COLUMN [setpromotion_product_discount_amount] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Order] ALTER COLUMN [setpromotion_shipping_charge_discount_amount] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Order] ALTER COLUMN [setpromotion_payment_charge_discount_amount] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Order] ALTER COLUMN [fixed_purchase_discount_price] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Order] ALTER COLUMN [fixed_purchase_member_discount_amount] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Order] ALTER COLUMN [last_billed_amount] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Order] ALTER COLUMN [last_order_point_use_yen] [decimal] (18,3) NOT NULL;
*/

/*
-- v5.14
ALTER TABLE [w2_Order] ADD [shipping_memo] [nvarchar] (MAX) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Order] ADD [invoice_bundle_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'):
ALTER TABLE [w2_Order] ADD [receipt_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [receipt_output_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [receipt_address] [nvarchar] (100) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Order] ADD [receipt_proviso] [nvarchar] (100) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Order] ADD [delivery_tran_id] [nvarchar] (100) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Order] ADD [online_delivery_status] [nvarchar] (2) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Order] ADD [external_payment_type] [nvarchar] (20) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Order] ADD [card_tran_pass] [nvarchar] (100) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Order] ADD [storepickup_status] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Order] ADD [storepickup_store_arrived_date] [datetime];
ALTER TABLE [w2_Order] ADD [storepickup_delivered_complete_date] [datetime];
ALTER TABLE [w2_Order] ADD [storepickup_return_date] [datetime];
*/

/*
-- Extend_status 41 ~ 50 and extend_status_date 41 ~ 50
ALTER TABLE [w2_Order] ADD [extend_status41] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status_date41] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status42] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status_date42] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status43] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status_date43] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status44] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status_date44] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status45] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status_date45] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status46] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status_date46] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status47] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status_date47] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status48] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status_date48] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status49] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status_date49] [datetime];
ALTER TABLE [w2_Order] ADD [extend_status50] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [extend_status_date50] [datetime];
IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'w2_Order' AND COLUMN_NAME = 'subscription_box_course_id')
    ALTER TABLE [w2_Order] ADD subscription_box_course_id NVARCHAR(30) NOT NULL DEFAULT (N'');
GO

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'w2_Order' AND COLUMN_NAME = 'subscription_box_fixed_amount')
    ALTER TABLE [dbo].[w2_Order] ADD [subscription_box_fixed_amount] [decimal] (18,3);
GO

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'w2_Order' AND COLUMN_NAME = 'subscription_box_order_count')
    ALTER TABLE [dbo].[w2_Order] ADD [subscription_box_order_count] [int];
GO
*/

/*
IF NOT EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'w2_Order' AND COLUMN_NAME = 'order_opportunity_id')
	ALTER TABLE [w2_Order] ADD [order_opportunity_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
GO

IF NOT EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'w2_Order' AND COLUMN_NAME = 'shipping_base_id')
	ALTER TABLE [w2_Order] ADD [shipping_base_id] [nvarchar] (2) NOT NULL DEFAULT (N'01');
GO
*/
