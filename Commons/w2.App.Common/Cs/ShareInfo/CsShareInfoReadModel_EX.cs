/*
=========================================================================================================
  Module      : 共有情報既読管理モデルのパーシャルクラス(CsShareInfoReadModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Util;

namespace w2.App.Common.Cs.ShareInfo
{
	/// <summary>
	/// 共有情報既読管理モデルのパーシャルクラス
	/// </summary>
	public partial class CsShareInfoReadModel : ModelBase<CsShareInfoReadModel>
	{
		/// <summary>オペレータ名</summary>
		public string EX_OperatorName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_SHOPOPERATOR_NAME]); }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_NAME] = value; }
		}
	}
}
