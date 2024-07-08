/*
=========================================================================================================
  Module      : 商品画像削除用商品マスタ取込設定クラス(ImportSettingDeleteProductImage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using w2.Common.Util;
using w2.Common.Sql;

namespace w2.Commerce.Batch.MasterFileImport.ImportSettings
{
	public class ImportSettingDeleteProductImage : ImportSettingBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		public ImportSettingDeleteProductImage(string shopId)
			: base(
			shopId,
			Constants.ACTION_KBN_DELETE_PRODUCT_IMAGE,
			null, null, null, null, null, null)
		{
			// 何もしない
		} 

		/// <summary>
		/// データ変換（各種変換、フィールド結合、固定値設定など）
		/// </summary>
		protected override void ConvertData()
		{
			//------------------------------------------------------
			// データ変換
			//------------------------------------------------------
			foreach (string strFieldName in this.HeadersCsv)
			{
				// Trim処理
				this.Data[strFieldName] = this.Data[strFieldName].ToString().Trim();

				// 半角変換
				switch (strFieldName)
				{
					case Constants.FIELD_PRODUCT_IMAGE_HEAD:
					case Constants.FIELD_PRODUCT_IMAGE_MOBILE:
					case Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD:
					case Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_MOBILE:
						this.Data[strFieldName] = StringUtility.ToHankaku(this.Data[strFieldName].ToString());
						break;
				}
			}
		}

		/// <summary>
		/// 入力チェック
		/// </summary>
		protected override void CheckData()
		{
			// なにもしない //
		}

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public override string CheckDataConsistency()
		{
			return "";	// 整合性チェックしない
		}
	}
}
