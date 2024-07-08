/*
=========================================================================================================
  Module      : ユーザーポイントリスト検索のためのヘルパクラス (UserPointListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Point.Helper
{
	/// <summary>
	/// ユーザーポイントリスト検索のためのサービスクラス
	/// </summary>
	/// <remarks>本クラスはinternalであり、外部への公開はServiceクラスを介す</remarks>
	internal class UserPointListSearch
	{
		#region ~GetCountOfSearchUserPoint ユーザーポイント検索件数取得
		/// <summary>
		/// ユーザーポイント検索件数取得
		/// </summary>
		/// <param name="cond">ユーザーポイント検索条件クラス</param>
		/// <returns>検索件数</returns>
		internal int GetCountOfSearchUserPoint(UserPointSearchCondition cond)
		{
			using (var rep = new PointRepository())
			{
				var count = rep.GetCountOfSearchUserPoint(cond);
				return count;
			}
		}
		#endregion

		#region ~Search ユーザーポイント検索
		/// <summary>
		/// ユーザーポイント検索
		/// </summary>
		/// <param name="cond">ユーザーポイント検索条件クラス</param>
		/// <returns>検索結果</returns>
		internal UserPointSearchResult[] Search(UserPointSearchCondition cond)
		{
			using (var rep = new PointRepository())
			{
				return rep.SearchUserPoint(cond);
			}
		}
		#endregion
	}

	/// <summary>
	/// ユーザーポイント検索条件クラス
	/// </summary>
	[Serializable]
	public class UserPointSearchCondition : BaseDbMapModel
	{
		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */
		#region プロパティ
		/// <summary>
		/// 識別ID
		/// </summary>
		[DbMapName("dept_id")]
		public string DeptId { get; set; }

		/// <summary>
		/// 検索項目
		/// 0：ユーザーID
		/// 1：氏名
		/// 2：フリガナ
		/// 3：電話番号
		/// 4：メールアドレス
		/// 5：郵便番号
		/// 6：住所
		/// 7：企業名
		/// 8：部署名
		/// 99：条件なし
		/// </summary>
		[DbMapName("srch_key")]
		public int SearchKey { get; set; }

		/// <summary>
		/// 検索ワード
		/// </summary>
		[DbMapName("srch_word_like_escaped")]
		public string SearchWordLikeEscaped { get; set; }

		/// <summary>
		/// 並び順区分
		/// 0：ポイント/昇順
		/// 1：ポイント/降順
		/// 2：有効期限/昇順
		/// 3：有効期限/降順
		/// 8：ユーザID/昇順
		/// 9：ユーザID/降順
		/// </summary>
		[DbMapName("sort_kbn")]
		public int SortKbn { get; set; }

		/// <summary>
		/// ポイント区分
		/// </summary>
		[DbMapName("select_point_kbn")]
		public string SelectPointKbn { get; set; }

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

		/// <summary>
		/// ユーザー削除フラグ
		/// </summary>
		[DbMapName("del_flg")]
		public string DelFlg { get; set; }
		#endregion
	}

	/// <summary>
	/// ユーザーポイントの検索結果クラス(DBモデルではない！)
	/// </summary>
	[Serializable]
	public class UserPointSearchResult : ModelBase<UserPointSearchResult>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private UserPointSearchResult()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserPointSearchResult(DataRowView source)
			: this()
		{
			this.DataSource = source.ToHashtable();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserPointSearchResult(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>行番号</summary>
		public int RowNum
		{
			get { return (int)this.DataSource["row_num"]; }
		}
		/// <summary>ユーザーID</summary>
		public string UserId
		{
			get { return (string)this.DataSource["user_id"]; }
		}
		/// <summary>ユーザー名</summary>
		public string Name
		{
			get { return (string)this.DataSource["name"]; }
		}
		/// <summary>ポイント区分</summary>
		public string PointKbn
		{
			get { return (string)this.DataSource["point_kbn"]; }
		}
		/// <summary>ポイント区分枝番</summary>
		public int PointKbnNo
		{
			get { return (int)this.DataSource["point_kbn_no"]; }
		}
		/// <summary>本ポイント</summary>
		public decimal? RealPoint
		{
			get { return (this.DataSource["point"] == DBNull.Value) ? null : (decimal?)this.DataSource["point"]; }
		}
		/// <summary>仮ポイント</summary>
		public decimal? TempPoint
		{
			get { return (this.DataSource["point_temp"] == DBNull.Value) ? null : (decimal?)this.DataSource["point_temp"]; }
		}
		/// <summary> 有効期限</summary>
		public DateTime? PointExp
		{
			get { return (this.DataSource["point_exp"] == DBNull.Value) ? null : (DateTime?)this.DataSource["point_exp"]; }
		}
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (this.DataSource["kbn1"] == DBNull.Value) ? string.Empty : (string)this.DataSource["kbn1"]; }
		}
		#endregion
	}
}
