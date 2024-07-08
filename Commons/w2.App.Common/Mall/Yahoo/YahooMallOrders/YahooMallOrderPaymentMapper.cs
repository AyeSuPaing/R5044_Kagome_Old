/*
=========================================================================================================
  Module      : Yahooモール注文決済マッピングクラス (YahooMallOrderPaymentMapper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Xml.Serialization;

namespace w2.App.Common.Mall.Yahoo.YahooMallOrders
{
	/// <summary>
	/// Yahooモール注文決済マッピングクラス
	/// </summary>
	public class YahooMallOrderPaymentMapper
	{
		/// <summary>
		/// コンストラクタ (ユニットテスト用)
		/// </summary>
		/// <param name="config">マッピング設定</param>
		public YahooMallOrderPaymentMapper(AddYahooOrder config)
		{
			this.MappingConfig = config;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public YahooMallOrderPaymentMapper()
		{
			// 値定義読み込み
			var path = Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + Constants.FILEPATH_XML_PARAM_DEFINE_SETTING;
			using (var xmlReader = new StreamReader(path))
			{
				var setting = (ParamDefineSetting)new XmlSerializer(typeof(ParamDefineSetting)).Deserialize(xmlReader);
				this.MappingConfig = setting.AddYahooOrder;
			}
		}

		/// <summary>マッピング設定</summary>
		public AddYahooOrder MappingConfig { get; }

		/// <summary>
		/// マッピング設定
		/// </summary>
		[XmlRoot("ParamDefineSetting")]
		public class ParamDefineSetting
		{
			/// <summary>Yahooモール注文</summary>
			[XmlElement("AddYahooOrder")]
			public AddYahooOrder AddYahooOrder { get; set; }
		}

		/// <summary>
		/// Yahooモール注文
		/// </summary>
		[Serializable]
		public class AddYahooOrder
		{
			/// <summary>
			/// 決済マッピング設定
			/// </summary>
			[XmlArray("OrderPaymentKbnAndPaymentStatusSettings")]
			[XmlArrayItem("Setting")]
			public Setting[] Settings { get; set; }

			/// <summary>
			/// カスタムフィールド (注文メモへ書き込む)
			/// </summary>
			[XmlArray("CustomFields")]
			[XmlArrayItem("CustomField")]
			public string[] CustomFields { get; set; } = new string[] { };
		}

		/// <summary>
		/// 決済マッピング設定
		/// </summary>
		[Serializable]
		public class Setting
		{
			/// <summary>決済方法 物理名</summary>
			[XmlAttribute("name")]
			public string YahooPayMethodPhysicalName { get; set; } = "";
			/// <summary>決済方法 論理名</summary>
			[XmlAttribute("value")]
			public string YahooPayMethodLogicalName { get; set; } = "";
			/// <summary>決済方法</summary>
			[XmlAttribute("payment_kbn")]
			public string PaymentKbn { get; set; } = "";
			/// <summary>決済ステータス</summary>
			[XmlAttribute("payment_status")]
			public string PaymentStatus { get; set; } = "";
		}
	}
}
