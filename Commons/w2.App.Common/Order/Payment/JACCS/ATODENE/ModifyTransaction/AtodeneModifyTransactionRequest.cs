/*
=========================================================================================================
  Module      : Atodene取引変更・キャンセルリクエスト(AtodeneModifyTransactionRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.ModifyTransaction
{
	/// <summary>
	/// Atodene取引変更・キャンセルリクエスト
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class AtodeneModifyTransactionRequest : BaseAtodeneRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AtodeneModifyTransactionRequest()
			: base()
		{

		}

		/// <summary>購入者情報</summary>
		[XmlElement("transactionInfo")]
		public TransactionInfoElement TransactionInfo { get; set; }

		/// <summary>購入者情報</summary>
		[XmlElement("customer")]
		public CustomerElement Customer { get; set; }

		/// <summary>配送先情報</summary>
		[XmlElement("ship")]
		public ShipElement Ship { get; set; }

		/// <summary>明細詳細項目</summary>
		[XmlElement("details")]
		public DetailsElement Details { get; set; }

		/// <summary>
		/// 購入者情報要素
		/// </summary>
		public class TransactionInfoElement
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public TransactionInfoElement()
			{
				this.UpdateTypeFlag = "";
				this.TransactionId = "";
			}

			/// <summary>更新種別フラグ</summary>
			[XmlElement("updateTypeFlag")]
			public string UpdateTypeFlag { get; set; }

			/// <summary>お問合せ番号</summary>
			[XmlElement("transactionId")]
			public string TransactionId { get; set; }
		}

		/// <summary>
		/// 購入者情報要素
		/// </summary>
		public class CustomerElement
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public CustomerElement()
			{
				this.ShopOrderId = "";
				this.ShopOrderDate = "";
				this.Name = "";
				this.KanaName = "";
				this.Zip = "";
				this.Address = "";
				this.CompanyName = "";
				this.SectionName = "";
				this.Tel = "";
				this.Email = "";
				this.BilledAmount = "0";
				this.Expand1 = AtodeneConst.EXTEND1_SHIPING_COMP_FLG_SKIP;
				this.Service = AtodeneConst.INVOICE_SEND_SERVICE_FLG_SEPARATE;
			}

			/// <summary>ご購入店受注番号</summary>
			[XmlElement("shopOrderId")]
			public string ShopOrderId { get; set; }

			/// <summary>購入者注文日(yyyy/mm/dd形式)</summary>
			[XmlElement("shopOrderDate")]
			public string ShopOrderDate { get; set; }

			/// <summary>氏名（漢字）</summary>
			[XmlElement(ElementName = "name")]
			public string Name { get; set; }

			/// <summary>氏名（カナ）</summary>
			[XmlElement("kanaName")]
			public string KanaName { get; set; }

			/// <summary>郵便番号</summary>
			[XmlElement("zip")]
			public string Zip { get; set; }

			/// <summary>住所</summary>
			[XmlElement("address")]
			public string Address { get; set; }

			/// <summary>会社名</summary>
			[XmlElement("companyName")]
			public string CompanyName { get; set; }

			/// <summary>部署名</summary>
			[XmlElement("sectionName")]
			public string SectionName { get; set; }

			/// <summary>電話番号</summary>
			[XmlElement("tel")]
			public string Tel { get; set; }

			/// <summary>メールアドレス</summary>
			[XmlElement("email")]
			public string Email { get; set; }

			/// <summary>顧客請求金額（税込）</summary>
			[XmlElement("billedAmount")]
			public string BilledAmount { get; set; }

			/// <summary>拡張審査パラメータ</summary>
			[XmlElement("expand1")]
			public string Expand1 { get; set; }

			/// <summary>請求書送付方法</summary>
			[XmlElement("service")]
			public string Service { get; set; }
		}

		/// <summary>
		/// 配送先情報要素
		/// </summary>
		public class ShipElement
		{
			/// <summary>コンストラクタ</summary>
			public ShipElement()
			{
				this.ShipName = "";
				this.ShipKananame = "";
				this.ShipZip = "";
				this.ShipAddress = "";
				this.ShipCompanyName = "";
				this.ShipSectionName = "";
				this.ShipTel = "";
			}

			/// <summary>氏名（漢字）</summary>
			[XmlElement("shipName")]
			public string ShipName { get; set; }

			/// <summary>氏名（カナ）</summary>
			[XmlElement("shipKananame")]
			public string ShipKananame { get; set; }

			/// <summary>郵便番号</summary>
			[XmlElement("shipZip")]
			public string ShipZip { get; set; }

			/// <summary>住所</summary>
			[XmlElement("shipAddress")]
			public string ShipAddress { get; set; }

			/// <summary>会社名</summary>
			[XmlElement("shipCompanyName")]
			public string ShipCompanyName { get; set; }

			/// <summary>部署名</summary>
			[XmlElement("shipSectionName")]
			public string ShipSectionName { get; set; }

			/// <summary>電話番号</summary>
			[XmlElement("shipTel")]
			public string ShipTel { get; set; }
		}

		/// <summary>
		/// 明細詳細項目要素
		/// </summary>
		public class DetailsElement
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public DetailsElement()
			{
				this.Detail = new DetailElement[] { };
			}

			/// <summary>明細詳細情報</summary>
			[XmlElement("detail")]
			public DetailElement[] Detail { get; set; }
		}

		/// <summary>
		/// 明細詳細情報要素
		/// </summary>
		public class DetailElement
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public DetailElement()
			{
				this.Goods = "";
				this.GoodsPrice = "0";
				this.GoodsAmount = "0";
				this.Expand2 = "1";
				this.Expand3 = "";
				this.Expand4 = "";
			}

			/// <summary>明細名（商品名）</summary>
			[XmlElement("goods")]
			public string Goods { get; set; }

			/// <summary>単価（税込）</summary>
			[XmlElement("goodsPrice")]
			public string GoodsPrice { get; set; }

			/// <summary>数量</summary>
			[XmlElement("goodsAmount")]
			public string GoodsAmount { get; set; }

			/// <summary>明細表示順</summary>
			[XmlElement("expand2")]
			public string Expand2 { get; set; }

			/// <summary>拡張項目３</summary>
			[XmlElement("expand3")]
			public string Expand3 { get; set; }

			/// <summary>拡張項目４</summary>
			[XmlElement("expand4")]
			public string Expand4 { get; set; }
		}
	}
}

