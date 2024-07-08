/*
=========================================================================================================
  Module      : 回答例モデルのパーシャルクラス(CsAnswerTemplateModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Util;

namespace w2.App.Common.Cs.AnswerTemplate
{
	/// <summary>
	/// 回答例モデルのパーシャルクラス
	/// 自動生成されないものはこちらで定義
	/// </summary>
	public partial class CsAnswerTemplateModel : ModelBase<CsAnswerTemplateModel>
	{
		/// <summary>
		/// 拡張項目_総件数
		/// </summary>
		public int EX_SearchCount
		{
			get { return (int)this.DataSource["row_count"]; }
			set { this.DataSource["row_count"] = value; }
		}
		/// <summary>
		/// 拡張項目_カテゴリ名
		/// </summary>
		public string EX_CategoryName
		{
			get { return StringUtility.ToEmpty(this.DataSource["category_name"]); }
			set { this.DataSource["category_name"] = value; }
		}
	}
}
