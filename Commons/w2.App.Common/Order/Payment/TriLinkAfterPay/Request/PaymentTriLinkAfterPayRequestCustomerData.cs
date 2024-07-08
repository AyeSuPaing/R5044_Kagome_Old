/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) リクエストデータ(Customer)生成クラス(PaymentTriLinkAfterPayRequestCustomerData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System;
using Newtonsoft.Json;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Request
{
	/// <summary>
	/// 後付款(TriLink後払い) リクエストデータ(Customer)生成クラス
	/// </summary>
	[JsonObject]
	public class PaymentTriLinkAfterPayRequestCustomerData
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="owner">注文者</param>
		/// <param name="userId">ユーザID</param>
		public PaymentTriLinkAfterPayRequestCustomerData(CartOwner owner, string userId)
			: this(
				owner.CompanyName,
				owner.CompanyPostName,
				owner.Name,
				owner.Zip,
				owner.Addr2 + owner.Addr3 + owner.Addr4,
				owner.Tel1,
				owner.Tel2,
				owner.MailAddr,
				userId,
				owner.Sex,
				owner.Birth)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="owner">注文者</param>
		/// <param name="userId">ユーザID</param>
		public PaymentTriLinkAfterPayRequestCustomerData(OrderOwnerModel owner, string userId)
			: this(
				owner.OwnerCompanyName,
				owner.OwnerCompanyPostName,
				owner.OwnerName,
				owner.OwnerZip,
				owner.OwnerAddr2 + owner.OwnerAddr3 + owner.OwnerAddr4,
				owner.OwnerTel1,
				owner.OwnerTel2,
				owner.OwnerMailAddr,
				userId,
				owner.OwnerSex,
				owner.OwnerBirth)
		{
		}
		/// <summary>
		/// コンストラクタ 
		/// </summary>
		/// <param name="companyName">購入者会社名</param>
		/// <param name="departmentName">購入者部署名</param>
		/// <param name="name">購入者名</param>
		/// <param name="zipCode">購入者郵便番号</param>
		/// <param name="address">購入者住所</param>
		/// <param name="mobile">携帯番号</param>
		/// <param name="landLine">固定電話番号</param>
		/// <param name="email">購入者メールアドレス</param>
		/// <param name="id">身分証番号</param>
		/// <param name="sex">性別</param>
		/// <param name="birthday">誕生日</param>
		private PaymentTriLinkAfterPayRequestCustomerData(
			string companyName,
 			string departmentName,
			string name,
			string zipCode,
			string address,
			string mobile,
			string landLine,
			string email,
			string id,
			string sex,
			DateTime? birthday)
		{
			this.CompanyName = companyName;
			this.DepartmentName = departmentName;
			this.Name = name;
			this.ZipCode = zipCode;
			this.Address = address;
			this.Mobile = mobile;
			this.LandLine = landLine;
			this.Email = email;
			this.Id = id;
			this.Sex = (sex == Constants.FLG_ORDEROWNER_OWNER_SEX_MALE)
				? TriLinkAfterPayConstants.FLG_TW_AFTERPAY_SEX_MALE
				: (sex == Constants.FLG_ORDEROWNER_OWNER_SEX_FEMALE)
					? TriLinkAfterPayConstants.FLG_TW_AFTERPAY_SEX_FEMALE
					: "";
			if (birthday.HasValue) this.Birthday = birthday.Value.ToString("yyyy-MM-dd");
		}
		#endregion

		#region プロパティ
		/// <summary>購入者会社名</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_COMPANY_NAME)]
		public string CompanyName { get; set; }
		/// <summary>購入者部署名</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_DEPARTMENT_NAME)]
		public string DepartmentName { get; set; }
		/// <summary>購入者名</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_NAME)]
		public string Name { get; set; }	
		/// <summary>購入者郵便番号</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ZIP_CODE)]
		public string ZipCode { get; set; }
		/// <summary>購入者住所</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ADDRESS)]
		public string Address { get; set; }
		/// <summary>携帯番号</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_MOBILE)]
		public string Mobile { get; set; }
		/// <summary>固定電話番号</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_LAND_LINE)]
		public string LandLine { get; set; }
		/// <summary>購入者メールアドレス</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_EMAIL)]
		public string Email { get; set; }
		/// <summary>身分証番号</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ID)]
		public string Id { get; set; }
		/// <summary>性別</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_SEX)]
		public string Sex { get; set; }
		/// <summary>誕生日</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_BIRTHDAY)]
		public string Birthday { get; set; }
		#endregion
	}
}