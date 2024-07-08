/*
=========================================================================================================
  Module      : クーポン利用ユーザー一覧検索クラス (CouponUseUserListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Coupon.Helper
{
	/// <summary>
	/// クーポン利用ユーザー一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class CouponUseUserListSearchCondition
		: BaseDbMapModel
	{
		/// <summary>クーポンID</summary>
		[DbMapName("coupon_id")]
		public string CouponId { get; set; }
		/// <summary>ユーザーID</summary>
		[DbMapName("user_id")]
		public string UserId { get; set; }
		/// <summary>ユーザーID(SQL LIKEエスケープ済)</summary>
		[DbMapName("user_id_like_escaped")]
		public string UserIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.UserId); }
		}
		/// <summary>氏名</summary>
		[DbMapName("name")]
		public string UserName { get; set; }
		/// <summary>氏名（SQL LIKEエスケープ済）</summary>
		[DbMapName("name_like_escaped")]
		public string UserNameLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.UserName); }
		}
		/// <summary>メールアドレス</summary>
		[DbMapName("mail_addr")]
		public string MailAddress { get; set; }
		/// <summary>
		/// メールアドレス（SQL LIKEエスケープ済）
		/// </summary>
		[DbMapName("mail_addr_like_escaped")]
		public string MailAddressLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.MailAddress); }
		}
		/// <summary>削除フラグ</summary>
		[DbMapName("del_flg")]
		public string DelFlg { get; set; }
		/// <summary>作成日From</summary>
		[DbMapName("date_created_from")]
		public DateTime? DateCreatedFrom { get; set; }
		/// <summary>作成日To</summary>
		[DbMapName("date_created_to")]
		public DateTime? DateCreatedTo { get; set; }
		/// <summary>開始行番号</summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }
		/// <summary>終了行番号</summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
		/// <summary>クーポン利用済みユーザー判定方法</summary>
		[DbMapName(Constants.FLG_COUPONUSEUSER_USED_USER_JUDGE_TYPE)]
		public string UsedUserJudgeType { get; set; }
	}

	/// <summary>
	/// クーポン利用ユーザー一覧検索結果クラス
	/// </summary>
	[Serializable]
	public class CouponUseUserListSearchResult
		: CouponUseUserModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CouponUseUserListSearchResult(DataRowView drv)
			: base(drv)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>クーポン名</summary>
		public string CouponName
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_NAME]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_NAME] = value; }
		}
		/// <summary>ユーザーID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USER_USER_ID] = value; }
		}
		/// <summary>メールアドレス</summary>
		public string MailAddress
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_MAIL_ADDR]; }
			set { this.DataSource[Constants.FIELD_USER_MAIL_ADDR] = value; }
		}
		/// <summary>氏名</summary>
		public string UserName
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME]; }
			set { this.DataSource[Constants.FIELD_USER_NAME] = value; }
		}
		/// <summary>顧客区分</summary>
		public string UserKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_USER_KBN]; }
			set { this.DataSource[Constants.FIELD_USER_USER_KBN] = value; }
		}
		#endregion
	}
}
