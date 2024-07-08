/*
=========================================================================================================
  Module      : 注文メモ設定サービス (OrderMemoSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Transactions;

namespace w2.Domain.OrderMemoSetting
{
	/// <summary>
	/// 注文メモ設定サービス
	/// </summary>
	public class OrderMemoSettingService : ServiceBase, IOrderMemoSettingService
	{
		#region +GetOrderMemoSettingInDataView 取得(表示区分指定)
		/// <summary>
		/// 取得(表示区分指定) HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="displayKbn">表示区分指定</param>
		/// <returns>注文メモ設定</returns>
		public DataView GetOrderMemoSettingInDataView(string displayKbn)
		{
			using (var repository = new OrderMemoSettingRepository())
			{
				var dv = repository.GetOrderMemoSettingInDataView(displayKbn);
				return dv;
			}
		}
		#endregion

		#region +GetOrderMemoSettingContainsGlobalSetting 注文メモ情報取得(翻訳情報含む)
		/// <summary>
		/// 注文メモ情報取得(翻訳情報含む)
		/// </summary>
		/// <param name="displayKbn">表示区分指定</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>注文メモ設定</returns>
		public DataView GetOrderMemoSettingContainsGlobalSetting(string displayKbn, string languageCode, string languageLocaleId)
		{
			using (var repository = new OrderMemoSettingRepository())
			{
				var dv = repository.GetOrderMemoSettingContainsGlobalSetting(displayKbn, languageCode, languageLocaleId);
				return dv;
			}
		}
		#endregion
	}
}
