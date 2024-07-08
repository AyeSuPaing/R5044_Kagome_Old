/*
=========================================================================================================
  Module      : Amazonモールのマッピング設定管理(AmazonMappingManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Xml;
using w2.Common.Util;

namespace w2.App.Common.Mall.Mapping
{
	/// <summary>
	/// Amazonモールのマッピング設定管理
	/// </summary>
	public class AmazonMappingManager : MallMappingManager
	{
		/// <summary>定数: デフォルト設定ファイル名</summary>
		private const string DEFAULT_SETTING_FILE_NAME = "AmazonMwsMapping.xml";
		/// <summary>定数: プロジェクト設定ファイル名</summary>
		private const string PROJECT_SETTING_FILE_NAME = "ProjectAmazonMwsMapping.xml";
		/// <summary>定数: ルートノード名</summary>
		private const string ROOT_NODE = "AmazonMwsMapping";

		/// <summary>定数: 支払区分ノード</summary>
		private const string MAPPING_PAYMENT_METHOD_NODE = "PaymentMethod";
		/// <summary>定数: 配送希望時間帯ノード</summary>
		private const string MAPPING_SHIPPINGTIME_SETTING_NODE = "ShippingTimeSetting";
		/// <summary>定数: 変換前ノード</summary>
		private const string MAPPING_BEFORE_NODE = "Bef";
		/// <summary>定数: 変換後ノード</summary>
		private const string MAPPING_AFTER_NODE = "Aft";

		/// <summary>インスタンス(シングルトン管理)</summary>
		private static AmazonMappingManager m_singletonInstance;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private AmazonMappingManager()
			: base(
				DEFAULT_SETTING_FILE_NAME,
				PROJECT_SETTING_FILE_NAME,
				ROOT_NODE)
		{
		}

		/// <summary>
		/// インスタンス返却
		/// </summary>
		/// <returns></returns>
		public static AmazonMappingManager GetInstance()
		{
			if (m_singletonInstance == null) m_singletonInstance = new AmazonMappingManager();
			return m_singletonInstance;
		}

		/// <summary>
		/// 支払区分変換
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="paymentMethod">支払い方法</param>
		/// <returns>支払区分</returns>
		public string ConvertOrderPaymentKbn(string mallId, string paymentMethod)
		{
			var orderPaymentKbn = string.Empty;
			var mappingSettings = base.GetNodeList(mallId, MAPPING_PAYMENT_METHOD_NODE);
			if (mappingSettings == null) return orderPaymentKbn;

			foreach (var paymentNode in mappingSettings.Cast<XmlNode>().Select(x => x))
			{
				var beforeNode = paymentNode.SelectSingleNode(MAPPING_BEFORE_NODE);
				if ((beforeNode != null) && (beforeNode.InnerText.ToUpper() == paymentMethod.ToUpper()))
				{
					orderPaymentKbn = StringUtility.ToEmpty(paymentNode.SelectSingleNode(MAPPING_AFTER_NODE).InnerText);
				}
			}
			return orderPaymentKbn;
		}

		/// <summary>
		/// 配送希望時間帯設定
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="earliestDeliveryDate">配送希望時間帯 開始</param>
		/// <param name="latestDeliveryDate">配送希望時間帯 終わり</param>
		/// <returns>配送希望時間帯ID</returns>
		public string ConvertShippingTime(string mallId, DateTime earliestDeliveryDate, DateTime latestDeliveryDate)
		{
			var shippingTime = string.Empty;
			var mappingSettings = base.GetNodeList(mallId, MAPPING_SHIPPINGTIME_SETTING_NODE);
			if (mappingSettings == null) return shippingTime;

			foreach (var shippingTimeNode in mappingSettings.Cast<XmlNode>().Select(x => x))
			{
				var beforeNode = shippingTimeNode.SelectSingleNode(MAPPING_BEFORE_NODE);
				if ((beforeNode != null))
				{
					var timeValue = beforeNode.InnerText.Split('-').ToList();
					DateTime startTime;
					DateTime endTime;
					if ((DateTime.TryParseExact(timeValue[0], "HH:mm", null, System.Globalization.DateTimeStyles.None, out startTime))
						&& (DateTime.TryParseExact(timeValue[1], "HH:mm", null, System.Globalization.DateTimeStyles.None, out endTime))
						&& (Convert.ToInt32(startTime.ToString("HHmm")) <= Convert.ToInt32(earliestDeliveryDate.ToString("HHmm")))
						&& (Convert.ToInt32(latestDeliveryDate.ToString("HHmm")) <= Convert.ToInt32(endTime.ToString("HHmm"))))
					{
						shippingTime = StringUtility.ToEmpty(shippingTimeNode.SelectSingleNode(MAPPING_AFTER_NODE).InnerText);
					}
				}
			}
			return shippingTime;
		}
	}
}
