if exists (select * from dbo.sysobjects where type = 'TR' and name = 'w2_AddMallCooperationLogTr')
	begin
		drop trigger [dbo].[w2_AddMallCooperationLogTr]
	end
GO
/*
=========================================================================================================
  Module      : モール連携処理ログ追加トリガ(w2_AddMallCooperationLogTr.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE Trigger w2_AddMallCooperationLogTr ON w2_ProductStock
FOR UPDATE AS
	/*
		 ■処理概要
		 ・商品在庫マスタが操作(更新)が行われた場合、
		   モール連携更新ログ(w2_MallCooperationUpdateLog)にログ情報を追加する
	*/

	-------------------------------------------
	-- 変数宣言
	-------------------------------------------
	DECLARE @master_kbn nvarchar(30)
	DECLARE @action_status nvarchar(2)
	
	------------------------------------------------
	-- 初期設定
	------------------------------------------------
	SET @master_kbn = 'ProductStock'	-- 商品在庫情報
	SET @action_status = '00'			-- 未処理
	
	/* 対象商品の取得 */
	;WITH tmpTarget
		AS
		(
			SELECT	ISNULL(Inserted.shop_id, Deleted.shop_id) AS shop_id,
					ISNULL(Inserted.product_id, Deleted.product_id) AS product_id,
					ISNULL(Inserted.variation_id, Deleted.variation_id) AS variation_id,
					CASE
						WHEN Deleted.shop_id IS NULL THEN 'I'
						WHEN Inserted.shop_id IS NULL THEN 'D'
						ELSE 'U' END AS action_kbn
			FROM	Inserted
			FULL JOIN
					Deleted
					ON ( Inserted.shop_id = Deleted.shop_id )
						AND ( Inserted.product_id = Deleted.product_id )
						AND ( Inserted.variation_id = Deleted.variation_id )
			WHERE
					EXISTS
						( SELECT * FROM  w2_ProductVariationView
							WHERE w2_ProductVariationView.shop_id = ISNULL(Inserted.shop_id, Deleted.shop_id)
								AND w2_ProductVariationView.product_id = ISNULL(Inserted.product_id, Deleted.product_id)
								AND w2_ProductVariationView.variation_id = ISNULL(Inserted.variation_id, Deleted.variation_id)
								AND w2_ProductVariationView.mall_variation_type NOT IN ('s', 'c')
						)
		)

	/* モール連携更新ログINSERT */
	INSERT INTO
		w2_MallCooperationUpdateLog
	(
		shop_id,
		mall_id,
		product_id,
		variation_id,
		master_kbn,
		action_kbn,
		action_status
	)
	SELECT	tmpTarget.shop_id,
			w2_MallCooperationSetting.mall_id,
			tmpTarget.product_id,
			tmpTarget.variation_id,
			@master_kbn,
			tmpTarget.action_kbn,
			@action_status
	FROM	w2_MallCooperationSetting, tmpTarget
	WHERE	w2_MallCooperationSetting.valid_flg = '1'	-- 有効
			AND
			(
				w2_MallCooperationSetting.mall_exhibits_config = ''
				OR
					EXISTS
					(
						SELECT	*
						FROM	w2_MallExhibitsConfig
						WHERE	w2_MallExhibitsConfig.shop_id = tmpTarget.shop_id
								AND w2_MallExhibitsConfig.product_id = tmpTarget.product_id
								AND (
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH1'
										AND w2_MallExhibitsConfig.exhibits_flg1 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH2'
										AND w2_MallExhibitsConfig.exhibits_flg2 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH3'
										AND w2_MallExhibitsConfig.exhibits_flg3 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH4'
										AND w2_MallExhibitsConfig.exhibits_flg4 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH5'
										AND w2_MallExhibitsConfig.exhibits_flg5 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH6'
										AND w2_MallExhibitsConfig.exhibits_flg6 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH7'
										AND w2_MallExhibitsConfig.exhibits_flg7 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH8'
										AND w2_MallExhibitsConfig.exhibits_flg8 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH9'
										AND w2_MallExhibitsConfig.exhibits_flg9 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH10'
										AND w2_MallExhibitsConfig.exhibits_flg10 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH11'
										AND w2_MallExhibitsConfig.exhibits_flg11 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH12'
										AND w2_MallExhibitsConfig.exhibits_flg12 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH13'
										AND w2_MallExhibitsConfig.exhibits_flg13 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH14'
										AND w2_MallExhibitsConfig.exhibits_flg14 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH15'
										AND w2_MallExhibitsConfig.exhibits_flg15 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH16'
										AND w2_MallExhibitsConfig.exhibits_flg16 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH17'
										AND w2_MallExhibitsConfig.exhibits_flg17 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH18'
										AND w2_MallExhibitsConfig.exhibits_flg18 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH19'
										AND w2_MallExhibitsConfig.exhibits_flg19 = '1'
									)
									OR
									(
										w2_MallCooperationSetting.mall_exhibits_config = 'EXH20'
										AND w2_MallExhibitsConfig.exhibits_flg20 = '1'
									)
								)
					)
			)

GO