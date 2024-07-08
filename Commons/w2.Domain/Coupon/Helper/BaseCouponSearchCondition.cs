/*
=========================================================================================================
  Module      : 基礎的なクーポン検索条件情報クラス (BaseCouponSearchCondition.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Coupon.Helper
{
	/// <summary>
	/// 基礎的なクーポン検索条件情報
	/// </summary>
	[Serializable]
	public class BaseCouponSearchCondition : BaseDbMapModel
	{
		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */
		#region プロパティ
		/// <summary>識別ID</summary>
		[DbMapName("dept_id")]
		public string DeptId { get; set; }

		/// <summary>ユーザーID</summary>
		[DbMapName("user_id")]
		public string UserId { get; set; }

		/// <summary>開始行番号</summary>
		[DbMapName("bgn_row_num")]
		public int BgnRowNumber { get; set; }

		/// <summary>終了行番号</summary>
		/// <remarks>ページング時の取得終了行</remarks>
		/// <remarks>PageSizeとは併用しない</remarks>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }

		/// <summary>表示範囲</summary>
		/// <remarks>ページングでOFFSET FETCHを利用する際の表示サイズ</remarks>
		/// <remarks>EndRowNumberとは併用しない</remarks>
		[DbMapName("page_size")]
		public int PageSize { get; set; }

		/// <summary>検索項目</summary>
		[DbMapName("srch_key")]
		public string SearchKey { get; set; }

		/// <summary>検査ワード</summary>
		[DbMapName("srch_word_like_escaped")]
		public string SearchWordLikeEscaped { get; set; }

		/// <summary>並び順区分</summary>
		[DbMapName("sort_kbn")]
		public string SortKbn { get; set; }

		/// <summary>メールアドレス</summary>
		[DbMapName("mail_addr")]
		public string MailAddress { get; set; }

		/// <summary>クーポン利用ユーザー判定条件</summary>
		[DbMapName(Constants.FLG_COUPONUSEUSER_USED_USER_JUDGE_TYPE)]
		public string UsedUserJudgeType { get; set; }
		#endregion
	}
}
