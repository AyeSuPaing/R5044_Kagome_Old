if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_FixedPurchase]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_FixedPurchase]
GO
/*
=========================================================================================================
  Module      : 定期購入情報(w2_FixedPurchase.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_FixedPurchase] (
	[fixed_purchase_id] [nvarchar] (30) NOT NULL,
	[fixed_purchase_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'01'),
	[fixed_purchase_setting1] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[base_tel_no] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[fixed_purchase_status] [nvarchar] (10) NOT NULL DEFAULT (N'10'),
	[payment_status] [nvarchar] (10) NOT NULL DEFAULT (N'10'),
	[last_order_date] [datetime],
	[order_count] [int] NOT NULL DEFAULT (0),
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[supplier_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[order_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'PC'),
	[order_payment_kbn] [nvarchar] (10) DEFAULT (N'00'),
	[fixed_purchase_date_bgn] [datetime] NOT NULL DEFAULT (getdate()),
	[card_kbn] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[credit_branch_no] [int],
	[next_shipping_date] [datetime],
	[next_next_shipping_date] [datetime],
	[fixed_purchase_management_memo] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[card_installments_code] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[extend_status1] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status2] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status3] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status4] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status5] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status6] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status7] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status8] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status9] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status10] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status11] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status12] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status13] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status14] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status15] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status16] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status17] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status18] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status19] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status20] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status21] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status22] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status23] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status24] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status25] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status26] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status27] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status28] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status29] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status30] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[shipped_count] [int] NOT NULL DEFAULT (0),
	[cancel_reason_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cancel_memo] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[next_shipping_use_point] [decimal] NOT NULL DEFAULT (0),
	[extend_status31] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status32] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status33] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status34] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status35] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status36] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status37] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status38] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status39] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status40] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[combined_org_fixedpurchase_ids] [nvarchar] (MAX) DEFAULT (N''),
	[access_country_iso_code] [nvarchar] (3) NOT NULL DEFAULT (N''),
	[disp_language_code] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[disp_language_locale_id] [nvarchar] (7) NOT NULL DEFAULT (N''),
	[disp_currency_code] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[disp_currency_locale_id] [nvarchar] (7) NOT NULL DEFAULT (N''),
	[external_payment_agreement_id] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[cancel_date] [datetime],
	[restart_date] [datetime],
	[memo] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[resume_date] [datetime],
	[suspend_reason] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[shipping_memo] [nvarchar] (MAX) DEFAULT (N''),
	[extend_status_date1] [datetime],
	[extend_status_date2] [datetime],
	[extend_status_date3] [datetime],
	[extend_status_date4] [datetime],
	[extend_status_date5] [datetime],
	[extend_status_date6] [datetime],
	[extend_status_date7] [datetime],
	[extend_status_date8] [datetime],
	[extend_status_date9] [datetime],
	[extend_status_date10] [datetime],
	[extend_status_date11] [datetime],
	[extend_status_date12] [datetime],
	[extend_status_date13] [datetime],
	[extend_status_date14] [datetime],
	[extend_status_date15] [datetime],
	[extend_status_date16] [datetime],
	[extend_status_date17] [datetime],
	[extend_status_date18] [datetime],
	[extend_status_date19] [datetime],
	[extend_status_date20] [datetime],
	[extend_status_date21] [datetime],
	[extend_status_date22] [datetime],
	[extend_status_date23] [datetime],
	[extend_status_date24] [datetime],
	[extend_status_date25] [datetime],
	[extend_status_date26] [datetime],
	[extend_status_date27] [datetime],
	[extend_status_date28] [datetime],
	[extend_status_date29] [datetime],
	[extend_status_date30] [datetime],
	[extend_status_date31] [datetime],
	[extend_status_date32] [datetime],
	[extend_status_date33] [datetime],
	[extend_status_date34] [datetime],
	[extend_status_date35] [datetime],
	[extend_status_date36] [datetime],
	[extend_status_date37] [datetime],
	[extend_status_date38] [datetime],
	[extend_status_date39] [datetime],
	[extend_status_date40] [datetime],
	[next_shipping_use_coupon_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[next_shipping_use_coupon_no] [int] NOT NULL DEFAULT (0),
	[receipt_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[receipt_address] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[receipt_proviso] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[skipped_count] [int] NOT NULL DEFAULT (0),
	[extend_status41] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status42] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status43] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status44] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status45] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status46] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status47] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status48] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status49] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status50] [nvarchar] (10) NOT NULL DEFAULT (N'0'),
	[extend_status_date41] [datetime],
	[extend_status_date42] [datetime],
	[extend_status_date43] [datetime],
	[extend_status_date44] [datetime],
	[extend_status_date45] [datetime],
	[extend_status_date46] [datetime],
	[extend_status_date47] [datetime],
	[extend_status_date48] [datetime],
	[extend_status_date49] [datetime],
	[extend_status_date50] [datetime],
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
	[subscription_box_course_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[subscription_box_order_count] [int] NOT NULL DEFAULT (0),
	[subscription_box_fixed_amount] [decimal] (18,3) DEFAULT (0),
	[use_all_point_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FixedPurchase] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FixedPurchase] PRIMARY KEY  CLUSTERED
	(
		[fixed_purchase_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_FixedPurchase_1] ON [dbo].[w2_FixedPurchase]([user_id]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_FixedPurchase_2] ON [dbo].[w2_FixedPurchase]([next_shipping_date]) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_FixedPurchase] ADD [extend_status1] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status2] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status3] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status4] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status5] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status6] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status7] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status8] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status9] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status10] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status11] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status12] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status13] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status14] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status15] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status16] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status17] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status18] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status19] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status20] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status21] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status22] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status23] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status24] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status25] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status26] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status27] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status28] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status29] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status30] [nvarchar] (10) NOT NULL DEFAULT (N'0');

-- v5.11
DECLARE @TABLE_NAME NVARCHAR(256)
DECLARE @COLUMN_NAME NVARCHAR(256)
DECLARE @TABLE_ID INTEGER 
DECLARE @COLUMN_ID INTEGER 
DECLARE @CONSTRAINT_NAME NVARCHAR(256)
SET @TABLE_NAME = 'w2_FixedPurchase'
SET @COLUMN_NAME = 'order_count'

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
--SELECT @CONSTRAINT_NAME

--制約を削除する
EXEC('ALTER TABLE '+ @TABLE_NAME + ' DROP CONSTRAINT ' + @CONSTRAINT_NAME)

ALTER TABLE [w2_FixedPurchase] ADD DEFAULT (0) FOR [order_count];
ALTER TABLE [w2_FixedPurchase] ADD [shipped_count] [int] NOT NULL DEFAULT (0);
ALTER TABLE [w2_FixedPurchase] ADD [cancel_reason_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchase] ADD [cancel_memo] [nvarchar] (MAX) NOT NULL DEFAULT (N'');

-- v5.12
ALTER TABLE [w2_FixedPurchase] ADD [next_shipping_use_point] [decimal] NOT NULL DEFAULT (0);
ALTER TABLE [w2_FixedPurchase] ADD [extend_status31] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status32] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status33] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status34] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status35] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status36] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status37] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status38] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status39] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status40] [nvarchar] (10) NOT NULL DEFAULT (N'0');

-- v5.13
ALTER TABLE [w2_FixedPurchase] ADD [cancel_date] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [restart_date] [datetime];

-- v5.14
ALTER TABLE [w2_FixedPurchase] ADD [resume_date] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [suspend_reason] [nvarchar] (max) NOT NULL DEFAULT (N'');

ALTER TABLE [w2_FixedPurchase] ADD [shipping_memo] [nvarchar] (MAX) NOT NULL DEFAULT (N'');
-- v5.14
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date1] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date2] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date3] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date4] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date5] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date6] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date7] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date8] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date9] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date10] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date11] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date12] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date13] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date14] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date15] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date16] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date17] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date18] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date19] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date20] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date21] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date22] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date23] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date24] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date25] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date26] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date27] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date28] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date29] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date30] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date31] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date32] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date33] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date34] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date35] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date36] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date37] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date38] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date39] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date40] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [next_shipping_use_coupon_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchase] ADD [next_shipping_use_coupon_no] [int] NOT NULL DEFAULT (0);
ALTER TABLE [w2_Order] ADD [receipt_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Order] ADD [receipt_address] [nvarchar] (100) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Order] ADD [receipt_proviso] [nvarchar] (100) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchase] ADD [skipped_count] [int] NOT NULL DEFAULT (0);

-- Extend status 41 ~ 50
ALTER TABLE [w2_FixedPurchase] ADD [extend_status41] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status42] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status43] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status44] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status45] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status46] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status47] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status48] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status49] [nvarchar] (10) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_FixedPurchase] ADD [extend_status50] [nvarchar] (10) NOT NULL DEFAULT (N'0');

-- Extend status date 41 ~ 50
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date41] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date42] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date43] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date44] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date45] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date46] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date47] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date48] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date49] [datetime];
ALTER TABLE [w2_FixedPurchase] ADD [extend_status_date50] [datetime];

ALTER TABLE [w2_FixedPurchase] ADD [attribute1] [nvarchar] (100) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchase] ADD [attribute2] [nvarchar] (100) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchase] ADD [attribute3] [nvarchar] (100) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchase] ADD [attribute4] [nvarchar] (100) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchase] ADD [attribute5] [nvarchar] (100) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchase] ADD [attribute6] [nvarchar] (100) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchase] ADD [attribute7] [nvarchar] (100) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchase] ADD [attribute8] [nvarchar] (100) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchase] ADD [attribute9] [nvarchar] (100) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_FixedPurchase] ADD [attribute10] [nvarchar] (100) NOT NULL DEFAULT (N'');

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'w2_FixedPurchase' AND COLUMN_NAME = 'subscription_box_course_id')
    ALTER TABLE [w2_FixedPurchase] ADD subscription_box_course_id NVARCHAR(30) NOT NULL DEFAULT (N'');
GO

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'w2_FixedPurchase' AND COLUMN_NAME = 'subscription_box_order_count')
    ALTER TABLE [w2_FixedPurchase] ADD subscription_box_order_count INT;
GO

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'w2_FixedPurchase' AND COLUMN_NAME = 'subscription_box_fixed_amount')
    ALTER TABLE [dbo].[w2_FixedPurchase] ADD [subscription_box_fixed_amount] [decimal] (18,3);
GO

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'w2_FixedPurchase' AND COLUMN_NAME = 'use_all_point_flg')
    ALTER TABLE [dbo].[w2_FixedPurchase] ADD [use_all_point_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
GO
*/