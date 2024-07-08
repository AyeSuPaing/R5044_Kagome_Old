/*
=========================================================================================================
  Module      : Score後払いリクエストデータ用の基底クラス(BaseScoreRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Score.ObjectsElement;
using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.Score
{
	/// <summary>
	/// リクエストデータ用の基底クラス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public abstract class BaseScoreRequest : IHttpApiRequestData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected BaseScoreRequest()
		{
			this.ShopInfo = new ShopInfoElement();
		}

		/// <summary>
		/// ポスト値生成
		/// </summary>
		/// <returns>生成したポスト値</returns>
		public virtual string CreatePostString()
		{
			return SerializeHelper.Serialize(this);
		}

		/// <summary>認証情報</summary>
		[XmlElement("shopInfo")]
		public ShopInfoElement ShopInfo { get; set; }
	}
}
