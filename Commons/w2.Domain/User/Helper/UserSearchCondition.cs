/*
=========================================================================================================
  Module      : ユーザー検索条件クラス (UserSearchCondition.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.User.Helper
{
	/// <summary>
	/// ユーザー検索条件クラス
	/// </summary>
	[Serializable]
	public class UserSearchCondition : BaseDbMapModel
	{
		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */
		#region プロパティ
		/// <summary>
		/// ユーザーID
		/// </summary>
		[DbMapName("user_id_like_escaped")]
		public string UserIdLikeEscaped { get; set; }

		/// <summary>
		/// ログインID
		/// </summary>
		[DbMapName("login_id_like_escaped")]
		public string LoginIdLikeEscaped { get; set; }

		/// <summary>
		/// モールID
		/// </summary>
		[DbMapName("mall_id")]
		public string MallId { get; set; }

		/// <summary>
		/// 顧客区分
		/// PC_USER：ＰＣ会員
		/// PC_GEST：ＰＣゲスト
		/// SP_USER：スマフォ会員
		/// SP_GEST：スマフォゲスト
		/// MB_USER：モバイル会員
		/// MB_GEST：モバイルゲスト
		/// OFF_USER：オフライン会員
		/// OFF_GEST：オフラインゲスト
		/// MAIL：メルマガ会員
		/// CS：CS
		/// </summary>
		[DbMapName("user_kbn")]
		public string UserKbn { get; set; }

		/// <summary>
		/// かんたん会員フラグ
		/// </summary>
		[DbMapName("easy_register_flg")]
		public string EasyRegisterFlg { get; set; }

		/// <summary>
		/// 氏名
		/// </summary>
		[DbMapName("name_like_escaped")]
		public string NameLikeEscaped { get; set; }

		/// <summary>
		/// 氏名かな
		/// </summary>
		[DbMapName("name_kana_like_escaped")]
		public string NameKanaLikeEscaped { get; set; }

		/// <summary>
		/// メールアドレス
		/// </summary>
		[DbMapName("mail_addr_like_escaped")]
		public string MailAddrLikeEscaped { get; set; }

		/// <summary>
		/// 電話番号1
		/// </summary>
		[DbMapName("tel1_like_escaped")]
		public string Tel1LikeEscaped { get; set; }

		/// <summary>
		/// 郵便番号
		/// </summary>
		[DbMapName("zip_like_escaped")]
		public string ZipLikeEscaped { get; set; }

		/// <summary>
		/// 住所
		/// </summary>
		[DbMapName("addr_like_escaped")]
		public string AddrLikeEscaped { get; set; }

		/// <summary>
		/// 企業名
		/// </summary>
		[DbMapName("company_name_like_escaped")]
		public string CompanyNameLikeEscaped { get; set; }

		/// <summary>
		/// 部署名
		/// </summary>
		[DbMapName("company_post_name_like_escaped")]
		public string CompanyPostNameLikeEscaped { get; set; }

		/// <summary>
		/// メール配信フラグ
		/// </summary>
		[DbMapName("mail_flg")]
		public string MailFlag { get; set; }

		/// <summary>
		/// 削除フラグ
		/// </summary>
		[DbMapName("del_flg")]
		public string DelFlg { get; set; }

		/// <summary>
		/// ユーザー管理レベルID
		/// </summary>
		[DbMapName("user_management_level_id")]
		public string UserManagementLevelId { get; set; }

		/// <summary>
		/// ユーザー管理レベルID除外
		/// </summary>
		[DbMapName("user_management_level_exclude")]
		public string UserManagementLevelExclude { get; set; }

		/// <summary>
		/// 定期会員フラグ
		/// </summary>
		[DbMapName("fixed_purchase_member_flg")]
		public string FixedPurchaseMemberFlg { get; set; }

		/// <summary>
		/// Amazonログイン連携フラグ
		/// </summary>
		[DbMapName("amazon_cooperated_flg")]
		public string AmazonCooperatedFlg { get; set; }

		/// <summary>
		/// AmazonユーザーID
		/// </summary>
		[DbMapName("amazon_user_id")]
		public string AmazonUserid { get; set; }

		/// <summary>
		/// ユーザー拡張項目カラム名(外部連携用)
		/// </summary>
		[DbMapName("external_cooperation_userextend_column_name")]
		public string ExternalCooperationUserExtendColumnName { get; set; }

		/// <summary>
		/// 作成日開始
		/// </summary>
		[DbMapName("date_created_from")]
		public DateTime? DateCreatedFrom { get; set; }

		/// <summary>
		/// 作成日終了
		/// </summary>
		[DbMapName("date_created_to")]
		public DateTime? DateCreatedTo { get; set; }

		/// <summary>
		/// 更新日開始
		/// </summary>
		[DbMapName("date_changed_from")]
		public DateTime? DateChangedFrom { get; set; }

		/// <summary>
		/// 更新日終了
		/// </summary>
		[DbMapName("date_changed_to")]
		public DateTime? DateChangedTo { get; set; }

		/// <summary>
		/// ユーザー統合フラグ
		/// </summary>
		[DbMapName("integrated_flg")]
		public string IntegratedFlg { get; set; }

		/// <summary>
		/// ユーザーメモフラグ
		/// 1：あり
		/// 0：なし
		/// </summary>
		[DbMapName("user_memo")]
		public string UserMemo { get; set; }

		/// <summary>
		/// ユーザーメモ
		/// </summary>
		[DbMapName("user_memo_like_escaped")]
		public string UserMemoLikeEscaped { get; set; }

		/// <summary>
		/// ユーザー拡張項目
		/// </summary>
		[DbMapName("user_extend_Name")]
		public string UserExtendName { get; set; }

		/// <summary>
		/// ユーザー拡張項目ありなしフラグ
		/// </summary>
		[DbMapName("user_extend_flg")]
		public string UserExtendFlg { get; set; }

		/// <summary>
		/// ユーザー拡張項目タイプ
		/// </summary>
		[DbMapName("user_extend_type")]
		public string UserExtendType { get; set; }

		/// <summary>
		/// ユーザー拡張項目テキスト
		/// </summary>
		[DbMapName("user_extend_like_escaped")]
		public string UserExtendLikeEscaped { get; set; }

		/// <summary>
		/// グローバル対応:アクセス国ISOコード
		/// </summary>
		[DbMapName("access_country_iso_code")]
		public string AccessCountoryIsoCode { get; set; }

		/// <summary>
		/// 並び順区分
		/// 0：氏名/昇順
		/// 1：氏名/降順
		/// 2：氏名(かな)/昇順
		/// 3：氏名(かな)/降順
		/// 4：作成日/昇順
		/// 5：作成日/降順
		/// 6：更新日/昇順
		/// 7：更新日/降順
		/// 8：ユーザID/昇順
		/// 9：ユーザID/降順
		/// </summary>
		[DbMapName("sort_kbn")]
		public int SortKbn { get; set; }

		/// <summary>
		/// 開始行番号
		/// </summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }

		/// <summary>
		/// 終了行番号
		/// </summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
		#endregion
	}
}
