/*
=========================================================================================================
  Module      : ポイントルール日付入力情報(PointRuleDateInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Input;
using w2.Domain.Point;

namespace Input.Point
{
	/// <summary>
	/// ポイントルール日付入力情報
	/// </summary>
	[Serializable]
	public class PointRuleDateInput : InputBase<PointRuleDateModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PointRuleDateInput()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public PointRuleDateInput(PointRuleDateModel model)
			: this()
		{
			this.DeptId = model.DeptId;
			this.PointRuleId = model.PointRuleId;
			this.TgtDate = model.TgtDate.ToString();
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override PointRuleDateModel CreateModel()
		{
			var model = new PointRuleDateModel
				{
					DeptId = this.DeptId,
					PointRuleId = this.PointRuleId,
					TgtDate = DateTime.Parse(this.TgtDate),
				};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate()
		{
			return "";
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULEDATE_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_POINTRULEDATE_DEPT_ID] = value; }
		}
		/// <summary>ポイントルールID</summary>
		public string PointRuleId
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULEDATE_POINT_RULE_ID]; }
			set { this.DataSource[Constants.FIELD_POINTRULEDATE_POINT_RULE_ID] = value; }
		}
		/// <summary>対象日付</summary>
		public string TgtDate
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULEDATE_TGT_DATE]; }
			set { this.DataSource[Constants.FIELD_POINTRULEDATE_TGT_DATE] = value; }
		}
		#endregion
	}
}