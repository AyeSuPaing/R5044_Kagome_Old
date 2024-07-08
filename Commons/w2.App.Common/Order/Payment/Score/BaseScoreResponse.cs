/*
=========================================================================================================
  Module      : Score後払いレスポンス用データの基底クラス(BaseScoreResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Linq;
using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Score.ObjectsElement;
using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.Score
{
	/// <summary>
	/// レスポンス用データの基底クラス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public abstract class BaseScoreResponse : IHttpApiResponseData
	{
		/// <summary>
		/// レスポンス文字列生成
		/// </summary>
		/// <returns>レスポンス文字列</returns>
		public virtual string CreateResponseString()
		{
			return SerializeHelper.Serialize(this);
		}

		///// <summary>処理結果</summary>
		[XmlElement("result")]
		public string Result { get; set; }
		/// <summary>エラー情報</summary>
		[XmlElement("transactionResult")]
		public TransactionResultElement TransactionResult { get; set; }
		/// <summary>エラー情報</summary>
		[XmlElement("errors")]
		public ErrorElements Errors { get; set; }
	}
}
