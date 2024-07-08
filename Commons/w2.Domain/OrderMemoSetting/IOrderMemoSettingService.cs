/*
=========================================================================================================
  Module      : Interface Order Memo Setting Service (IOrderMemoSettingService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Data;

namespace w2.Domain.OrderMemoSetting
{
	/// <summary>
	/// Interface Order Memo Setting Service
	/// </summary>
	public interface IOrderMemoSettingService : IService
	{
		/// <summary>
		/// 取得(表示区分指定) HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="displayKbn">表示区分指定</param>
		/// <returns>注文メモ設定</returns>
		DataView GetOrderMemoSettingInDataView(string displayKbn);

		/// <summary>
		/// 注文メモ情報取得(翻訳情報含む)
		/// </summary>
		/// <param name="displayKbn">表示区分指定</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>注文メモ設定</returns>
		DataView GetOrderMemoSettingContainsGlobalSetting(string displayKbn, string languageCode, string languageLocaleId);
	}
}
