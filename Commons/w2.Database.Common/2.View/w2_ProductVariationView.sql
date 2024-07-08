if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductVariationView]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[w2_ProductVariationView]
GO
/*
=========================================================================================================
  Module      : 商品バリエーションビュー(w2_ProductVariationView.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
CREATE VIEW [dbo].[w2_ProductVariationView]
            (
              [shop_id],
              [product_id],
              [variation_id],
              [variation_cooperation_id1],
              [variation_cooperation_id2],
              [variation_cooperation_id3],
              [variation_cooperation_id4],
              [variation_cooperation_id5],
              [variation_cooperation_id6],
              [variation_cooperation_id7],
              [variation_cooperation_id8],
              [variation_cooperation_id9],
              [variation_cooperation_id10],
              [mall_variation_id1],
              [mall_variation_id2],
              [mall_variation_type],
              [variation_name1],
              [variation_name2],
              [variation_name3],
              [price],
              [special_price],
              [variation_image_head],
              [variation_image_mobile],
              [shipping_type],
              [shipping_size_kbn],
              [display_order],
              [variation_mall_cooperated_flg],
              [valid_flg],
              [del_flg],
              [date_created],
              [date_changed],
              [last_changed],
              [variation_download_url],
              [variation_fixed_purchase_firsttime_price],
              [variation_fixed_purchase_price],
              [use_variation_flg],
              [fixed_purchase_product_order_limit_flg],
              [product_order_limit_flg_fp],
              [variation_andmall_reservation_flg],
              [variation_color_id],
              [variation_weight_gram],
              [variation_add_cart_url_limit_flg],
              [mall_ex_product_id]
            ) AS
     -- バリエーション使用フラグがOFFの場合は商品データから持ってくる（shop_idとproduct_idはインデックスが効かなくなるためCASEで分岐させない）
     SELECT [dbo].[w2_Product].[shop_id],
            [dbo].[w2_Product].[product_id],
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[product_id]            ELSE [dbo].[w2_ProductVariation].[variation_id]                  END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[cooperation_id1]       ELSE [dbo].[w2_ProductVariation].[variation_cooperation_id1]     END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[cooperation_id2]       ELSE [dbo].[w2_ProductVariation].[variation_cooperation_id2]     END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[cooperation_id3]       ELSE [dbo].[w2_ProductVariation].[variation_cooperation_id3]     END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[cooperation_id4]       ELSE [dbo].[w2_ProductVariation].[variation_cooperation_id4]     END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[cooperation_id5]       ELSE [dbo].[w2_ProductVariation].[variation_cooperation_id5]     END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[cooperation_id6]       ELSE [dbo].[w2_ProductVariation].[variation_cooperation_id6]     END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[cooperation_id7]       ELSE [dbo].[w2_ProductVariation].[variation_cooperation_id7]     END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[cooperation_id8]       ELSE [dbo].[w2_ProductVariation].[variation_cooperation_id8]     END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[cooperation_id9]       ELSE [dbo].[w2_ProductVariation].[variation_cooperation_id9]     END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[cooperation_id10]      ELSE [dbo].[w2_ProductVariation].[variation_cooperation_id10]    END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN ''                                         ELSE [dbo].[w2_ProductVariation].[mall_variation_id1]            END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN ''                                         ELSE [dbo].[w2_ProductVariation].[mall_variation_id2]            END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN ''                                         ELSE [dbo].[w2_ProductVariation].[mall_variation_type]           END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN ''                                         ELSE [dbo].[w2_ProductVariation].[variation_name1]               END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN ''                                         ELSE [dbo].[w2_ProductVariation].[variation_name2]               END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN ''                                         ELSE [dbo].[w2_ProductVariation].[variation_name3]               END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[display_price]         ELSE [dbo].[w2_ProductVariation].[price]                         END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[display_special_price] ELSE [dbo].[w2_ProductVariation].[special_price]                 END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[image_head]            ELSE [dbo].[w2_ProductVariation].[variation_image_head]          END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[image_mobile]          ELSE [dbo].[w2_ProductVariation].[variation_image_mobile]        END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[shipping_type]         ELSE [dbo].[w2_ProductVariation].[shipping_type]                 END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[shipping_size_kbn]     ELSE [dbo].[w2_ProductVariation].[shipping_size_kbn]             END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN 1                                          ELSE [dbo].[w2_ProductVariation].[display_order]                 END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[mall_cooperated_flg]   ELSE [dbo].[w2_ProductVariation].[variation_mall_cooperated_flg] END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[valid_flg]             ELSE [dbo].[w2_ProductVariation].[valid_flg]                     END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[del_flg]               ELSE [dbo].[w2_ProductVariation].[del_flg]                       END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[date_created]          ELSE [dbo].[w2_ProductVariation].[date_created]                  END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[date_changed]          ELSE [dbo].[w2_ProductVariation].[date_changed]                  END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[last_changed]          ELSE [dbo].[w2_ProductVariation].[last_changed]                  END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[download_url]          ELSE [dbo].[w2_ProductVariation].[variation_download_url]                  END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[fixed_purchase_firsttime_price] ELSE [dbo].[w2_ProductVariation].[variation_fixed_purchase_firsttime_price] END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[fixed_purchase_price]  ELSE [dbo].[w2_ProductVariation].[variation_fixed_purchase_price] END,
            [dbo].[w2_Product].[use_variation_flg],
            [dbo].[w2_Product].[fixed_purchase_product_order_limit_flg],
            [dbo].[w2_Product].[product_order_limit_flg_fp],
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[andmall_reservation_flg] ELSE [dbo].[w2_ProductVariation].[variation_andmall_reservation_flg] END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN ''                                         ELSE [dbo].[w2_ProductVariation].[variation_color_id]         END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[product_weight_gram]  ELSE [dbo].[w2_ProductVariation].[variation_weight_gram] END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[add_cart_url_limit_flg]  ELSE [dbo].[w2_ProductVariation].[variation_add_cart_url_limit_flg] END,
            CASE WHEN [dbo].[w2_Product].[use_variation_flg] != '1' THEN [dbo].[w2_Product].[mall_ex_product_id]      ELSE [dbo].[w2_ProductVariation].[mall_variation_id1]               END
       FROM [dbo].[w2_Product]
  LEFT JOIN [dbo].[w2_ProductVariation] ON
            (
              [dbo].[w2_ProductVariation].[shop_id] = [dbo].[w2_Product].[shop_id]
              AND
              [dbo].[w2_ProductVariation].[product_id] = [dbo].[w2_Product].[product_id]
            )
