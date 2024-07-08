if exists (select * from sys.procedures where name = 'w2_CreateSummaryPdctSrchSp')
	drop PROCEDURE [dbo].[w2_CreateSummaryPdctSrchSp]
GO
CREATE PROCEDURE [dbo].[w2_CreateSummaryPdctSrchSp] (
					@TARGET_DATE varchar(10)) AS

	---------------------------------------
	-- �ϐ���`
	---------------------------------------
	DECLARE @DEPT_ID nvarchar(30)
	DECLARE @SUMMARY_KBN nvarchar(30)
	DECLARE @TGT_YEAR nvarchar(4)
	DECLARE @TGT_MONTH nvarchar(2)
	DECLARE @TGT_DAY nvarchar(2)

	---------------------------------------
	-- ���t����
	---------------------------------------
	SET @TGT_YEAR = SUBSTRING(@TARGET_DATE, 0, 5)
	SET @TGT_MONTH = SUBSTRING(@TARGET_DATE, 6, 2)
	SET @TGT_DAY = SUBSTRING(@TARGET_DATE, 9, 2)


	---------------------------------------
	-- �������[�h�T�}���쐬�i�S�́j
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_searchword'	-- �������[�h

	-- �폜
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- �}��
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
	-- �������[�h�T�}���쐬�iPC�j
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_searchword_pc'	-- �������[�h

	-- �폜
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- �}��
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
	   AND	access_kbn = '0'	-- �A�N�Z�X�敪�iPC'0' or ���o�C��'1' or �X�}�[�g�t�H��'2'�j
	GROUP BY search_word
	ORDER BY TOTAL DESC

	---------------------------------------
	-- �������[�h�T�}���쐬�i���o�C���j
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_searchword_mob'	-- �������[�h

	-- �폜
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- �}��
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
	   AND	access_kbn = '1'	-- �A�N�Z�X�敪�iPC'0' or ���o�C��'1' or �X�}�[�g�t�H��'2'�j
	GROUP BY search_word
	ORDER BY TOTAL DESC

	---------------------------------------
	-- �������[�h�T�}���쐬�i�X�}�[�g�t�H���j
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_searchword_sp'	-- �������[�h

	-- �폜
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- �}��
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
	   AND	access_kbn = '2'	-- �A�N�Z�X�敪�iPC'0' or ���o�C��'1' or �X�}�[�g�t�H��'2'�j
	GROUP BY search_word
	ORDER BY TOTAL DESC


	---------------------------------------
	-- ���i�̔����T�}���쐬�i�S�́j
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_buycount'	-- ���i�̔���

	-- �폜
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- �}��
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
	   AND	order_status NOT IN ('TMP','ODR_CNSL','TMP_CNSL')	-- �������A�L�����Z���A�������L�����Z���͏��O
	GROUP BY w2_OrderItem.product_id, w2_OrderItem.variation_id
	ORDER BY TOTAL DESC

	---------------------------------------
	-- ���i�̔����T�}���쐬�iPC�j
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_buycount_pc'	-- ���i�̔���

	-- �폜
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- �}��
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
	   AND	order_status NOT IN ('TMP','ODR_CNSL','TMP_CNSL')	-- �������A�L�����Z���A�������L�����Z���͏��O
	   AND	order_kbn = 'PC'					-- �����敪��PC
	GROUP BY w2_OrderItem.product_id, w2_OrderItem.variation_id
	ORDER BY TOTAL DESC

	---------------------------------------
	-- ���i�̔����T�}���쐬�i���o�C���j
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_buycount_mob'	-- ���i�̔���

	-- �폜
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- �}��
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
	   AND	order_status NOT IN ('TMP','ODR_CNSL','TMP_CNSL')	-- �������A�L�����Z���A�������L�����Z���͏��O
	   AND	order_kbn = 'MB'					-- �����敪�����o�C��
	GROUP BY w2_OrderItem.product_id, w2_OrderItem.variation_id
	ORDER BY TOTAL DESC

	---------------------------------------
	-- ���i�̔����T�}���쐬�i�X�}�[�g�t�H���j
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_buycount_sp'	-- ���i�̔���

	-- �폜
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- �}��
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
	   AND	order_status NOT IN ('TMP','ODR_CNSL','TMP_CNSL')	-- �������A�L�����Z���A�������L�����Z���͏��O
	   AND	order_kbn = 'SP'					-- �����敪���X�}�[�g�t�H��
	GROUP BY w2_OrderItem.product_id, w2_OrderItem.variation_id
	ORDER BY TOTAL DESC


	---------------------------------------
	-- ���i�̔����z�T�}���쐬�i�S�́j
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_buyprice'	-- ���i�̔���

	-- �폜
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- �}��
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
	   AND	order_status NOT IN ('TMP','ODR_CNSL','TMP_CNSL')	-- �������A�L�����Z���A�������L�����Z���͏��O
	GROUP BY w2_OrderItem.product_id, w2_OrderItem.variation_id
	ORDER BY TOTAL DESC

	---------------------------------------
	-- ���i�̔����z�T�}���쐬�iPC�j
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_buyprice_pc'	-- ���i�̔���

	-- �폜
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- �}��
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
	   AND	order_status NOT IN ('TMP','ODR_CNSL','TMP_CNSL')	-- �������A�L�����Z���A�������L�����Z���͏��O
	   AND	order_kbn = 'PC'					-- �����敪��PC
	GROUP BY w2_OrderItem.product_id, w2_OrderItem.variation_id
	ORDER BY TOTAL DESC

	---------------------------------------
	-- ���i�̔����z�T�}���쐬�i���o�C���j
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_buyprice_mob'	-- ���i�̔���

	-- �폜
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- �}��
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
	   AND	order_status NOT IN ('TMP','ODR_CNSL','TMP_CNSL')	-- �������A�L�����Z���A�������L�����Z���͏��O
	   AND	order_kbn = 'MB'					-- �����敪�����o�C��
	GROUP BY w2_OrderItem.product_id, w2_OrderItem.variation_id
	ORDER BY TOTAL DESC
	
	---------------------------------------
	-- ���i�̔����z�T�}���쐬�i�X�}�[�g�t�H���j
	---------------------------------------
	SET @DEPT_ID = '0'
	SET @SUMMARY_KBN = 'pct_buyprice_sp'	-- ���i�̔���

	-- �폜
	DELETE
	  FROM	w2_DispSummaryAnalysis
	 WHERE	dept_id = @DEPT_ID
	   AND	summary_kbn = @SUMMARY_KBN
	   AND	tgt_year = @TGT_YEAR
	   AND	tgt_month = @TGT_MONTH
	   AND	tgt_day = @TGT_DAY

	-- �}��
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
	   AND	order_status NOT IN ('TMP','ODR_CNSL','TMP_CNSL')	-- �������A�L�����Z���A�������L�����Z���͏��O
	   AND	order_kbn = 'SP'					-- �����敪���X�}�[�g�t�H��
	GROUP BY w2_OrderItem.product_id, w2_OrderItem.variation_id
	ORDER BY TOTAL DESC
	
GO