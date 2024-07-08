/*
=========================================================================================================
  Module      : 外部リンク設定拡張モデル(CsExternalLinkModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Util;

namespace w2.Domain.ExternalLink
{
	/// <summary>
	/// 外部リンク設定モデルのパーシャルクラス
	/// 自動生成されないものはこちらで定義
	/// </summary>
	public partial class CsExternalLinkModel : ModelBase<CsExternalLinkModel>
	{
		/// <summary>
		/// 拡張項目_総件数
		/// </summary>
		public int SearchCount
		{
			get { return (int)this.DataSource["row_count"]; }
			set { this.DataSource["row_count"] = value; }
		}
	}
}
