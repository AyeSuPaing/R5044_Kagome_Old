/*
=========================================================================================================
  Module      : Login Response(LoginResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.User.Botchan
{
	/// <summary>
	/// Login Response
	/// </summary>
	[Serializable]
	public class LoginResponse
	{
		/// <summary>Result</summary>
		[JsonProperty("result")]
		public bool Result { get; set; }
		/// <summary>Message Id</summary>
		[JsonProperty("message_id")]
		public string MessageId { get; set; }
		/// <summary>Message</summary>
		[JsonProperty("message")]
		public string Message { get; set; }
		/// <summary>ログイン処理結果レスポンス</summary>
		[JsonProperty("data")]
		public Data Data { get; set; }
	}

	/// <summary>
	/// Data
	/// </summary>
	[Serializable]
	public class Data
	{
		/// <summary>User</summary>
		[JsonProperty("user")]
		public User User { get; set; }
		/// <summary>User Attribute</summary>
		[JsonProperty("userAttribute")]
		public UserAttribute UserAttribute { get; set; }
		/// <summary>User Extend</summary>
		[JsonProperty("userExtend")]
		public UserExtendSetting[] UserExtendSetting { get; set; }
		/// <summary>User Credit Card</summary>
		[JsonProperty("userCreditCard")]
		public UserCreditCard[] UserCreditCard { get; set; }
		/// <summary>User Shipping</summary>
		[JsonProperty("userShipping")]
		public UserShipping[] UserShipping { get; set; }
		/// <summary>User Default Order Setting</summary>
		[JsonProperty("userDefaultOrderSetting")]
		public UserDefaultOrderSetting UserDefaultOrderSetting { get; set; }
		/// <summary>User Point</summary>
		[JsonProperty("userPoint")]
		public UserPoint[] UserPoint { get; set; }
		/// <summary>User Coupon</summary>
		[JsonProperty("userCoupon")]
		public UserCoupon[] UserCoupon { get; set; }
	}

	/// <summary>
	/// User
	/// </summary>
	[Serializable]
	public class User
	{
		/// <summary>ユーザID</summary>
		[JsonProperty("user_id")]
		public string UserId { get; set; }
		/// <summary>顧客区分</summary>
		[JsonProperty("user_kbn")]
		public string UserKbn { get; set; }
		/// <summary>氏名</summary>
		[JsonProperty("name")]
		public string Name { get; set; }
		/// <summary>ニックネーム</summary>
		[JsonProperty("nick_name")]
		public string NickName { get; set; }
		/// <summary>メールアドレス</summary>
		[JsonProperty("mail_addr")]
		public string MailAddr { get; set; }
		/// <summary>郵便番号</summary>
		[JsonProperty("zip")]
		public string Zip { get; set; }
		/// <summary>住所</summary>
		[JsonProperty("addr")]
		public string Addr { get; set; }
		/// <summary>電話番号1</summary>
		[JsonProperty("tel1")]
		public string Tel1 { get; set; }
		/// <summary>電話番号2</summary>
		[JsonProperty("tel2")]
		public string Tel2 { get; set; }
		/// <summary>電話番号3</summary>
		[JsonProperty("tel3")]
		public string Tel3 { get; set; }
		/// <summary>ＦＡＸ</summary>
		[JsonProperty("fax")]
		public string Fax { get; set; }
		/// <summary>性別</summary>
		[JsonProperty("sex")]
		public string Sex { get; set; }
		/// <summary>生年月日</summary>
		[JsonProperty("birth")]
		public DateTime? Birth { get; set; }
		/// <summary>企業名</summary>
		[JsonProperty("company_name")]
		public string CompanyName { get; set; }
		/// <summary>部署名</summary>
		[JsonProperty("company_post_name")]
		public string CompanyPostName { get; set; }
		/// <summary>役職名</summary>
		[JsonProperty("company_exective_name")]
		public string CompanyExectiveName { get; set; }
		/// <summary>初回広告コード</summary>
		[JsonProperty("advcode_first")]
		public string AdvcodeFirst { get; set; }
		/// <summary>属性1</summary>
		[JsonProperty("attribute1")]
		public string Attribute1 { get; set; }
		/// <summary>属性2</summary>
		[JsonProperty("attribute2")]
		public string Attribute2 { get; set; }
		/// <summary>属性3</summary>
		[JsonProperty("attribute3")]
		public string Attribute3 { get; set; }
		/// <summary>属性4</summary>
		[JsonProperty("attribute4")]
		public string Attribute4 { get; set; }
		/// <summary>属性5</summary>
		[JsonProperty("attribute5")]
		public string Attribute5 { get; set; }
		/// <summary>属性6</summary>
		[JsonProperty("attribute6")]
		public string Attribute6 { get; set; }
		/// <summary>属性7</summary>
		[JsonProperty("attribute7")]
		public string Attribute7 { get; set; }
		/// <summary>属性8</summary>
		[JsonProperty("attribute8")]
		public string Attribute8 { get; set; }
		/// <summary>属性9</summary>
		[JsonProperty("attribute9")]
		public string Attribute9 { get; set; }
		/// <summary>属性10</summary>
		[JsonProperty("attribute10")]
		public string Attribute10 { get; set; }
		/// <summary>ログインＩＤ</summary>
		[JsonProperty("login_id")]
		public string LoginId { get; set; }
		/// <summary>パスワード</summary>
		[JsonProperty("password")]
		public string Password { get; set; }
		/// <summary>ユーザメモ</summary>
		[JsonProperty("user_memo")]
		public string UserMemo { get; set; }
		/// <summary>キャリアID</summary>
		[JsonProperty("career_id")]
		public string CareerId { get; set; }
		/// <summary>会員ランクID</summary>
		[JsonProperty("member_rank_id")]
		public string MemberRankId { get; set; }
		/// <summary>外部レコメンド連携用ユーザID</summary>
		[JsonProperty("recommend_uid")]
		public string RecommendUid { get; set; }
		/// <summary>定期会員フラグ</summary>
		[JsonProperty("fixed_purchase_member_flg")]
		public string FixedPurchaseMemberFlg { get; set; }
		/// <summary>リアルタイム購入回数（注文基準）</summary>
		[JsonProperty("order_count_order_realtime")]
		public int OrderCountOrderRealtime { get; set; }
		/// <summary>過去累計購入回数</summary>
		[JsonProperty("order_count_old")]
		public int OrderCountOld { get; set; }
	}

	/// <summary>
	/// User Attribute
	/// </summary>
	[Serializable]
	public class UserAttribute
	{
		/// <summary>初回購入日</summary>
		[JsonProperty("first_order_date")]
		public DateTime? FirstOrderDate { get; set; }
		/// <summary>２回目購入日</summary>
		[JsonProperty("second_order_date")]
		public DateTime? SecondOrderDate { get; set; }
		/// <summary>最終購入日</summary>
		[JsonProperty("last_order_date")]
		public DateTime? LastOrderDate { get; set; }
		/// <summary>CPMクラスタ属性1</summary>
		[JsonProperty("cpm_cluster_attribute1")]
		public string CpmClusterAttribute1 { get; set; }
		/// <summary>CPMクラスタ属性2</summary>
		[JsonProperty("cpm_cluster_attribute2")]
		public string CpmClusterAttribute2 { get; set; }
		/// <summary>以前のCPMクラスタ属性1</summary>
		[JsonProperty("cpm_cluster_attribute1_before")]
		public string CpmClusterAttribute1Before { get; set; }
		/// <summary>以前のCPMクラスタ属性2</summary>
		[JsonProperty("cpm_cluster_attribute2_before")]
		public string CpmClusterAttribute2Before { get; set; }
	}

	/// <summary>
	/// User Extend Setting
	/// </summary>
	[Serializable]
	public class UserExtendSetting
	{
		/// <summary>ユーザ拡張項目ID</summary>
		[JsonProperty("setting_id")]
		public string SettingId { get; set; }
		/// <summary>名称</summary>
		[JsonProperty("setting_name")]
		public string SettingName { get; set; }
		/// <summary>ユーザ拡張項目概要表示区分</summary>
		[JsonProperty("outline_kbn")]
		public string OutlineKbn { get; set; }
		/// <summary>ユーザ拡張項目概要</summary>
		[JsonProperty("outline")]
		public string Outline { get; set; }
	}

	/// <summary>
	/// User Credit Card
	/// </summary>
	[Serializable]
	public class UserCreditCard
	{
		/// <summary>カード枝番</summary>
		[JsonProperty("branch_no")]
		public int BranchNo { get; set; }
		/// <summary>連携ID</summary>
		[JsonProperty("cooperation_id")]
		public string CooperationId { get; set; }
		/// <summary>カード表示名</summary>
		[JsonProperty("card_disp_name")]
		public string CardDispName { get; set; }
		/// <summary>カード番号下４桁</summary>
		[JsonProperty("last_four_digit")]
		public string LastFourDigit { get; set; }
		/// <summary>有効期限（月）</summary>
		[JsonProperty("expiration_month")]
		public string ExpirationMonth { get; set; }
		/// <summary>有効期限（年）</summary>
		[JsonProperty("expiration_year")]
		public string ExpirationYear { get; set; }
		/// <summary>カード名義人</summary>
		[JsonProperty("author_name")]
		public string AuthorName { get; set; }
		/// <summary>カード会社コード</summary>
		[JsonProperty("company_code")]
		public string CompanyCode { get; set; }
		/// <summary>連携種別</summary>
		[JsonProperty("cooperation_type")]
		public string CooperationType { get; set; }
	}

	/// <summary>
	/// User Shipping
	/// </summary>
	[Serializable]
	public class UserShipping
	{
		/// <summary>配送先枝番</summary>
		[JsonProperty("shipping_no")]
		
		public int ShippingNo { get; set; }
		/// <summary>配送先名</summary>
		[JsonProperty("name")]
		public string Name { get; set; }
		/// <summary>配送先氏名</summary>
		[JsonProperty("shipping_name")]
		public string ShippingName
		{
			get
			{
				return string.Format(
					"{0}{1}",
					this.ShippingName1,
					this.ShippingName2);
			}
		}
		/// <summary>配送先氏名1</summary>
		[JsonProperty("shipping_name1")]
		public string ShippingName1 { get; set; }
		/// <summary>配送先氏名2</summary>
		[JsonProperty("shipping_name2")]
		public string ShippingName2 { get; set; }
		/// <summary>配送先氏名かな</summary>
		[JsonProperty("shipping_name_kana")]
		public string ShippingNameKana
		{
			get
			{
				return string.Format(
					"{0}{1}",
					this.ShippingNameKana1,
					this.ShippingNameKana2);
			}
		}
		/// <summary>配送先氏名かな1</summary>
		[JsonProperty("shipping_name_kana1")]
		public string ShippingNameKana1 { get; set; }
		/// <summary>配送先氏名かな2</summary>
		[JsonProperty("shipping_name_kana2")]
		public string ShippingNameKana2 { get; set; }
		/// <summary>郵便番号</summary>
		[JsonProperty("shipping_zip")]
		public string ShippingZip { get; set; }
		/// <summary>住所1</summary>
		[JsonProperty("shipping_addr1")]
		public string ShippingAddr1 { get; set; }
		/// <summary>住所2</summary>
		[JsonProperty("shipping_addr2")]
		public string ShippingAddr2 { get; set; }
		/// <summary>住所3</summary>
		[JsonProperty("shipping_addr3")]
		public string ShippingAddr3 { get; set; }
		/// <summary>住所4</summary>
		[JsonProperty("shipping_addr4")]
		public string ShippingAddr4 { get; set; }
		/// <summary>電話番号1</summary>
		[JsonProperty("shipping_tel1")]
		public string ShippingTel1 { get; set; }
		/// <summary>企業名</summary>
		[JsonProperty("shipping_company_name")]
		public string ShippingCompanyName { get; set; }
		/// <summary>部署名</summary>
		[JsonProperty("shipping_company_post_name")]
		public string ShippingCompanyPostName { get; set; }
	}

	/// <summary>
	/// User Default Order Setting
	/// </summary>
	[Serializable]
	public class UserDefaultOrderSetting
	{
		/// <summary>決済種別ID</summary>
		[JsonProperty("payment_id")]
		public string PaymentId { get; set; }
		/// <summary>クレジットカード枝番</summary>
		[JsonProperty("credit_branch_no")]
		public int? CreditBranchNo { get; set; }
		/// <summary>配送先枝番</summary>
		[JsonProperty("user_shipping_no")]
		public int? UserShippingNo { get; set; }
	}

	/// <summary>
	/// User Point
	/// </summary>
	[Serializable]
	public class UserPoint
	{
		/// <summary>ポイント区分</summary>
		[JsonProperty("point_kbn")]
		public string PointKbn { get; set; }
		/// <summary>枝番</summary>
		[JsonProperty("point_kbn_no")]
		public int PointKbnNo { get; set; }
		/// <summary>識別ID</summary>
		[JsonProperty("dept_id")]
		public string DeptId { get; set; }
		/// <summary>ポイントルールID</summary>
		[JsonProperty("point_rule_id")]
		public string PointRuleId { get; set; }
		/// <summary>ポイントルール区分</summary>
		[JsonProperty("point_rule_kbn")]
		public string PointRuleKbn { get; set; }
		/// <summary>ポイント種別</summary>
		[JsonProperty("point_type")]
		public string PointType { get; set; }
		/// <summary>ポイント加算区分</summary>
		[JsonProperty("point_inc_kbn")]
		public string PointIncKbn { get; set; }
		/// <summary>ポイント数</summary>
		[JsonProperty("point")]
		public decimal Point { get; set; }
		/// <summary>有効期限</summary>
		[JsonProperty("point_exp")]
		public DateTime? PointExp { get; set; }
	}

	/// <summary>
	/// User Coupon
	/// </summary>
	[Serializable]
	public class UserCoupon
	{
		/// <summary>識別ID</summary>
		[JsonProperty("dept_id")]
		public string DeptId { get; set; }
		/// <summary>クーポンID</summary>
		[JsonProperty("coupon_id")]
		public string CouponId { get; set; }
		/// <summary>枝番</summary>
		[JsonProperty("coupon_no")]
		public int? CouponNo { get; set; }
		/// <summary>注文ID</summary>
		[JsonProperty("order_id")]
		public string OrderId { get; set; }
		/// <summary>利用フラグ</summary>
		[JsonProperty("use_flg")]
		public string UseFlg { get; set; }
		/// <summary>ユーザークーポン利用可能回数</summary>
		[JsonProperty("user_coupon_count")]
		public int? UserCouponCount { get; set; }
	}
}
