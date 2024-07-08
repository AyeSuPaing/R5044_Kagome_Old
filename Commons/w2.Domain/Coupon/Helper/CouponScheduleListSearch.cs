/*
=========================================================================================================
  Module      : クーポン発行スケジュールリスト検索のためのヘルパクラス (CouponScheduleListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Coupon.Helper
{
	/// <summary>
	/// クーポン発行スケジュールリスト検索のためのヘルパクラス
	/// </summary>
	/// <remarks>本クラスはinternalであり、外部への公開はServiceクラスを介す</remarks>
	internal class CouponScheduleListSearch
	{
		#region ~CouponScheduleListSearchResult クーポン発行スケジュールリスト検索
		/// <summary>
		/// クーポン発行スケジュールリスト検索
		/// </summary>
		/// <param name="cond">クーポン発行スケジュールリスト検索条件クラス</param>
		/// <returns>検索結果</returns>
		internal CouponScheduleListSearchResult[] Search(CouponScheduleListSearchCondition cond)
		{
			using (var rep = new CouponRepository())
			{
				return rep.SearchCouponScheduleList(cond);
			}
		}
		#endregion
	}

	/// <summary>
	/// クーポン発行スケジュールリスト検索条件クラス
	/// </summary>
	[Serializable]
	public class CouponScheduleListSearchCondition : BaseDbMapModel
	{
		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */
		#region プロパティ
		/// <summary>
		/// クーポン発行スケジュールID
		/// </summary>
		[DbMapName("coupon_schedule_id")]
		public string CouponScheduleId { get; set; }

		/// <summary>
		/// クーポン発行スケジュール名
		/// </summary>
		[DbMapName("coupon_schedule_name")]
		public string CouponScheduleName { get; set; }

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
	/// クーポン発行スケジュールリスト検索結果クラス(CouponScheduleModelを拡張)
	/// </summary>
	[Serializable]
	public class CouponScheduleListSearchResult : CouponScheduleModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private CouponScheduleListSearchResult()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CouponScheduleListSearchResult(CouponModel model)
			: this(model.DataSource)
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CouponScheduleListSearchResult(DataRowView source)
			: this()
		{
			this.DataSource = source.ToHashtable();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CouponScheduleListSearchResult(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>検索結果の総合計行数</summary>
		public int RowCount
		{
			get { return (int)this.DataSource["row_count"]; }
			set { this.DataSource["row_count"] = value; }
		}
		#endregion
	}
}
