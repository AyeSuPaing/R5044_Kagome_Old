/*
=========================================================================================================
  Module      : Atodene与信取得レスポンス(AtodeneGetAuthResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.GetAuth
{
	/// <summary>
	/// Atodene与信取得レスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class AtodeneGetAuthResponse : BaseAtodeneResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AtodeneGetAuthResponse()
			: base()
		{
		}

		/// <summary>取引情報</summary>
		[XmlElement("transactionInfo")]
		public TransactionInfoElement TransactionInfo { get; set; }

		/// <summary>
		/// 取引情報要素
		/// </summary>
		public class TransactionInfoElement
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public TransactionInfoElement()
			{
				this.TransactionId = "";
			}

			/// <summary>お問合せ番号</summary>
			[XmlElement("transactionId")]
			public string TransactionId { get; set; }

			/// <summary>自動審査結果</summary>
			[XmlElement("autoAuthoriresult")]
			public string AutoAuthoriresult { get; set; }

			/// <summary>目視審査結果</summary>
			[XmlElement("manualAuthoriresult")]
			public string ManualAuthoriresult { get; set; }

			/// <summary>目視審査結果理由項目</summary>
			[XmlElement("manualAuthorireasons")]
			public ManualAuthorireasonsElement ManualAuthorireasons { get; set; }

			/// <summary>
			/// 目視審査結果理由項目要素
			/// </summary>
			public class ManualAuthorireasonsElement
			{
				/// <summary>
				/// コンストラクタ
				/// </summary>
				public ManualAuthorireasonsElement()
				{
					this.ManualAuthorireason = new string[] { };
				}

				/// <summary>目視審査結果理由項目</summary>
				[XmlElement("manualAuthorireason")]
				public string[] ManualAuthorireason { get; set; }
			}
		}
	}
}
