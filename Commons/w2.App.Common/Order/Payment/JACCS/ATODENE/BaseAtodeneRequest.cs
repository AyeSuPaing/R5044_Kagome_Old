/*
=========================================================================================================
  Module      : Atodene規定リクエスト(BaseAtodeneRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE
{
	/// <summary>
	/// Atodene規定リクエスト
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public abstract class BaseAtodeneRequest : IHttpApiRequestData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected BaseAtodeneRequest()
		{
			this.LinkInfo = new LinkInfoElement();
		}

		/// <summary>連携情報</summary>
		[XmlElement("linkInfo")]
		public LinkInfoElement LinkInfo { get; set; }

		/// <summary>
		/// ポスト値生成
		/// </summary>
		/// <returns></returns>
		public string CreatePostString()
		{
			// アトディーネの連携先が、空要素のフォーマットが<***></***>しか許容されない独自仕様なのでここで何とかする
			// XmlTextWriterを拡張して対応
			using (var stream = new MemoryStream())
			using (var writer = new ExtendXmlTextWriter(stream, Encoding.UTF8))
			using (var reader = new StreamReader(stream))
			{
				var ns = new XmlSerializerNamespaces();
				ns.Add("", "");
				new XmlSerializer(this.GetType()).Serialize(writer, this, ns);
				stream.Seek(0, SeekOrigin.Begin);
				var s = reader.ReadToEnd();
				return s;
			}
		}

		/// <summary>
		/// 認証情報要素
		/// </summary>
		public class LinkInfoElement
		{
			/// <summary>コンストラクタ</summary>
			public LinkInfoElement()
			{
				this.ShopCode = Constants.PAYMENT_SETTING_ATODENE_SHOP_CODE;
				this.LinkID = Constants.PAYMENT_SETTING_ATODENE_LINK_ID;
				this.LinkPassword = Constants.PAYMENT_SETTING_ATODENE_LINK_PASSWORD;
			}

			/// <summary>加盟店コード</summary>
			/// <remarks>加盟時に付与される加盟店を一意にするコード</remarks>
			[XmlElement("shopCode")]
			public string ShopCode { get; set; }

			/// <summary>接続先特定ID</summary>
			/// <remarks>接続先を一意にするID</remarks>
			[XmlElement("linkId")]
			public string LinkID { get; set; }

			/// <summary>連携パスワード</summary>
			/// <remarks>APIを利用するためのパスワード</remarks>
			[XmlElement("linkPassword")]
			public string LinkPassword { get; set; }
		}
	}

	/// <summary>
	/// 空要素の場合に長形式のタグを出すための拡張ライタ
	/// </summary>
	public class ExtendXmlTextWriter : XmlTextWriter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="stream">ストリーム</param>
		/// <param name="enc">エンコード</param>
		public ExtendXmlTextWriter(Stream stream, Encoding enc)
			: base(stream, enc)
		{
		}

		/// <summary>
		/// WriteEndElementをオーバーライド
		/// </summary>
		public override void WriteEndElement()
		{
			// 要素終了の際は対応要素全ぶっぱ
			base.WriteFullEndElement();
		}
	}
}
