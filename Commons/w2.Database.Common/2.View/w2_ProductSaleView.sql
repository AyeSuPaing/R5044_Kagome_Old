if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ProductSaleView]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[w2_ProductSaleView]
GO
/*
=========================================================================================================
  Module      : 商品セール情報ビュー(w2_ProductSaleView.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE VIEW [dbo].[w2_ProductSaleView](
		[shop_id],
		[productsale_id],
		[productsale_kbn],
		[productsale_name],
		[closedmarket_password],
		[date_bgn],
		[date_end],
		[valid_flg],
		[product_id],
		[variation_id],
		[sale_price],
		[display_order],
		[validity]
		) AS
SELECT	[w2_ProductSale].[shop_id],
		[w2_ProductSale].[productsale_id],
		[w2_ProductSale].[productsale_kbn],
		[w2_ProductSale].[productsale_name],
		[w2_ProductSale].[closedmarket_password],
		[w2_ProductSale].[date_bgn],
		[w2_ProductSale].[date_end],
		[w2_ProductSale].[valid_flg],
		[w2_ProductSalePrice].[product_id],
		[w2_ProductSalePrice].[variation_id],
		[w2_ProductSalePrice].[sale_price],
		[w2_ProductSalePrice].[display_order],
		(
			CASE
				WHEN (([w2_ProductSale].[date_bgn] <= GETDATE()) AND (GETDATE() <= [w2_ProductSale].[date_end]) AND ([w2_ProductSale].[valid_flg] = '1'))  THEN '1'
				ELSE '0'
			END
		)
  FROM	[dbo].[w2_ProductSale]
		LEFT JOIN [dbo].[w2_ProductSalePrice] ON
		(
			[dbo].[w2_ProductSale].[shop_id] = [dbo].[w2_ProductSalePrice].[shop_id]
			AND
			[dbo].[w2_ProductSale].[productsale_id] = [dbo].[w2_ProductSalePrice].[productsale_id]
		)
