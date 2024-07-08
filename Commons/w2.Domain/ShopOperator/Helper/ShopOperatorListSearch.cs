/*
=========================================================================================================
  Module      : 店舗管理者一覧検索条件クラス (ShopOperatorListSearchCondition.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.ShopOperator.Helper
{
	/// <summary>
	/// 店舗管理者一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class ShopOperatorListSearchCondition : BaseDbMapModel
	{
		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */

		#region プロパティ
		/// <summary>店舗ID</summary>
		[DbMapName("shop_id")]
		public string ShopId { get; set; }
		/// <summary>PKG区分</summary>
		[DbMapName("pkg_kbn")]
		public string PkgKbn { get; set; }
		/// <summary>オペレータID（SQL LIKEエスケープ済） </summary>
		[DbMapName("operator_id_like_escaped")]
		public string OperatorIdLikeEscaped { get { return StringUtility.SqlLikeStringSharpEscape(this.OperatorId); } }
		/// <summary>オペレータID</summary>
		public string OperatorId { get; set; }
		/// <summary>オペレータ名（SQL LIKEエスケープ済） </summary>
		[DbMapName("name_like_escaped")]
		public string OperatorNameIdLikeEscaped { get { return StringUtility.SqlLikeStringSharpEscape(this.OperatorName); } }
		/// <summary>オペレータ名</summary>
		public string OperatorName { get; set; }
		/// <summary>ログインオペレーターメニューアクセスレベルEC</summary>
		[DbMapName("login_operator_menu_access_level1")]
		public int? LoginOperatorMenuAccessLevelEc { get; set; }
		/// <summary>ログインオペレーターメニューアクセスレベルMP</summary>
		[DbMapName("login_operator_menu_access_level2")]
		public int? LoginOperatorMenuAccessLevelMp { get; set; }
		/// <summary>ログインオペレーターメニューアクセスレベルCS</summary>
		[DbMapName("login_operator_menu_access_level3")]
		public int? LoginOperatorMenuAccessLevelCs { get; set; }
		/// <summary>ログインオペレーターメニューアクセスレベルCMS</summary>
		[DbMapName("login_operator_menu_access_level4")]
		public int? LoginOperatorMenuAccessLevelCms { get; set; }
		/// <summary>並び順</summary>
		[DbMapName("sort_kbn")]
		public string SortKbn { get; set; }
		/// <summary>有効フラグ</summary>
		[DbMapName("valid_flg")]
		public string ValidFlg { get; set; }
		/// <summary>メニュー権限</summary>
		[DbMapName("condition_menu_access_level")]
		public string ConditionMenuAccessLevel { get; set; }

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

	/// <summary>
	///店舗管理者一覧検索クラス(DBモデルではない！)
	/// </summary>
	[Serializable]
	public class ShopOperatorListSearchResult : ShopOperatorModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ShopOperatorListSearchResult(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ(Modelに実装している以外）
		/// <summary>メニュー権限名</summary>
		public string MenuAuthorityName
		{
			get
			{
				return (string)this.DataSource[Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_NAME];
			}
			set { this.DataSource[Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_NAME] = value; }
		}
		#endregion
	}
}
