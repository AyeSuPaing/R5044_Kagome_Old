if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_DeliveryCompany]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_DeliveryCompany]
GO
/*
=========================================================================================================
  Module      : 配送会社マスタ (w2_DeliveryCompany.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_DeliveryCompany] (
	[delivery_company_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[delivery_company_name] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[delivery_company_type_creditcard] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_set_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[shipping_time_id1] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id2] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id3] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id4] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message4] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id5] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message5] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id6] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message6] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id7] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message7] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id8] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message8] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id9] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message9] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[shipping_time_id10] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[shipping_time_message10] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[display_order] [int] NOT NULL DEFAULT (1),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[delivery_lead_time_set_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[shipping_lead_time_default] [int] NOT NULL DEFAULT (2),
	[delivery_company_type_post_payment] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[delivery_company_type_post_np_payment] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[deadline_time] [nvarchar] (5) NOT NULL DEFAULT (N''),
	[delivery_company_type_gooddeal][nvarchar] (3) NOT NULL DEFAULT (N''),
	[delivery_company_type_gmo_atokara_payment][nvarchar] (3) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_DeliveryCompany] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_DeliveryCompany] PRIMARY KEY  CLUSTERED
	(
		[delivery_company_id]
	) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_DeliveryCompany] ADD [delivery_lead_time_set_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_DeliveryCompany] ADD [shipping_lead_time_default] [int] NOT NULL DEFAULT (2);
EXEC sp_rename '[w2_DeliveryCompany].delivery_company_type', 'delivery_company_type_creditcard', 'COLUMN';
ALTER TABLE [w2_DeliveryCompany] ADD [delivery_company_type_post_payment] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_DeliveryCompany] ADD [delivery_company_type_post_np_payment] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_DeliveryCompany] ADD [deadline_time] [nvarchar] (5) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_DeliveryCompany] ADD [delivery_company_type_gooddeal] [nvarchar] (3) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_DeliveryCompany] ADD [delivery_company_type_gmo_atokara_payment] [nvarchar] (3) NOT NULL DEFAULT (N'');
*/
