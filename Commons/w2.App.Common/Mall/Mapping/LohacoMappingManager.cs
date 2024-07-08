/*
=========================================================================================================
  Module      : Lohacoモールのマッピング設定管理(LohacoMappingManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Xml;
using w2.Common.Util;

namespace w2.App.Common.Mall.Mapping
{
	/// <summary>
	/// Lohacoモールのマッピング設定管理
	/// </summary>
	public class LohacoMappingManager : MallMappingManager
	{
		/// <summary>定数: デフォルト設定ファイル名</summary>
		private const string DEFAULT_SETTING_FILE_NAME = "LohacoCreatorMapping.xml";
		/// <summary>定数: プロジェクト設定ファイル名</summary>
		private const string PROJECT_SETTING_FILE_NAME = "ProjectLohacoCreatorMapping.xml";
		/// <summary>定数: ルートノード名</summary>
		private const string ROOT_NODE = "LohacoCreatorMapping";

		/// <summary>定数: 支払区分ノード</summary>
		private const string MAPPING_PAYMENT_METHOD_NODE = "PaymentMethod";
		/// <summary>定数: 配送希望時間帯ノード</summary>
		private const string MAPPING_SHIPPINGTIME_SETTING_NODE = "ShippingTimeSetting";
		/// <summary>定数: 変換前ノード</summary>
		private const string MAPPING_BEFORE_NODE = "Before";
		/// <summary>定数: 変換後ノード</summary>
		private const string MAPPING_AFTER_NODE = "After";

		/// <summary>インスタンス(シングルトン管理)</summary>
		private static LohacoMappingManager m_singletonInstance;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private LohacoMappingManager()
			: base(
				DEFAULT_SETTING_FILE_NAME,
				PROJECT_SETTING_FILE_NAME,
				ROOT_NODE)
		{
		}

		/// <summary>
		/// インスタンス返却
		/// </summary>
		/// <returns>インスタンス</returns>
		public static LohacoMappingManager GetInstance()
		{
			if (m_singletonInstance == null) m_singletonInstance = new LohacoMappingManager();
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
			string orderPaymentKbn = string.Empty;
			if (string.IsNullOrWhiteSpace(paymentMethod))
			{
				return orderPaymentKbn;
			}

			var mappingSettings = base.GetNodeList(mallId, MAPPING_PAYMENT_METHOD_NODE);
			if (mappingSettings == null) return orderPaymentKbn;

			foreach (var paymentNode in mappingSettings.Cast<XmlNode>().Select(x => x))
			{
				var beforeNode = paymentNode.SelectSingleNode(MAPPING_BEFORE_NODE);
				if ((beforeNode != null) && (beforeNode.InnerText.ToUpper() == paymentMethod.ToUpper()))
				{
					orderPaymentKbn = StringUtility.ToEmpty(paymentNode.SelectSingleNode(MAPPING_AFTER_NODE).InnerText);
					break;
				}
			}
			return orderPaymentKbn;
		}

		/// <summary>
		/// 配送希望時間帯設定
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="shippingTime">配送希望時間帯</param>
		/// <returns>配送希望時間帯ID</returns>
		public string ConvertShippingTime(string mallId, string shippingTime)
		{
			string shippingTimeId = string.Empty;
			if (string.IsNullOrWhiteSpace(shippingTime))
			{
				return shippingTimeId;
			}

			var mappingSettings = base.GetNodeList(mallId, MAPPING_SHIPPINGTIME_SETTING_NODE);
			if (mappingSettings == null) return shippingTimeId;

			foreach (var shippingTimeNode in mappingSettings.Cast<XmlNode>().Select(x => x))
			{
				var beforeNode = shippingTimeNode.SelectSingleNode(MAPPING_BEFORE_NODE);
				if ((beforeNode != null) && (beforeNode.InnerText.ToUpper() == shippingTime.ToUpper()))
				{
					shippingTimeId = StringUtility.ToEmpty(shippingTimeNode.SelectSingleNode(MAPPING_AFTER_NODE).InnerText);
					break;
				}
			}
			return shippingTimeId;
		}
	}
}
