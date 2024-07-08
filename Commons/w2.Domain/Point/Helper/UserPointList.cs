/*
=========================================================================================================
  Module      : ポイント操作のヘルパクラス (PointOperation.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Util;

namespace w2.Domain.Point.Helper
{
	/// <summary>
	/// ユーザポイント情報ヘルパクラス
	/// </summary>
	[Serializable]
	public class UserPointList
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserPointList()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public UserPointList(UserPointModel[] model)
			: this()
		{
			this.Items = model.ToList();
		}
		#endregion

		#region メソッド
		/// <summary>
		/// ユーザポイント取得
		/// </summary>
		/// <param name="pointKbnNo">ポイント区分No</param>
		/// <returns>ユーザポイント情報</returns>
		public UserPointModel GetUserPointByPointKbnNo(int pointKbnNo)
		{
			var point = this.Items.FirstOrDefault(p => p.PointKbnNo == pointKbnNo);
			return point;
		}
		/// <summary>
		/// ユーザポイント取得
		/// </summary>
		/// <param name="index">インデックス</param>
		/// <returns>ユーザポイント情報</returns>
		public UserPointModel GetUserPoint(int index)
		{
			var point = (this.Items.Count > index)
				? this.Items[index]
				: null;
			return point;
		}

		/// <summary>
		/// 注文関連ポイント情報取得(通常本ポイントだけ除く)
		/// </summary>
		/// <returns>ユーザポイント情報</returns>
		public UserPointList GetOrderPoint()
		{
			var pointList = ObjectUtility.DeepCopy(this);
			pointList.Items = this.Items.Where(x => ((x.IsPointTypeComp && x.IsBasePoint) == false)).ToList();
			return pointList;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザポイントリスト</summary>
		public List<UserPointModel> Items { get; private set; }
		/// <summary>ユーザ本ポイント</summary>
		public UserPointModel[] UserPointComp
		{
			get { return this.Items.Where(point => point.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP).ToArray(); }
		}
		/// <summary>ユーザ仮ポイントリスト</summary>
		public UserPointModel[] UserPointTemp
		{
			get { return this.Items.Where(point => point.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP).ToArray(); }
		}
		#endregion
	}
}
