/*
=========================================================================================================
  Module      : 配送会社マスタモデル (ShippingCompanyModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using w2.Common.Util;

namespace w2.Domain.DeliveryCompany
{
	/// <summary>
	/// 配送会社マスタモデル
	/// </summary>
	public partial class DeliveryCompanyModel
	{
		/// <summary>配送希望時間帯保持数</summary>
		public const int CONST_SHIPPING_TIME_COUNT = 10;

		#region メソッド
		/// <summary>
		/// トリムした配送希望時間リストを作成
		/// </summary>
		/// <returns>トリムした配送希望時間リスト</returns>
		public Dictionary<string, string> GetTrimmedShippingTimeList()
		{
			var shippingTimes = GetShippingTimeList().ToDictionary(
				t => t.Key,
				t => t.Value.Trim().TrimStart('0').Replace("-", "～"));
			return shippingTimes;
		}

		/// <summary>
		/// 配送時間帯リスト取得
		/// </summary>
		/// <returns>配送時間帯リスト</returns>
		public Dictionary<string, string> GetShippingTimeList()
		{
			var result = new Dictionary<string, string>();
			for (int number = 1; number <= CONST_SHIPPING_TIME_COUNT; number++)
			{
				var shippingTimeId = StringUtility.ToEmpty(this.DataSource["shipping_time_id" + number]);
				var shippingTimeMessage = StringUtility.ToEmpty(this.DataSource["shipping_time_message" + number]);

				if ((result.ContainsKey(shippingTimeId) == false)
					&& (string.IsNullOrEmpty(shippingTimeId) == false)
					&& (string.IsNullOrEmpty(shippingTimeMessage) == false))
				{
					result.Add(shippingTimeId, shippingTimeMessage);
				}
			}

			return result;
		}

		/// <summary>
		/// 配送時間帯マッチング文言取得
		/// </summary>
		/// <returns>配送時間帯マッチング文言リスト</returns>
		public Dictionary<string, string[]> GetShippingTimeMatchingList()
		{
			var result = new Dictionary<string, string[]>();
			for (int number = 1; number <= CONST_SHIPPING_TIME_COUNT; number++)
			{
				var shippingTimeId = (string)this.DataSource["shipping_time_id" + number];
				var shippingTimeMatching = (string)this.DataSource["shipping_time_matching" + number];

				if ((result.ContainsKey(shippingTimeId) == false)
					&& (string.IsNullOrEmpty(shippingTimeId) == false)
					&& (string.IsNullOrEmpty(shippingTimeMatching) == false))
				{
					result.Add(shippingTimeId, shippingTimeMatching.Split(','));
				}
			}
			return result;
		}

		/// <summary>
		/// 配送希望時間帯文言取得
		/// </summary>
		/// <param name="shippingTimeId">配送希望時間帯ID</param>
		/// <param name="defaultValue">該当する配送希望時間帯が取得できなかった際のデフォルト文言</param>
		/// <returns>配送希望時間帯文言</returns>
		public string GetShippingTimeMessage(string shippingTimeId, string defaultValue = "")
		{
			return GetShippingTimeList().FirstOrDefault(item => item.Key == shippingTimeId).Value ?? defaultValue;
		}
		#endregion

		#region プロパティ
		/// <summary>配送希望時間帯設定可能フラグが有効か</summary>
		public bool IsValidShippingTimeSetFlg
		{
			get { return (this.ShippingTimeSetFlg == Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID); }
		}
		/// <summary>Is valid delivery lead time set flg</summary>
		public bool IsValidDeliveryLeadTimeSetFlg
		{
			get { return (this.DeliveryLeadTimeSetFlg == Constants.FLG_DELIVERYCOMPANY_LEAD_TIME_SET_FLG_VALID); }
		}
		#endregion
	}
}
