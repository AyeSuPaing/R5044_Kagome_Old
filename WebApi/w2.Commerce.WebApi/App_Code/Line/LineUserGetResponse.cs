/*
=========================================================================================================
  Module      : Line User Get Response (LineUserGetResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

/// <summary>
/// LinePay User Get Response Object
/// </summary>
[Serializable]
public class LineUserGetResponse
{
	/// <summary>User Id</summary>
	[JsonProperty(PropertyName = "user_id")]
	public string UserId { get; set; }
	/// <summary>User Kbn</summary>
	[JsonProperty(PropertyName = "user_kbn")]
	public string UserKbn { get; set; }
	/// <summary>Name</summary>
	[JsonProperty(PropertyName = "name")]
	public string Name { get; set; }
	/// <summary>Name1</summary>
	[JsonProperty(PropertyName = "name1")]
	public string Name1 { get; set; }
	/// <summary>Name2</summary>
	[JsonProperty(PropertyName = "name2")]
	public string Name2 { get; set; }
	/// <summary>Name Kana</summary>
	[JsonProperty(PropertyName = "name_kana")]
	public string NameKana { get; set; }
	/// <summary>Name Kana1</summary>
	[JsonProperty(PropertyName = "name_kana1")]
	public string NameKana1 { get; set; }
	/// <summary>Name Kana2</summary>
	[JsonProperty(PropertyName = "name_kana2")]
	public string NameKana2 { get; set; }
	/// <summary>Mail Addr</summary>
	[JsonProperty(PropertyName = "mail_addr")]
	public string MailAddr { get; set; }
	/// <summary>Mail Addr2</summary>
	[JsonProperty(PropertyName = "mail_addr2")]
	public string MailAddr2 { get; set; }
	/// <summary>Zip</summary>
	[JsonProperty(PropertyName = "zip")]
	public string Zip { get; set; }
	/// <summary>Addr</summary>
	[JsonProperty(PropertyName = "addr")]
	public string Addr
	{
		get { return (this.Addr1 + this.Addr2 + this.Addr3 + this.Addr4); }
	}
	/// <summary>Addr1</summary>
	[JsonProperty(PropertyName = "addr1")]
	public string Addr1 { get; set; }
	/// <summary>Addr2</summary>
	[JsonProperty(PropertyName = "addr2")]
	public string Addr2 { get; set; }
	/// <summary>Addr3</summary>
	[JsonProperty(PropertyName = "addr3")]
	public string Addr3 { get; set; }
	/// <summary>Addr4</summary>
	[JsonProperty(PropertyName = "addr4")]
	public string Addr4 { get; set; }
	/// <summary>Addr5</summary>
	[JsonProperty(PropertyName = "addr5")]
	public string Addr5 { get; set; }
	/// <summary>Tel1</summary>
	[JsonProperty(PropertyName = "tel1")]
	public string Tel1 { get; set; }
	/// <summary>Sex</summary>
	[JsonProperty(PropertyName = "sex")]
	public string Sex { get; set; }
	/// <summary>Birth</summary>
	[JsonProperty(PropertyName = "birth")]
	public string Birth { get; set; }
	/// <summary>Birth Year</summary>
	[JsonProperty(PropertyName = "birth_year")]
	public string BirthYear { get; set; }
	/// <summary>Birth Month</summary>
	[JsonProperty(PropertyName = "birth_month")]
	public string BirthMonth { get; set; }
	/// <summary>Birth Day</summary>
	[JsonProperty(PropertyName = "birth_day")]
	public string BirthDay { get; set; }
	/// <summary>Mail Flg</summary>
	[JsonProperty(PropertyName = "mail_flg")]
	public string MailFlg { get; set; }
	/// <summary>Easy Register Flg</summary>
	[JsonProperty(PropertyName = "easy_register_flg")]
	public string EasyRegisterFlg { get; set; }
	/// <summary>User Memo</summary>
	[JsonProperty(PropertyName = "user_memo")]
	public string UserMemo { get; set; }
	/// <summary>Advcode First</summary>
	[JsonProperty(PropertyName = "advcode_first")]
	public string AdvcodeFirst { get; set; }
	/// <summary>Del Flg</summary>
	[JsonProperty(PropertyName = "del_flg")]
	public string DelFlg { get; set; }
	/// <summary>Member Rank Id</summary>
	[JsonProperty(PropertyName = "member_rank_id")]
	public string MemberRankId { get; set; }
	/// <summary>First Order Date</summary>
	[JsonProperty(PropertyName = "first_order_date")]
	public string FirstOrderDate { get; set; }
	/// <summary>Second Order Date</summary>
	[JsonProperty(PropertyName = "second_order_date")]
	public string SecondOrderDate { get; set; }
	/// <summary>Last Order Date</summary>
	[JsonProperty(PropertyName = "last_order_date")]
	public string LastOrderDate { get; set; }
	/// <summary>Enrollment Days</summary>
	[JsonProperty(PropertyName = "enrollment_days")]
	public string EnrollmentDays { get; set; }
	/// <summary>Away Days</summary>
	[JsonProperty(PropertyName = "away_days")]
	public string AwayDays { get; set; }
	/// <summary>Order Amount Order All</summary>
	[JsonProperty(PropertyName = "order_amount_order_all")]
	public decimal OrderAmountOrderAll { get; set; }
	/// <summary>Order Amount Order Fp</summary>
	[JsonProperty(PropertyName = "order_amount_order_fp")]
	public decimal OrderAmountOrderFp { get; set; }
	/// <summary>Order Count Order All</summary>
	[JsonProperty(PropertyName = "order_count_order_all")]
	public int OrderCountOrderAll { get; set; }
	/// <summary>Order Count Order Fp</summary>
	[JsonProperty(PropertyName = "order_count_order_fp")]
	public int OrderCountOrderFp { get; set; }
	/// <summary>Order Amount Ship All</summary>
	[JsonProperty(PropertyName = "order_amount_ship_all")]
	public decimal OrderAmountShipAll { get; set; }
	/// <summary>Order Amount Ship Fp</summary>
	[JsonProperty(PropertyName = "order_amount_ship_fp")]
	public decimal OrderAmountShipFp { get; set; }
	/// <summary>Order Count Ship All</summary>
	[JsonProperty(PropertyName = "order_count_ship_all")]
	public int OrderCountShipAll { get; set; }
	/// <summary>Order Count Ship Fp</summary>
	[JsonProperty(PropertyName = "order_count_ship_fp")]
	public int OrderCountShipFp { get; set; }
	/// <summary>Attribute Date Changed</summary>
	[JsonProperty(PropertyName = "attribute_date_changed")]
	public string AttributeDateChanged { get; set; }
	/// <summary>Cpm Cluster Attribute 1</summary>
	[JsonProperty(PropertyName = "cpm_cluster_attribute1")]
	public string CpmClusterAttribute1 { get; set; }
	/// <summary>Cpm Cluster Attribute 2</summary>
	[JsonProperty(PropertyName = "cpm_cluster_attribute2")]
	public string CpmClusterAttribute2 { get; set; }
	/// <summary>Fixed Purchase Member Flg</summary>
	[JsonProperty(PropertyName = "fixed_purchase_member_flg")]
	public string FixedPurchaseMemberFlg { get; set; }
	/// <summary>Order Count Order Realtime</summary>
	[JsonProperty(PropertyName = "order_count_order_realtime")]
	public int OrderCountOrderRealtime { get; set; }
	/// <summary>Order Count Old</summary>
	[JsonProperty(PropertyName = "order_count_old")]
	public int OrderCountOld { get; set; }
	/// <summary>Line User Id</summary>
	[JsonProperty(PropertyName = "line_user_id")]
	public string LineUserId { get; set; }
	/// <summary>Date Created</summary>
	[JsonProperty(PropertyName = "date_created")]
	public string DateCreated { get; set; }
	/// <summary>Date Changed</summary>
	[JsonProperty(PropertyName = "date_changed")]
	public string DateChanged { get; set; }
	/// <summary>Status</summary>
	[JsonProperty(PropertyName = "status")]
	public int Status { get; set; }
	/// <summary>統合先ユーザーId</summary>
	[JsonProperty(PropertyName = "integrated_user_id")]
	public string IntegratedUserId { get; set; }
}