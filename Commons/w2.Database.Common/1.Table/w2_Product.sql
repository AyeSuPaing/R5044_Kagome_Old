if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_Product]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_Product]
GO
/*
=========================================================================================================
  Module      : 商品マスタ(w2_Product.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_Product] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[supplier_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cooperation_id1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cooperation_id2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cooperation_id3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cooperation_id4] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cooperation_id5] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mall_ex_product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[maker_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[maker_name] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[name] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[name_kana] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[name2] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[name2_kana] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[seo_keywords] [nvarchar] (200) NOT NULL DEFAULT (N''),
	[catchcopy] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[catchcopy_mobile] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[outline_kbn] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[outline] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[outline_kbn_mobile] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[outline_mobile] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[desc_detail_kbn1] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[desc_detail1] [ntext] NOT NULL DEFAULT (N''),
	[desc_detail_kbn2] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[desc_detail2] [ntext] NOT NULL DEFAULT (N''),
	[desc_detail_kbn3] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[desc_detail3] [ntext] NOT NULL DEFAULT (N''),
	[desc_detail_kbn4] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[desc_detail4] [ntext] NOT NULL DEFAULT (N''),
	[return_exchange_message] [ntext] NOT NULL DEFAULT (N''),
	[desc_detail_kbn1_mobile] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[desc_detail1_mobile] [ntext] NOT NULL DEFAULT (N''),
	[desc_detail_kbn2_mobile] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[desc_detail2_mobile] [ntext] NOT NULL DEFAULT (N''),
	[return_exchange_message_mobile] [ntext] NOT NULL DEFAULT (N''),
	[note] [ntext] NOT NULL DEFAULT (N''),
	[display_price] [decimal] (18,3) NOT NULL DEFAULT (0),
	[display_special_price] [decimal] (18,3),
	[tax_included_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[tax_rate] [decimal] (5,2) NOT NULL DEFAULT (0),
	[tax_round_type] [nvarchar] (20) NOT NULL DEFAULT (N'ROUNDDOWN'),
	[price_pretax] [decimal] (18,3) NOT NULL DEFAULT (0),
	[price_shipping] [decimal],
	[shipping_type] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[shipping_size_kbn] [nvarchar] (4),
	[price_cost] [decimal],
	[point_kbn1] [nvarchar] (1) DEFAULT (N'0'),
	[point1] [decimal] NOT NULL DEFAULT (0),
	[point_kbn2] [nvarchar] (1) DEFAULT (N'0'),
	[point2] [decimal] NOT NULL DEFAULT (0),
	[point_kbn3] [nvarchar] (1) DEFAULT (N'0'),
	[point3] [decimal] NOT NULL DEFAULT (0),
	[campaign_from] [datetime],
	[campaign_to] [datetime],
	[campaign_point_kbn] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[campaign_point] [int] NOT NULL DEFAULT (0),
	[display_from] [datetime] NOT NULL,
	[display_to] [datetime],
	[sell_from] [datetime] NOT NULL,
	[sell_to] [datetime],
	[before_sale_display_kbn] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[after_sale_display_kbn] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[max_sell_quantity] [int] NOT NULL DEFAULT (1),
	[stock_management_kbn] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[stock_disp_kbn] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[stock_message_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[url] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[inquire_email] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[inquire_tel] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[display_kbn] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[category_id1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[category_id2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[category_id3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[category_id4] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[category_id5] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[related_product_id_cs1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[related_product_id_cs2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[related_product_id_cs3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[related_product_id_cs4] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[related_product_id_cs5] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[related_product_id_us1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[related_product_id_us2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[related_product_id_us3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[related_product_id_us4] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[related_product_id_us5] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[image_head] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[image_mobile] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[icon_flg1] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[icon_term_end1] [datetime],
	[icon_flg2] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[icon_term_end2] [datetime],
	[icon_flg3] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[icon_term_end3] [datetime],
	[icon_flg4] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[icon_term_end4] [datetime],
	[icon_flg5] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[icon_term_end5] [datetime],
	[icon_flg6] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[icon_term_end6] [datetime],
	[icon_flg7] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[icon_term_end7] [datetime],
	[icon_flg8] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[icon_term_end8] [datetime],
	[icon_flg9] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[icon_term_end9] [datetime],
	[icon_flg10] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[icon_term_end10] [datetime],
	[mobile_disp_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[use_variation_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[reservation_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[fixed_purchase_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[mall_cooperated_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[search_keyword] [nvarchar] (500) NOT NULL DEFAULT (N''),
	[member_rank_discount_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[display_member_rank] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[buyable_member_rank] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[google_shopping_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[product_option_settings] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[arrival_mail_valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[release_mail_valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[resale_mail_valid_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[select_variation_kbn] [nvarchar] (20) NOT NULL DEFAULT (N'STANDARD'),
	[select_variation_kbn_mobile] [nvarchar] (20) NOT NULL DEFAULT (N'STANDARD'),
	[brand_id1] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[brand_id2] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[brand_id3] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[brand_id4] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[brand_id5] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[use_recommend_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[plural_shipping_price_free_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[age_limit_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[gift_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[digital_contents_flg] [nvarchar] (1) NOT NULL DEFAULT (N'1'),
	[download_url] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[display_sell_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[display_priority] [int] NOT NULL DEFAULT (0),
	[limited_payment_ids] [nvarchar] (250) NOT NULL DEFAULT (N''),
	[fixed_purchase_firsttime_price] [decimal] (18,3),
	[fixed_purchase_price] [decimal] (18,3),
	[bundle_item_display_type] [nvarchar] (10) NOT NULL DEFAULT (N'VALID'),
	[product_type] [nvarchar] (10) NOT NULL DEFAULT (N'PRODUCT'),
	[limited_fixed_purchase_kbn1_setting] [nvarchar](200) NOT NULL DEFAULT (N''),
	[limited_fixed_purchase_kbn3_setting] [nvarchar](200) NOT NULL DEFAULT (N''),
	[recommend_product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[cooperation_id6] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[cooperation_id7] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[cooperation_id8] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[cooperation_id9] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[cooperation_id10] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[fixed_purchase_product_order_limit_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[andmall_reservation_flg] [nvarchar] (3) NOT NULL DEFAULT (N'001'),
	[product_color_id] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[display_only_fixed_purchase_member_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[sell_only_fixed_purchase_member_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[product_weight_gram] [int] NOT NULL DEFAULT (0),
	[tax_category_id] [nvarchar] (30) NOT NULL DEFAULT (N'default'),
	[product_order_limit_flg_fp] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[fixed_purchase_cancelable_count] [int] NOT NULL DEFAULT (0),
	[fixed_purchase_limited_user_level_ids] [nvarchar](200) NOT NULL DEFAULT (N''),
	[fixed_purchase_next_shipping_product_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[fixed_purchase_next_shipping_variation_id] [nvarchar] (60) NOT NULL DEFAULT (N''),
	[fixed_purchase_next_shipping_item_quantity] [int] NOT NULL DEFAULT (0),
	[fixed_purchase_limited_skipped_count] [int],
	[next_shipping_item_fixed_purchase_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[next_shipping_item_fixed_purchase_setting] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[product_size_factor] [int] NOT NULL DEFAULT(1),
	[limited_fixed_purchase_kbn4_setting] [nvarchar](50) NOT NULL DEFAULT (N''),
	[subscription_box_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0'),
	[member_rank_point_exclude_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[storepickup_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[exclude_free_shipping_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_Product] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_Product] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[product_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_Product_1] ON [dbo].[w2_Product]([shop_id], [display_kbn], [display_from], [display_to]) ON [PRIMARY]
GO

/*
ALTER TABLE [w2_Product] ADD [product_option_settings] [nvarchar] (max) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Product] ADD [select_variation_kbn] [nvarchar] (20) NOT NULL DEFAULT (N'STANDARD');
ALTER TABLE [w2_Product] ADD [select_variation_kbn_mobile] [nvarchar] (20) NOT NULL DEFAULT (N'STANDARD');
ALTER TABLE [w2_Product] ADD [gift_flg] [nvarchar] (2) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Product] ADD [limited_payment_ids] [nvarchar] (250) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Product] ADD [bundle_item_display_type] [nvarchar] (10) NOT NULL DEFAULT (N'VALID');
ALTER TABLE [w2_Product] ADD [product_type] [nvarchar] (10) NOT NULL DEFAULT (N'PRODUCT');
ALTER TABLE dbo.w2_Product ADD [limited_fixed_purchase_kbn1_setting] [nvarchar](200) NOT NULL DEFAULT (N'');
ALTER TABLE dbo.w2_Product ADD [limited_fixed_purchase_kbn3_setting] [nvarchar](200) NOT NULL DEFAULT (N'');
ALTER TABLE dbo.w2_Product ADD [limited_fixed_purchase_kbn4_setting] [nvarchar](50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Product] ADD [recommend_product_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Product] ADD [cooperation_id6] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Product] ADD [cooperation_id7] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Product] ADD [cooperation_id8] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Product] ADD [cooperation_id9] [nvarchar] (50) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Product] ADD [cooperation_id10] [nvarchar] (50) NOT NULL DEFAULT (N'');
EXEC sp_rename '[w2_Product].check_fixed_purchase_firsttime_flg', 'fixed_purchase_product_order_limit_flg', 'COLUMN';
ALTER TABLE [w2_Product] ADD [product_order_limit_flg_fp] [nvarchar](1) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Product] ADD [product_size_factor] [int] NOT NULL DEFAULT (1);
*/

