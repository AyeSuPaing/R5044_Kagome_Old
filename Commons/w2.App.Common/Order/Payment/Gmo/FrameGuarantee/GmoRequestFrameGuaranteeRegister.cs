/*
=========================================================================================================
  Module      : Gmo Response Frame Guarantee Get Status(GmoResponseFrameGuaranteeGetStatus.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using w2.Common.Util;
using w2.Domain.User;
using w2.Domain.UserBusinessOwner;

namespace w2.App.Common.Order.Payment.GMO.FrameGuarantee
{
	/// <summary>
	/// GMOリクエスト枠保証登録
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class GmoRequestFrameGuaranteeRegister : BaseGmoRequest
	{
		/// <summary>コンストラクタ</summary>
		public GmoRequestFrameGuaranteeRegister()
			: base()
		{
			this.Buyer = new BuyerElement();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="user">ユーザー</param>
		/// <param name="userBusinessOwner">ビジネスオーナーのモデル</param>
		public GmoRequestFrameGuaranteeRegister(UserModel user, UserBusinessOwnerModel userBusinessOwner)
			: base()
		{
			this.CreditFacility = new CreditFacility();
			this.Buyer = new BuyerElement();
			this.Buyer.BuyerNameFamily = user.Name1;
			this.Buyer.BuyerName = user.Name2;
			this.Buyer.BuyerNameFamilyKana = StringUtility.ToZenkakuKatakana(user.NameKana1);
			this.Buyer.BuyerNameKana = StringUtility.ToZenkakuKatakana(user.NameKana2); 
			this.Buyer.ZipCode = user.Zip;
			this.Buyer.Address = user.Addr;
			this.Buyer.CompanyName = user.CompanyName;
			this.Buyer.DepartmentName = user.CompanyPostName;
			this.Buyer.Tel1 = user.Tel1;
			this.Buyer.Tel2 = user.Tel2;
			this.Buyer.Email1 = user.MailAddr;
			this.Buyer.Email2 = user.MailAddr2;
			this.Buyer.Birthday = string.Format("{0:yyyyMMdd}", userBusinessOwner.Birth);
			this.Buyer.PresidentNameFamily = userBusinessOwner.OwnerName1;
			this.Buyer.PresidentName = userBusinessOwner.OwnerName2;
			this.Buyer.PresidentNameFamilyKana = userBusinessOwner.OwnerNameKana1;
			this.Buyer.PresidentNameKana = userBusinessOwner.OwnerNameKana2;
			this.Buyer.ShopCustomerId = userBusinessOwner.ShopCustomerId;
			if (userBusinessOwner.RequestBudget > 0)
			{
				this.CreditFacility.ReqUpperLimit = userBusinessOwner.RequestBudget;
				this.CreditFacility.Apply = 1;
			}
		}

		/// <summary>購入者情報</summary>
		[XmlElement("buyer")]
		public BuyerElement Buyer;

		/// <summary>購入者情報</summary>
		[XmlElement("creditFacility")]
		public CreditFacility CreditFacility;
	}

	#region BuyerElement 購入者情報要素
	/// <summary>
	/// 購入者情報要素
	/// </summary>
	public class BuyerElement
	{
		/// <summary>コンストラクタ</summary>
		public BuyerElement()
		{
			this.BuyerNameFamily = "";
			this.BuyerName = "";
			this.BuyerNameFamilyKana = "";
			this.BuyerNameKana = "";
			this.ZipCode = "";
			this.Address = "";
			this.CompanyName = "";
			this.DepartmentName = "";
			this.Tel1 = "";
			this.Tel2 = "";
			this.Email1 = "";
			this.Email2 = "";
			this.PresidentNameFamily = "";
			this.PresidentName = "";
			this.PresidentNameFamilyKana = "";
			this.PresidentNameKana = "";
			this.Birthday = "";
			this.ShopCustomerId = "";
			this.CompanyName = "";
		}

		/// <summary>購入者の名前（姓）</summary>
		[XmlElement("buyerNameFamily")]
		public string BuyerNameFamily;

		/// <summary>購入者の名前（名）</summary>
		[XmlElement("buyerName")]
		public string BuyerName;

		/// <summary>購入者の名前（姓）（カナ）</summary>
		[XmlElement("buyerNameFamilyKana")]
		public string BuyerNameFamilyKana;

		/// <summary>購入者の名前（名）（カナ）</summary>
		[XmlElement("buyerNameKana")]
		public string BuyerNameKana;

		/// <summary>郵便番号</summary>
		[XmlElement("zipCode")]
		public string ZipCode;

		/// <summary>住所</summary>
		[XmlElement("address")]
		public string Address;

		/// <summary>会社名</summary>
		[XmlElement("companyName")]
		public string CompanyName;

		/// <summary>部署名</summary>
		[XmlElement("departmentName")]
		public string DepartmentName;

		/// <summary>電話番号１</summary>
		[XmlElement("tel1")]
		public string Tel1;

		/// <summary>電話番号２</summary>
		[XmlElement("tel2")]
		public string Tel2;

		/// <summary>メールアドレス１</summary>
		[XmlElement("email1")]
		public string Email1;

		/// <summary>メールアドレス２</summary>
		[XmlElement("email2")]
		public string Email2;

		/// <summary>社長の名前（姓）</summary>
		[XmlElement("presidentNameFamily")]
		public string PresidentNameFamily;

		/// <summary>社長の名前（名）</summary>
		[XmlElement("presidentName")]
		public string PresidentName;

		/// <summary>社長の名前（姓）（カナ）</summary>
		[XmlElement("presidentNameFamilyKana")]
		public string PresidentNameFamilyKana;

		/// <summary>社長の名前（名）（カナ）</summary>
		[XmlElement("presidentNameKana")]
		public string PresidentNameKana;

		/// <summary>誕生日</summary>
		[XmlElement("birthday")]
		public string Birthday;

		/// <summary>ショップの顧客ID</summary>
		[XmlElement("shopCustomerId")]
		public string ShopCustomerId;

		/// <summary>誕生日</summary>
		[XmlElement("corpNumber")]
		public string CorpNumber;
	}
	#endregion

	#region CreditFacility
	/// <summary>
	/// CreditFacility
	/// </summary>
	public class CreditFacility
	{
		/// <summary>コンストラクタ</summary>
		public CreditFacility()
		{
			this.Apply = 0;
			this.ReqUpperLimit = 0;
		}

		/// <summary>Apply</summary>
		[XmlElement("apply")]
		public int Apply;

		/// <summary>Req Upper Limit</summary>
		[XmlElement("reqUpperLimit")]
		public int ReqUpperLimit;
	}
	#endregion
}
