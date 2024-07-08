/*
=========================================================================================================
  Module      : 注文メモ設定リポジトリ (OrderMemoSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.OrderMemoSetting
{
	/// <summary>
	/// 注文メモ設定リポジトリ
	/// </summary>
	public class OrderMemoSettingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "OrderMemoSetting";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderMemoSettingRepository(SqlAccessor accessor = null)
			: base(accessor)
		{
		}
		#endregion

		#region +GetOrderMemoSettingInDataView 取得(表示区分指定)
		/// <summary>
		/// 取得(表示区分指定) HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="displayKbn">表示区分</param>
		/// <returns>注文メモ設定</returns>
		internal DataView GetOrderMemoSettingInDataView(string displayKbn)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERMEMOSETTING_DISPLAY_KBN, displayKbn},
				{Constants.FIELD_ORDERMEMOSETTING_VALID_FLG, Constants.FLG_ORDER_MEMO_SETTING_VALID_FLG_VALID}
			};
			var dv = Get(XML_KEY_NAME, "GetOrderMemoSetting", ht);
			return dv;
		}
		#endregion

		#region +GetOrderMemoSettingContainsGlobalSetting 注文メモ情報取得(翻訳情報含む)
		/// <summary>
		/// 注文メモ情報取得(翻訳情報含む)
		/// </summary>
		/// <param name="displayKbn">表示区分</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>注文メモ設定</returns>
		internal DataView GetOrderMemoSettingContainsGlobalSetting(string displayKbn, string languageCode, string languageLocaleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERMEMOSETTING_DISPLAY_KBN, displayKbn},
				{Constants.FIELD_ORDERMEMOSETTING_VALID_FLG, Constants.FLG_ORDER_MEMO_SETTING_VALID_FLG_VALID},
				{Constants.FIELD_NAMETRANSLATIONSETTING_LANGUAGE_CODE, languageCode},
				{Constants.FIELD_NAMETRANSLATIONSETTING_LANGUAGE_LOCALE_ID, languageLocaleId},
			};
			var dv = Get(XML_KEY_NAME, "GetOrderMemoSettingContainsGlobalSetting", ht);
			return dv;
		}
		#endregion
	}
}