/*
ALTER TABLE [w2_Product] ADD [andmall_reservation_flg] [nvarchar] (3) NOT NULL DEFAULT (N'001');
ALTER TABLE [w2_Product] ADD [display_only_fixed_purchase_member_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Product] ADD [sell_only_fixed_purchase_member_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Product] ADD [product_weight_gram] [int] NOT NULL DEFAULT (0);
*/

/*
■ 決済通貨対応
ALTER TABLE [w2_Product] ALTER COLUMN [display_price] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Product] ALTER COLUMN [display_special_price] [decimal] (18,3);
ALTER TABLE [w2_Product] ALTER COLUMN [price_pretax] [decimal] (18,3) NOT NULL;
ALTER TABLE [w2_Product] ALTER COLUMN [fixed_purchase_firsttime_price] [decimal] (18,3);
ALTER TABLE [w2_Product] ALTER COLUMN [fixed_purchase_price] [decimal] (18,3);
*/

/*
-- V5.13
ALTER TABLE [w2_Product] ADD [fixed_purchase_next_shipping_product_id] [nvarchar] (30) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Product] ADD [fixed_purchase_next_shipping_variation_id] [nvarchar] (60) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Product] ADD [fixed_purchase_next_shipping_item_quantity] [int] NOT NULL DEFAULT (0);
ALTER TABLE [w2_Product] ADD [fixed_purchase_limited_skipped_count] [int];
ALTER TABLE [w2_Product] ADD [next_shipping_item_fixed_purchase_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'');
ALTER TABLE [w2_Product] ADD [next_shipping_item_fixed_purchase_setting] [nvarchar] (10) NOT NULL DEFAULT (N'');
*/

/*
-- V5.14
ALTER TABLE [w2_Product] ADD [member_rank_point_exclude_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Product] ADD [storepickup_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
ALTER TABLE [w2_Product] ADD [exclude_free_shipping_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0');
*/
