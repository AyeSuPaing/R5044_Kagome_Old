/*
=========================================================================================================
  Module      : CrossPoint API ユーザー結果モデル (UserApiResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml;
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.User
{
	/// <summary>
	/// ユーザー結果モデル
	/// </summary>
	public class UserApiResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserApiResult()
		{
			this.No = 0;
			this.LastName = string.Empty;
			this.FirstName = string.Empty;
			this.Name = string.Empty;
			this.LastNamePhonetic = string.Empty;
			this.FirstNamePhonetic = string.Empty;
			this.NamePhonetic = string.Empty;
			this.AdmissionShopName = string.Empty;
			this.MemberRankId = string.Empty;
			this.MemberRankName = string.Empty;
			this.Sex = string.Empty;
			this.Birthday = string.Empty;
			this.Postcode = string.Empty;
			this.PrefName = string.Empty;
			this.City = string.Empty;
			this.Town = string.Empty;
			this.Address = string.Empty;
			this.Building = string.Empty;
			this.Tel = string.Empty;
			this.MbTel = string.Empty;
			this.PcMail = string.Empty;
			this.MbMail = string.Empty;
			this.PostcardDmUnnecessaryFlg = string.Empty;
			this.EmailDmUnnecessaryFlg = string.Empty;
			this.MemberStatus = string.Empty;
			this.BlackFlg = string.Empty;
			this.Remarks1 = string.Empty;
			this.Remarks2 = string.Empty;
			this.Remarks3 = string.Empty;
			this.UseUnitPoint = string.Empty;
			this.RealizationAmount = string.Empty;
			this.TempAdmissionDatetime = string.Empty;
			this.AdmissionDatetime = string.Empty;
			this.EffectivePoint = string.Empty;
			this.TempGrantPoint = string.Empty;
			this.TotalGrantPoint = string.Empty;
			this.TotalUsePoint = string.Empty;
			this.FirstPurchaseDate = string.Empty;
			this.LastPurchaseDate = string.Empty;
			this.LastLoginDatetime = string.Empty;
			this.TotalLoginCount = string.Empty;
			this.TotalPurchaseAmount = string.Empty;
			this.AveragePurchaseAmount = string.Empty;
			this.MaxPurchaseAmount = string.Empty;
			this.TotalPurchaseCount = string.Empty;
			this.AveragePurchaseNum = string.Empty;
			this.MaxPurchaseNum = string.Empty;
			this.PurchaseFrequency = string.Empty;
			this.ExceptInvalidPoint = string.Empty;
			this.ExceptInvalidDate = string.Empty;
			this.RealShopCardNo = string.Empty;
			this.PinCode = string.Empty;
			this.NetShopMemberId = string.Empty;
			this.TotalPurchaseAmountForRankUpdate = string.Empty;
			this.TotalPurchaseCountForRankUpdate = string.Empty;
			this.MemberRankUpNextAmount = string.Empty;
			this.MemberRankUpNextCount = string.Empty;
			this.NextRank = string.Empty;
			this.CustomAttribute = string.Empty;
		}

		/// <summary>連番</summary>
		[XmlElement("No")]
		public int No { get; set; }
		/// <summary>姓</summary>
		[XmlElement("LastName")]
		public string LastName { get; set; }
		/// <summary>名</summary>
		[XmlElement("FirstName")]
		public string FirstName { get; set; }
		/// <summary>姓 名</summary>
		[XmlElement("Name")]
		public string Name { get; set; }
		/// <summary>姓(カナ)</summary>
		[XmlElement("LastNameKana")]
		public string LastNamePhonetic { get; set; }
		/// <summary>名(カナ)</summary>
		[XmlElement("FirstNameKana")]
		public string FirstNamePhonetic { get; set; }
		/// <summary>姓 名(カナ)</summary>
		[XmlElement("NameKana")]
		public string NamePhonetic { get; set; }
		/// <summary>入会ショップ名</summary>
		[XmlElement("AdmissionShopName")]
		public string AdmissionShopName { get; set; }
		/// <summary>会員ランクID</summary>
		[XmlElement("MemberRankId")]
		public string MemberRankId { get; set; }
		/// <summary>会員ランク名</summary>
		[XmlElement("MemberRankName")]
		public string MemberRankName { get; set; }
		/// <summary>性別</summary>
		[XmlElement("Sex")]
		public string Sex { get; set; }
		/// <summary>生年月日</summary>
		[XmlElement("Birthday")]
		public string Birthday { get; set; }
		/// <summary>郵便番号</summary>
		[XmlElement("Postcode")]
		public string Postcode { get; set; }
		/// <summary>都道府県</summary>
		[XmlElement("PrefName")]
		public string PrefName { get; set; }
		/// <summary>市区町村</summary>
		[XmlElement("City")]
		public string City { get; set; }
		/// <summary>町域</summary>
		[XmlElement("Town")]
		public string Town { get; set; }
		/// <summary>番地</summary>
		[XmlElement("Address")]
		public string Address { get; set; }
		/// <summary>ビル等</summary>
		[XmlElement("Building")]
		public string Building { get; set; }
		/// <summary>電話番号</summary>
		[XmlElement("Tel")]
		public string Tel { get; set; }
		/// <summary>携帯電話番号</summary>
		[XmlElement("MbTel")]
		public string MbTel { get; set; }
		/// <summary>PCメールアドレス</summary>
		[XmlElement("PcMail")]
		public string PcMail { get; set; }
		/// <summary>モバイルメールアドレス</summary>
		[XmlElement("MbMail")]
		public string MbMail { get; set; }
		/// <summary>郵便DM不要フラグ</summary>
		[XmlElement("PostcardDmUnnecessaryFlg")]
		public string PostcardDmUnnecessaryFlg { get; set; }
		/// <summary>メールDM不要フラグ</summary>
		[XmlElement("EmailDmUnnecessaryFlg")]
		public string EmailDmUnnecessaryFlg { get; set; }
		/// <summary>会員ステータス</summary>
		[XmlElement("MemberSts")]
		public string MemberStatus { get; set; }
		/// <summary>ブラックフラグ</summary>
		[XmlElement("BlackFlg")]
		public string BlackFlg { get; set; }
		/// <summary>備考1</summary>
		[XmlElement("Remarks1")]
		public string Remarks1 { get; set; }
		/// <summary>備考2</summary>
		[XmlElement("Remarks2")]
		public string Remarks2 { get; set; }
		/// <summary>備考3</summary>
		[XmlElement("Remarks3")]
		public string Remarks3 { get; set; }
		/// <summary>利用単位ポイント</summary>
		[XmlElement("UseUnitPoint")]
		public string UseUnitPoint { get; set; }
		/// <summary>換金額</summary>
		[XmlElement("RealizationAmount")]
		public string RealizationAmount { get; set; }
		/// <summary>仮入会日時</summary>
		[XmlElement("TempAdmissionDatetime")]
		public string TempAdmissionDatetime { get; set; }
		/// <summary>本入会日時</summary>
		[XmlElement("AdmissionDatetime")]
		public string AdmissionDatetime { get; set; }
		/// <summary>有効ポイント</summary>
		[XmlElement("EffectivePoint")]
		public string EffectivePoint { get; set; }
		/// <summary>仮付与ポイント</summary>
		[XmlElement("TempGrantPoint")]
		public string TempGrantPoint { get; set; }
		/// <summary>累計付与ポイント</summary>
		[XmlElement("TotalGrantPoint")]
		public string TotalGrantPoint { get; set; }
		/// <summary>累計利用ポイント</summary>
		[XmlElement("TotalUsePoint")]
		public string TotalUsePoint { get; set; }
		/// <summary>初回購入日</summary>
		[XmlElement("FirstPurchaseDate")]
		public string FirstPurchaseDate { get; set; }
		/// <summary>最終購入日</summary>
		[XmlElement("LastPurchaseDate")]
		public string LastPurchaseDate { get; set; }
		/// <summary>最終ログイン日時</summary>
		[XmlElement("LastLoginDatetime")]
		public string LastLoginDatetime { get; set; }
		/// <summary>累計ログイン回数</summary>
		[XmlElement("TotalLoginCount")]
		public string TotalLoginCount { get; set; }
		/// <summary>累計購入金額</summary>
		[XmlElement("TotalPurchaseAmount")]
		public string TotalPurchaseAmount { get; set; }
		/// <summary>平均購入金額</summary>
		[XmlElement("AveragePurchaseAmount")]
		public string AveragePurchaseAmount { get; set; }
		/// <summary>最高購入金額</summary>
		[XmlElement("MaxPurchaseAmount")]
		public string MaxPurchaseAmount { get; set; }
		/// <summary>累計購入回数</summary>
		[XmlElement("TotalPurchaseCount")]
		public string TotalPurchaseCount { get; set; }
		/// <summary>平均購入点数</summary>
		[XmlElement("AveragePurchaseNum")]
		public string AveragePurchaseNum { get; set; }
		/// <summary>最高購入点数</summary>
		[XmlElement("MaxPurchaseNum")]
		public string MaxPurchaseNum { get; set; }
		/// <summary>購入頻度</summary>
		[XmlElement("PurchaseFrequecy")]
		public string PurchaseFrequency { get; set; }
		/// <summary>直近失効予定ポイント</summary>
		[XmlElement("ExceptInvalidPoint")]
		public string ExceptInvalidPoint { get; set; }
		/// <summary>直近失効予定日</summary>
		[XmlElement("ExceptInvalidDate")]
		public string ExceptInvalidDate { get; set; }
		/// <summary>リアル店舗カード番号</summary>
		[XmlElement("RealShopCardNo")]
		public string RealShopCardNo { get; set; }
		/// <summary>PINコード</summary>
		[XmlElement("PinCd")]
		public string PinCode { get; set; }
		/// <summary>その他</summary>
		[XmlAnyElement]
		public XmlElement[] OtherElements { get; set; }
		/// <summary>ネットショップ会員ID</summary>
		public string NetShopMemberId { get; set; }
		/// <summary>ランク査定対象累計購入金額</summary>
		[XmlElement("TotalPurchaseAmountForRankUpdate")]
		public string TotalPurchaseAmountForRankUpdate { get; set; }
		/// <summary>ランク査定対象累計購入回数</summary>
		[XmlElement("TotalPurchaseCountForRankUpdate")]
		public string TotalPurchaseCountForRankUpdate { get; set; }
		/// <summary>次のランクまでに必要な購入金額</summary>
		[XmlElement("MemberRankUpNextAmount")]
		public string MemberRankUpNextAmount { get; set; }
		/// <summary>次のランクまでに必要な購入回数</summary>
		[XmlElement("MemberRankUpNextCount")]
		public string MemberRankUpNextCount { get; set; }
		/// <summary>次のランク</summary>
		[XmlElement("NextRank")]
		public string NextRank { get; set; }
		/// <summary>カスタム属性</summary>
		public string CustomAttribute { get; set; }
	}
}
