/*
=========================================================================================================
  Module      : 定期購入一覧検索のためのヘルパクラス (FixedPurchaseListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.FixedPurchase.Helper
{
	#region +定期購入一覧検索条件クラス
	/// <summary>
	/// 定期購入一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class FixedPurchaseListSearchCondition : BaseDbMapModel
	{
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId { get; set; }
		/// <summary>定期購入ID（SQL LIKEエスケープ済）</summary>
		[DbMapName("fixed_purchase_id_like_escaped")]
		public string FixedPurchaseIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.FixedPurchaseId); }
		}
		/// <summary>定期購入ステータス</summary>
		[DbMapName("fixed_purchase_status")]
		public string FixedPurchaseStatus { get; set; }
		/// <summary>ユーザID</summary>
		public string UserId { get; set; }
		/// <summary>ユーザID（SQL LIKEエスケープ済）</summary>
		[DbMapName("user_id_like_escaped")]
		public string UserIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.UserId); }
		}
		/// <summary>注文者名</summary>
		public string Name { get; set; }
		/// <summary>注文者名（SQL LIKEエスケープ済）</summary>
		[DbMapName("name_like_escaped")]
		public string NameLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.Name); }
		}
		/// <summary>購入回数(注文基準)（開始）</summary>
		[DbMapName("order_count_from")]
		public int? OrderCountFrom { get; set; }
		/// <summary>購入回数(注文基準)（終了）</summary>
		[DbMapName("order_count_to")]
		public int? OrderCountTo { get; set; }
		/// <summary>購入回数(出荷基準)（開始）</summary>
		[DbMapName("shipped_count_from")]
		public int? ShippedCountFrom { get; set; }
		/// <summary>購入回数(出荷基準)（終了）</summary>
		[DbMapName("shipped_count_to")]
		public int? ShippedCountTo { get; set; }
		/// <summary>注文区分</summary>
		[DbMapName("order_kbn")]
		public string OrderKbn { get; set; }
		/// <summary>決済種別</summary>
		[DbMapName("order_payment_kbn")]
		public string OrderPaymentKbn { get; set; }
		/// <summary>決済ステータス</summary>
		[DbMapName("payment_status")]
		public string PaymentStatus { get; set; }
		/// <summary>定期購入区分</summary>
		[DbMapName("fixed_purchase_kbn")]
		public string FixedPurchaseKbn { get; set; }
		/// <summary>配送先</summary>
		[DbMapName("shipping_compare_kbn")]
		public string ShippingCompareKbn { get; set; }
		/// <summary>商品ID</summary>
		public string VariationId { get; set; }
		/// <summary>商品ID（SQL LIKEエスケープ済）</summary>
		[DbMapName("variation_id_like_escaped")]
		public string VariationIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.VariationId); }
		}
		/// <summary>管理メモフラグ（ブランク or あり or なし）</summary>
		[DbMapName("management_memo_flg")]
		public string ManagementMemoFlg { get; set; }
		/// <summary>管理メモ</summary>
		public string ManagementMemo { get; set; }
		/// <summary>管理メモ（SQL LIKEエスケープ済）</summary>
		[DbMapName("management_memo_like_escaped")]
		public string ManagementMemoLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.ManagementMemo); }
		}
		/// <summary>配送メモフラグ（ブランク or あり or なし）</summary>
		[DbMapName("shipping_memo_flg")]
		public string ShippingMemoFlg { get; set; }
		/// <summary>配送メモ</summary>
		public string ShippingMemo { get; set; }
		/// <summary>配送メモ（SQL LIKEエスケープ済）</summary>
		[DbMapName("shipping_memo_like_escaped")]
		public string ShippingMemoLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.ShippingMemo); }
		}
		/// <summary>日付</summary>
		[DbMapName("date_type")]
		public string DateType { get; set; }
		/// <summary>開始日付（年）</summary>
		[DbMapName("date_from")]
		public DateTime? DateFrom { get; set; }
		/// <summary>開始日付（月）</summary>
		[DbMapName("date_to")]
		public DateTime? DateTo { get; set; }
		/// <summary>拡張ステータス1</summary>
		[DbMapName("extend_status1")]
		public string ExtendStatus1 { get; set; }
		/// <summary>拡張ステータス2</summary>
		[DbMapName("extend_status2")]
		public string ExtendStatus2 { get; set; }
		/// <summary>拡張ステータス3</summary>
		[DbMapName("extend_status3")]
		public string ExtendStatus3 { get; set; }
		/// <summary>拡張ステータス4</summary>
		[DbMapName("extend_status4")]
		public string ExtendStatus4 { get; set; }
		/// <summary>拡張ステータス5</summary>
		[DbMapName("extend_status5")]
		public string ExtendStatus5 { get; set; }
		/// <summary>拡張ステータス6</summary>
		[DbMapName("extend_status6")]
		public string ExtendStatus6 { get; set; }
		/// <summary>拡張ステータス7</summary>
		[DbMapName("extend_status7")]
		public string ExtendStatus7 { get; set; }
		/// <summary>拡張ステータス8</summary>
		[DbMapName("extend_status8")]
		public string ExtendStatus8 { get; set; }
		/// <summary>拡張ステータス9</summary>
		[DbMapName("extend_status9")]
		public string ExtendStatus9 { get; set; }
		/// <summary>拡張ステータス10</summary>
		[DbMapName("extend_status10")]
		public string ExtendStatus10 { get; set; }
		/// <summary>拡張ステータス11</summary>
		[DbMapName("extend_status11")]
		public string ExtendStatus11 { get; set; }
		/// <summary>拡張ステータス12</summary>
		[DbMapName("extend_status12")]
		public string ExtendStatus12 { get; set; }
		/// <summary>拡張ステータス13</summary>
		[DbMapName("extend_status13")]
		public string ExtendStatus13 { get; set; }
		/// <summary>拡張ステータス14</summary>
		[DbMapName("extend_status14")]
		public string ExtendStatus14 { get; set; }
		/// <summary>拡張ステータス15</summary>
		[DbMapName("extend_status15")]
		public string ExtendStatus15 { get; set; }
		/// <summary>拡張ステータス16</summary>
		[DbMapName("extend_status16")]
		public string ExtendStatus16 { get; set; }
		/// <summary>拡張ステータス17</summary>
		[DbMapName("extend_status17")]
		public string ExtendStatus17 { get; set; }
		/// <summary>拡張ステータス18</summary>
		[DbMapName("extend_status18")]
		public string ExtendStatus18 { get; set; }
		/// <summary>拡張ステータス19</summary>
		[DbMapName("extend_status19")]
		public string ExtendStatus19 { get; set; }
		/// <summary>拡張ステータス20</summary>
		[DbMapName("extend_status20")]
		public string ExtendStatus20 { get; set; }
		/// <summary>拡張ステータス21</summary>
		[DbMapName("extend_status21")]
		public string ExtendStatus21 { get; set; }
		/// <summary>拡張ステータス22</summary>
		[DbMapName("extend_status22")]
		public string ExtendStatus22 { get; set; }
		/// <summary>拡張ステータス23</summary>
		[DbMapName("extend_status23")]
		public string ExtendStatus23 { get; set; }
		/// <summary>拡張ステータス24</summary>
		[DbMapName("extend_status24")]
		public string ExtendStatus24 { get; set; }
		/// <summary>拡張ステータス25</summary>
		[DbMapName("extend_status25")]
		public string ExtendStatus25 { get; set; }
		/// <summary>拡張ステータス26</summary>
		[DbMapName("extend_status26")]
		public string ExtendStatus26 { get; set; }
		/// <summary>拡張ステータス27</summary>
		[DbMapName("extend_status27")]
		public string ExtendStatus27 { get; set; }
		/// <summary>拡張ステータス28</summary>
		[DbMapName("extend_status28")]
		public string ExtendStatus28 { get; set; }
		/// <summary>拡張ステータス29</summary>
		[DbMapName("extend_status29")]
		public string ExtendStatus29 { get; set; }
		/// <summary>拡張ステータス30</summary>
		[DbMapName("extend_status30")]
		public string ExtendStatus30 { get; set; }
		/// <summary>拡張ステータス31</summary>
		[DbMapName("extend_status31")]
		public string ExtendStatus31 { get; set; }
		/// <summary>拡張ステータス32</summary>
		[DbMapName("extend_status32")]
		public string ExtendStatus32 { get; set; }
		/// <summary>拡張ステータス33</summary>
		[DbMapName("extend_status33")]
		public string ExtendStatus33 { get; set; }
		/// <summary>拡張ステータス34</summary>
		[DbMapName("extend_status34")]
		public string ExtendStatus34 { get; set; }
		/// <summary>拡張ステータス35</summary>
		[DbMapName("extend_status35")]
		public string ExtendStatus35 { get; set; }
		/// <summary>拡張ステータス36</summary>
		[DbMapName("extend_status36")]
		public string ExtendStatus36 { get; set; }
		/// <summary>拡張ステータス37</summary>
		[DbMapName("extend_status37")]
		public string ExtendStatus37 { get; set; }
		/// <summary>拡張ステータス38</summary>
		[DbMapName("extend_status38")]
		public string ExtendStatus38 { get; set; }
		/// <summary>拡張ステータス39</summary>
		[DbMapName("extend_status39")]
		public string ExtendStatus39 { get; set; }
		/// <summary>拡張ステータス40</summary>
		[DbMapName("extend_status40")]
		public string ExtendStatus40 { get; set; }
		/// <summary>拡張ステータス枝番</summary>
		[DbMapName("extend_status_date_no")]
		public string ExtendStatusDateNo { get; set; }
		/// <summary>>拡張ステータス更新日・開始日付</summary>
		[DbMapName("extend_status_date_from")]
		public DateTime? ExtendStatusDateFrom { get; set; }
		/// <summary>拡張ステータス更新日・終了日付</summary>
		[DbMapName("extend_status_date_to")]
		public DateTime? ExtendStatusDateTo { get; set; }
		/// <summary>
		/// 並び順区分
		/// 0：作成日/昇順
		/// 1：作成日/降順
		/// 2：更新日/昇順
		/// 3：更新日/降順
		/// 4：定期購入ID/昇順
		/// 5：定期購入ID/降順
		/// 6：次回配送日/昇順
		/// 7：次回配送日/降順
		/// 8：次々回配送日/昇順
		/// 9：次々回配送日/降順
		/// 10：定期購入開始日時/昇順
		/// 11：定期購入開始日時/降順
		/// 12：最終購入日/昇順
		/// 13：最終購入日/降順
		/// </summary>
		[DbMapName("sort_kbn")]
		public string SortKbn { get; set; }
		/// <summary>開始行番号</summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }
		/// <summary>終了行番号</summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
		/// <summary>配送方法</summary>
		[DbMapName("shipping_method")]
		public string ShippingMethod { get; set; }
		/// <summary>領収書希望フラグ</summary>
		[DbMapName("receipt_flg")]
		public string ReceiptFlg { get; set; }
		/// <summary>注文拡張項目 項目名</summary>
		[DbMapName("order_extend_name")]
		public string OrderExtendName { get; set; }
		/// <summary>注文拡張項目 項目 ありなし</summary>
		[DbMapName("order_extend_flg")]
		public string OrderExtendFlg { get; set; }
		/// <summary>注文拡張項目 項目 種別</summary>
		[DbMapName("order_extend_type")]
		public string OrderExtendType { get; set; }
		/// <summary>注文拡張項目 内容</summary>
		[DbMapName("order_extend_like_escaped")]
		public string OrderExtendLikeEscaped { get; set; }
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId { get; set; }
		/// <summary>頒布会コースID LIKEエスケープ済</summary>
		[DbMapName(Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_COURSE_ID + "_like_escaped")]
		public string SubscriptionBoxCourseIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.SubscriptionBoxCourseId); }
		}
		/// <summary>頒布会か</summary>
		[DbMapName("is_subscription_box")]
		public string IsSubscriptionBox { get; set; }
		/// <summary>頒布会購入回数FROM</summary>
		[DbMapName("subscription_box_order_count_from")]
		public int? SubscriptionBoxOrderCountFrom { get; set; }
		/// <summary>頒布会購入回数TO</summary>
		[DbMapName("subscription_box_order_count_to")]
		public int? SubscriptionBoxOrderCountTo { get; set; }
	}
	#endregion

	#region +定期購入一覧検索結果クラス
	/// <summary>
	/// 定期購入一覧検索結果クラス
	/// ※FixedPurchaseModelを拡張
	/// </summary>
	[Serializable]
	public class FixedPurchaseListSearchResult : FixedPurchaseModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseListSearchResult(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>注文者名</summary>
		public string UserName
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME]; }
		}
		/// <summary>決済種別名</summary>
		public string PaymentName
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_NAME]; }
		}
		#endregion
	}
	#endregion
}
