/*
=========================================================================================================
  Module      : 商品一覧表示設定リポジトリ (ProductListDispSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ProductListDispSetting
{
	/// <summary>
	/// 商品一覧表示設定リポジトリ
	/// </summary>
	public class ProductListDispSettingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ProductListDispSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductListDispSettingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductListDispSettingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetAll すべて取得
		/// <summary>
		/// すべて取得
		/// </summary>
		/// <returns>モデル列</returns>
		public ProductListDispSettingModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll");
			return dv.Cast<DataRowView>().Select(drv => new ProductListDispSettingModel(drv)).ToArray();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Insert(ProductListDispSettingModel model)
		{
			var result = Exec(XML_KEY_NAME, "Insert", model.DataSource);
			return result;
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(ProductListDispSettingModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="settingId">設定ID</param>
		/// <param name="settingKbn">設定区分</param>
		/// <returns>影響を受けた数</returns>
		public int Delete(string settingId, string settingKbn)
		{
			var result = Exec(
				XML_KEY_NAME,
				"Delete",
				new Hashtable()
				{
					{ Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_ID, settingId },
					{ Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_KBN, settingKbn}
				});
			return result;
		}
		#endregion

		#region +DeleteBySettingKbn 設定区分で削除
		/// <summary>
		/// 設定区分で削除
		/// </summary>
		/// <param name="settingKbn">設定区分</param>
		/// <returns>影響を受けた数</returns>
		public int DeleteBySettingKbn(string settingKbn)
		{
			var result = Exec(
				XML_KEY_NAME,
				"DeleteBySettingKbn",
				new Hashtable()
				{
					{ Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_KBN, settingKbn}
				});
			return result;
		}
		#endregion
	}
}
