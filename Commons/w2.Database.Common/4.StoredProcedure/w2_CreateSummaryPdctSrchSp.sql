if exists (select * from sys.procedures where name = 'w2_CreateSummaryPdctSrchSp')
	drop PROCEDURE [dbo].[w2_CreateSummaryPdctSrchSp]
GO
CREATE PROCEDURE [dbo].[w2_CreateSummaryPdctSrchSp] (
					@TARGET_DATE varchar(10)) AS

	---------------------------------------
	-- 変数定義
	---------------------------------------
	DECLARE @DEPT_ID nvarchar(30)
	DECLARE @SUMMARY_KBN nvarchar(30)
	DECLARE @TGT_YEAR nvarchar(4)
	DECLARE @TGT_MONTH nvarchar(2)
	DECLARE @TGT_DAY nvarchar(2)

	---------------------------------------
	-- 日付分割
	---------------------------------------
	SET @TGT_YEAR = SUBSTRING(@TARGET_DATE, 0, 5)
	SET @TGT_MONTH = SUBSTRING(@TARGET_DATE, 6, 2)
	SET @TGT_DAY = SUBSTRING(@TARGET_DATE, 9, 2)


	---------------------------------------
	-- 検索ワードサマリ作成（全体）
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_searchword'	-- 検索ワード

	-- 削除
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- 挿入
	INSERT
	  INTO	w2_DispSummaryAnalysis
			(
			dept_id,
			summary_kbn,
			tgt_year,
			tgt_month,
			tgt_day,
			value_name,
			counts,
			reserved1
			)
	SELECT	@DEPT_ID,
			@SUMMARY_KBN,
			@TGT_YEAR,
			@TGT_MONTH,
			@TGT_DAY,
			search_word,
			COUNT(*) AS TOTAL,
			AVG(hits) as AVERAGE
	  FROM	w2_ProductSearchWordHistory
	 WHERE	dept_id = @DEPT_ID
	   AND	date_created BETWEEN  @TARGET_DATE + ' 00:00:00' AND @TARGET_DATE + ' 23:59:59.997'
	GROUP BY search_word
	ORDER BY TOTAL DESC

	---------------------------------------
	-- 検索ワードサマリ作成（PC）
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_searchword_pc'	-- 検索ワード

	-- 削除
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- 挿入
	INSERT
	  INTO	w2_DispSummaryAnalysis
			(
			dept_id,
			summary_kbn,
			tgt_year,
			tgt_month,
			tgt_day,
			value_name,
			counts,
			reserved1
			)
	SELECT	@DEPT_ID,
			@SUMMARY_KBN,
			@TGT_YEAR,
			@TGT_MONTH,
			@TGT_DAY,
			search_word,
			COUNT(*) AS TOTAL,
			AVG(hits) as AVERAGE
	  FROM	w2_ProductSearchWordHistory
	 WHERE	dept_id = @DEPT_ID
	   AND	date_created BETWEEN  @TARGET_DATE + ' 00:00:00' AND @TARGET_DATE + ' 23:59:59.997'
	   AND	access_kbn = '0'	-- アクセス区分（PC'0' or モバイル'1' or スマートフォン'2'）
	GROUP BY search_word
	ORDER BY TOTAL DESC

	---------------------------------------
	-- 検索ワードサマリ作成（モバイル）
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_searchword_mob'	-- 検索ワード

	-- 削除
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- 挿入
	INSERT
	  INTO	w2_DispSummaryAnalysis
			(
			dept_id,
			summary_kbn,
			tgt_year,
			tgt_month,
			tgt_day,
			value_name,
			counts,
			reserved1
			)
	SELECT	@DEPT_ID,
			@SUMMARY_KBN,
			@TGT_YEAR,
			@TGT_MONTH,
			@TGT_DAY,
			search_word,
			COUNT(*) AS TOTAL,
			AVG(hits) as AVERAGE
	  FROM	w2_ProductSearchWordHistory
	 WHERE	dept_id = @DEPT_ID
	   AND	date_created BETWEEN  @TARGET_DATE + ' 00:00:00' AND @TARGET_DATE + ' 23:59:59.997'
	   AND	access_kbn = '1'	-- アクセス区分（PC'0' or モバイル'1' or スマートフォン'2'）
	GROUP BY search_word
	ORDER BY TOTAL DESC

	---------------------------------------
	-- 検索ワードサマリ作成（スマートフォン）
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_searchword_sp'	-- 検索ワード

	-- 削除
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- 挿入
	INSERT
	  INTO	w2_DispSummaryAnalysis
			(
			dept_id,
			summary_kbn,
			tgt_year,
			tgt_month,
			tgt_day,
			value_name,
			counts,
			reserved1
			)
	SELECT	@DEPT_ID,
			@SUMMARY_KBN,
			@TGT_YEAR,
			@TGT_MONTH,
			@TGT_DAY,
			search_word,
			COUNT(*) AS TOTAL,
			AVG(hits) as AVERAGE
	  FROM	w2_ProductSearchWordHistory
	 WHERE	dept_id = @DEPT_ID
	   AND	date_created BETWEEN  @TARGET_DATE + ' 00:00:00' AND @TARGET_DATE + ' 23:59:59.997'
	   AND	access_kbn = '2'	-- アクセス区分（PC'0' or モバイル'1' or スマートフォン'2'）
	GROUP BY search_word
	ORDER BY TOTAL DESC


	---------------------------------------
	-- 商品販売数サマリ作成（全体）
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_buycount'	-- 商品販売数

	-- 削除
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- 挿入
	INSERT
	  INTO	w2_DispSummaryAnalysis
			(
			dept_id,
			summary_kbn,
			tgt_year,
			tgt_month,
			tgt_day,
			value_name,
			counts,
			reserved6,
			reserved7
			)
	SELECT	@DEPT_ID,
			@SUMMARY_KBN,
			@TGT_YEAR,
			@TGT_MONTH,
			@TGT_DAY,
			MAX(w2_OrderItem.product_name),
			SUM(w2_OrderItem.item_quantity) AS TOTAL,
			w2_OrderItem.product_id,
			w2_OrderItem.variation_id
	  FROM	w2_Order
			LEFT JOIN w2_OrderItem ON w2_Order.order_id = w2_OrderItem.order_id
	 WHERE	order_date BETWEEN  @TARGET_DATE + ' 00:00:00' AND @TARGET_DATE + ' 23:59:59.997'
	   AND	order_status NOT IN ('TMP','ODR_CNSL','TMP_CNSL')	-- 仮注文、キャンセル、仮注文キャンセルは除外
	GROUP BY w2_OrderItem.product_id, w2_OrderItem.variation_id
	ORDER BY TOTAL DESC

	---------------------------------------
	-- 商品販売数サマリ作成（PC）
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_buycount_pc'	-- 商品販売数

	-- 削除
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- 挿入
	INSERT
	  INTO	w2_DispSummaryAnalysis
			(
			dept_id,
			summary_kbn,
			tgt_year,
			tgt_month,
			tgt_day,
			value_name,
			counts,
			reserved6,
			reserved7
			)
	SELECT	@DEPT_ID,
			@SUMMARY_KBN,
			@TGT_YEAR,
			@TGT_MONTH,
			@TGT_DAY,
			MAX(w2_OrderItem.product_name),
			SUM(w2_OrderItem.item_quantity) AS TOTAL,
			w2_OrderItem.product_id,
			w2_OrderItem.variation_id
	  FROM	w2_Order
			LEFT JOIN w2_OrderItem ON w2_Order.order_id = w2_OrderItem.order_id
	 WHERE	order_date BETWEEN  @TARGET_DATE + ' 00:00:00' AND @TARGET_DATE + ' 23:59:59.997'
	   AND	order_status NOT IN ('TMP','ODR_CNSL','TMP_CNSL')	-- 仮注文、キャンセル、仮注文キャンセルは除外
	   AND	order_kbn = 'PC'					-- 注文区分がPC
	GROUP BY w2_OrderItem.product_id, w2_OrderItem.variation_id
	ORDER BY TOTAL DESC

	---------------------------------------
	-- 商品販売数サマリ作成（モバイル）
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_buycount_mob'	-- 商品販売数

	-- 削除
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- 挿入
	INSERT
	  INTO	w2_DispSummaryAnalysis
			(
			dept_id,
			summary_kbn,
			tgt_year,
			tgt_month,
			tgt_day,
			value_name,
			counts,
			reserved6,
			reserved7
			)
	SELECT	@DEPT_ID,
			@SUMMARY_KBN,
			@TGT_YEAR,
			@TGT_MONTH,
			@TGT_DAY,
			MAX(w2_OrderItem.product_name),
			SUM(w2_OrderItem.item_quantity) AS TOTAL,
			w2_OrderItem.product_id,
			w2_OrderItem.variation_id
	  FROM	w2_Order
			LEFT JOIN w2_OrderItem ON w2_Order.order_id = w2_OrderItem.order_id
	 WHERE	order_date BETWEEN  @TARGET_DATE + ' 00:00:00' AND @TARGET_DATE + ' 23:59:59.997'
	   AND	order_status NOT IN ('TMP','ODR_CNSL','TMP_CNSL')	-- 仮注文、キャンセル、仮注文キャンセルは除外
	   AND	order_kbn = 'MB'					-- 注文区分がモバイル
	GROUP BY w2_OrderItem.product_id, w2_OrderItem.variation_id
	ORDER BY TOTAL DESC

	---------------------------------------
	-- 商品販売数サマリ作成（スマートフォン）
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_buycount_sp'	-- 商品販売数

	-- 削除
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- 挿入
	INSERT
	  INTO	w2_DispSummaryAnalysis
			(
			dept_id,
			summary_kbn,
			tgt_year,
			tgt_month,
			tgt_day,
			value_name,
			counts,
			reserved6,
			reserved7
			)
	SELECT	@DEPT_ID,
			@SUMMARY_KBN,
			@TGT_YEAR,
			@TGT_MONTH,
			@TGT_DAY,
			MAX(w2_OrderItem.product_name),
			SUM(w2_OrderItem.item_quantity) AS TOTAL,
			w2_OrderItem.product_id,
			w2_OrderItem.variation_id
	  FROM	w2_Order
			LEFT JOIN w2_OrderItem ON w2_Order.order_id = w2_OrderItem.order_id
	 WHERE	order_date BETWEEN  @TARGET_DATE + ' 00:00:00' AND @TARGET_DATE + ' 23:59:59.997'
	   AND	order_status NOT IN ('TMP','ODR_CNSL','TMP_CNSL')	-- 仮注文、キャンセル、仮注文キャンセルは除外
	   AND	order_kbn = 'SP'					-- 注文区分がスマートフォン
	GROUP BY w2_OrderItem.product_id, w2_OrderItem.variation_id
	ORDER BY TOTAL DESC


	---------------------------------------
	-- 商品販売金額サマリ作成（全体）
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_buyprice'	-- 商品販売数

	-- 削除
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- 挿入
	INSERT
	  INTO	w2_DispSummaryAnalysis
			(
			dept_id,
			summary_kbn,
			tgt_year,
			tgt_month,
			tgt_day,
			value_name,
			price,
			price_tax,
			reserved6,
			reserved7
			)
	SELECT	@DEPT_ID,
			@SUMMARY_KBN,
			@TGT_YEAR,
			@TGT_MONTH,
			@TGT_DAY,
			MAX(w2_OrderItem.product_name),
			SUM(w2_OrderItem.item_quantity * product_price)  AS TOTAL,
			SUM(w2_OrderItem.item_price_tax)  AS TAXTOTAL,
			w2_OrderItem.product_id,
			w2_OrderItem.variation_id
	  FROM	w2_Order
			LEFT JOIN w2_OrderItem ON w2_Order.order_id = w2_OrderItem.order_id
	 WHERE	order_date BETWEEN  @TARGET_DATE + ' 00:00:00' AND @TARGET_DATE + ' 23:59:59.997'
	   AND	order_status NOT IN ('TMP','ODR_CNSL','TMP_CNSL')	-- 仮注文、キャンセル、仮注文キャンセルは除外
	GROUP BY w2_OrderItem.product_id, w2_OrderItem.variation_id
	ORDER BY TOTAL DESC

	---------------------------------------
	-- 商品販売金額サマリ作成（PC）
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_buyprice_pc'	-- 商品販売数

	-- 削除
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- 挿入
	INSERT
	  INTO	w2_DispSummaryAnalysis
			(
			dept_id,
			summary_kbn,
			tgt_year,
			tgt_month,
			tgt_day,
			value_name,
			price,
			price_tax,
			reserved6,
			reserved7
			)
	SELECT	@DEPT_ID,
			@SUMMARY_KBN,
			@TGT_YEAR,
			@TGT_MONTH,
			@TGT_DAY,
			MAX(w2_OrderItem.product_name),
			SUM(w2_OrderItem.item_quantity * product_price)  AS TOTAL,
			SUM(w2_OrderItem.item_price_tax)  AS TAXTOTAL,
			w2_OrderItem.product_id,
			w2_OrderItem.variation_id
	  FROM	w2_Order
			LEFT JOIN w2_OrderItem ON w2_Order.order_id = w2_OrderItem.order_id
	 WHERE	order_date BETWEEN  @TARGET_DATE + ' 00:00:00' AND @TARGET_DATE + ' 23:59:59.997'
	   AND	order_status NOT IN ('TMP','ODR_CNSL','TMP_CNSL')	-- 仮注文、キャンセル、仮注文キャンセルは除外
	   AND	order_kbn = 'PC'					-- 注文区分がPC
	GROUP BY w2_OrderItem.product_id, w2_OrderItem.variation_id
	ORDER BY TOTAL DESC

	---------------------------------------
	-- 商品販売金額サマリ作成（モバイル）
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_buyprice_mob'	-- 商品販売数

	-- 削除
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- 挿入
	INSERT
	  INTO	w2_DispSummaryAnalysis
			(
			dept_id,
			summary_kbn,
			tgt_year,
			tgt_month,
			tgt_day,
			value_name,
			price,
			price_tax,
			reserved6,
			reserved7
			)
	SELECT	@DEPT_ID,
			@SUMMARY_KBN,
			@TGT_YEAR,
			@TGT_MONTH,
			@TGT_DAY,
			MAX(w2_OrderItem.product_name),
			SUM(w2_OrderItem.item_quantity * product_price)  AS TOTAL,
			SUM(w2_OrderItem.item_price_tax)  AS TAXTOTAL,
			w2_OrderItem.product_id,
			w2_OrderItem.variation_id
	  FROM	w2_Order
			LEFT JOIN w2_OrderItem ON w2_Order.order_id = w2_OrderItem.order_id
	 WHERE	order_date BETWEEN  @TARGET_DATE + ' 00:00:00' AND @TARGET_DATE + ' 23:59:59.997'
	   AND	order_status NOT IN ('TMP','ODR_CNSL','TMP_CNSL')	-- 仮注文、キャンセル、仮注文キャンセルは除外
	   AND	order_kbn = 'MB'					-- 注文区分がモバイル
	GROUP BY w2_OrderItem.product_id, w2_OrderItem.variation_id
	ORDER BY TOTAL DESC
	
	---------------------------------------
	-- 商品販売金額サマリ作成（スマートフォン）
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_buyprice_sp'	-- 商品販売数

	-- 削除
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- 挿入
	INSERT
	  INTO	w2_DispSummaryAnalysis
			(
			dept_id,
			summary_kbn,
			tgt_year,
			tgt_month,
			tgt_day,
			value_name,
			price,
			price_tax,
			reserved6,
			reserved7
			)
	SELECT	@DEPT_ID,
			@SUMMARY_KBN,
			@TGT_YEAR,
			@TGT_MONTH,
			@TGT_DAY,
			MAX(w2_OrderItem.product_name),
			SUM(w2_OrderItem.item_quantity * product_price)  AS TOTAL,
			SUM(w2_OrderItem.item_price_tax)  AS TAXTOTAL,
			w2_OrderItem.product_id,
			w2_OrderItem.variation_id
	  FROM	w2_Order
			LEFT JOIN w2_OrderItem ON w2_Order.order_id = w2_OrderItem.order_id
	 WHERE	order_date BETWEEN  @TARGET_DATE + ' 00:00:00' AND @TARGET_DATE + ' 23:59:59.997'
	   AND	order_status NOT IN ('TMP','ODR_CNSL','TMP_CNSL')	-- 仮注文、キャンセル、仮注文キャンセルは除外
	   AND	order_kbn = 'SP'					-- 注文区分がスマートフォン
	GROUP BY w2_OrderItem.product_id, w2_OrderItem.variation_id
	ORDER BY TOTAL DESC
	
GO