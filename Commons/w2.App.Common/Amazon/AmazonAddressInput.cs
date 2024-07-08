/*
=========================================================================================================
  Module      : Amazon住所情報入力クラスモデル(AmazonAddressInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Text;
using Amazon.Pay.API.WebStore.Types;
using AmazonPay.Responses;
using w2.Common.Util;

namespace w2.App.Common.Amazon
{
	/// <summary>
	/// Amazon住所情報入力クラス
	/// </summary>
	[Serializable]
	public class AmazonAddressInput
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AmazonAddressInput()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="orderReference">OrderReferencce</param>
		public AmazonAddressInput(OrderReferenceDetailsResponse orderReference)
			: this()
		{
			this.BuyerShippingName = orderReference.GetBuyerShippingName();
			this.AddressLine1 = orderReference.GetAddressLine1();
			this.AddressLine2 = orderReference.GetAddressLine2();
			this.AddressLine3 = orderReference.GetAddressLine3();
			this.Email = orderReference.GetEmail();
			this.Phone = StringUtility.ToHankaku(StringUtility.ToEmpty(orderReference.GetPhone()));
			this.PhoneArray = this.Phone.Split('-');
			this.PostalCode = orderReference.GetPostalCode();
			this.StateOrRegion = orderReference.GetStateOrRegion();
			this.CountryCode = orderReference.GetCountryCode();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="billingAgreement">BillingAgreement</param>
		public AmazonAddressInput(BillingAgreementDetailsResponse billingAgreement)
			: this()
		{
			this.BuyerShippingName = billingAgreement.GetBuyerShippingName();
			this.AddressLine1 = billingAgreement.GetAddressLine1();
			this.AddressLine2 = billingAgreement.GetAddressLine2();
			this.AddressLine3 = billingAgreement.GetAddressLine3();
			this.Email = billingAgreement.GetEmail();
			this.Phone = StringUtility.ToHankaku(StringUtility.ToEmpty(billingAgreement.GetPhone()));
			this.PhoneArray = this.Phone.Split('-');
			this.PostalCode = billingAgreement.GetPostalCode();
			this.StateOrRegion = billingAgreement.GetStateOrRegion();
			this.CountryCode = billingAgreement.GetCountryCode();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="address">アマゾンペイレスポンスAddress</param>
		/// <param name="email">メールアドレス</param>
		public AmazonAddressInput(Address address, string email = null)
			: this()
		{
			this.BuyerShippingName = address.Name;
			this.AddressLine1 = address.AddressLine1;
			this.AddressLine2 = address.AddressLine2;
			this.AddressLine3 = address.AddressLine3;
			this.Email = StringUtility.ToEmpty(email);
			this.Phone = StringUtility.ToHankaku(StringUtility.ToEmpty(address.PhoneNumber));
			this.PhoneArray = this.Phone.Split('-');
			this.PostalCode = address.PostalCode;
			this.StateOrRegion = address.StateOrRegion;
			this.CountryCode = address.CountryCode;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 入力チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate()
		{
			var errorMessage = new StringBuilder();
			// 名前チェック
			if (string.IsNullOrEmpty(this.BuyerShippingName))
			{
				errorMessage.AppendLine(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_INVALID_NAME_FOR_AMAZON_ADDRESS_WIDGET));
			}

			// 住所チェック
			if (string.IsNullOrEmpty(this.AddressLine1))
			{
				errorMessage.AppendLine(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_INVALID_ADDRESS_FOR_AMAZON_ADDRESS_WIDGET));
			}

			// 都道府県チェック
			if (Constants.STR_PREFECTURES_LIST.Any(prefecture => (prefecture == this.StateOrRegion)) == false)
			{
				errorMessage.AppendLine(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_INVAILD_PREFECTURE_FOR_AMAZON_ADDRESS_WIDGET));
			}

			return StringUtility.ChangeToBrTag(errorMessage.ToString());
		}
		#endregion

		#region プロパティ
		/// <summary>住所ライン1</summary>
		public string AddressLine1 { get; set; }
		/// <summary>住所ライン2</summary>
		public string AddressLine2 { get; set; }
		/// <summary>住所ライン3</summary>
		public string AddressLine3 { get; set; }
		/// <summary>名前または会社名</summary>
		public string BuyerShippingName { get; set; }
		/// <summary>メールアドレス</summary>
		public string Email { get; set; }
		/// <summary>電話番号</summary>
		public string Phone { get; set; }
		/// <summary>郵便番号</summary>
		public string PostalCode { get; set; }
		/// <summary>州または行政区(※日本では都道府県名として利用)</summary>
		public string StateOrRegion { get; set; }
		/// <summary>電話番号1</summary>
		public string Phone1
		{
			get
			{
				if (this.Phone.Contains("-"))
				{
					if (this.PhoneArray.Length == 3) return this.PhoneArray[0];
					this.Phone = this.Phone.Replace("-" , "");
				}
				return (this.Phone.Length == 11)
					? string.Join("", this.Phone.Take(3))
					: string.Join("", this.Phone.Take(2));
			}
		}
		/// <summary>電話番号2</summary>
		public string Phone2
		{
			get
			{
				if (this.Phone.Contains("-"))
				{
					if (this.PhoneArray.Length == 3) return this.PhoneArray[1];
					this.Phone = this.Phone.Replace("-", "");
				}
				return (this.Phone.Length == 11)
					? string.Join("", this.Phone.Skip(3).Take(4))
					: string.Join("", this.Phone.Skip(2).Take(4));
			}
		}
		/// <summary>電話番号3</summary>
		public string Phone3
		{
			get
			{
				if (this.Phone.Contains("-"))
				{
					if (this.PhoneArray.Length == 3) return this.PhoneArray[2];
					this.Phone = this.Phone.Replace("-", "");
				}
				return (this.Phone.Length == 11)
					? string.Join("", this.Phone.Skip(7).Take(4))
					: string.Join("", this.Phone.Skip(6).Take(4));
			}
		}
		private string[] PhoneArray { get; set; }
		/// <summary>国名コード</summary>
		public string CountryCode { get; set; }
		#endregion
	}
}
