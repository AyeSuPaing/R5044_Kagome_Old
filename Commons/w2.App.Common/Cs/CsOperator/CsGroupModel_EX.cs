/*
=========================================================================================================
  Module      : CSグループモデルのパーシャルクラス(CsGroupModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Util;

namespace w2.App.Common.Cs.CsOperator
{
	/// <summary>
	/// CSグループモデルのパーシャルクラス
	/// </summary>
	public partial class CsGroupModel : ModelBase<CsGroupModel>
	{
		/// <summary>所属しているオペレータ群</summary>
		public CsOperatorModel[] Ex_Operators { get; set; }

		/// <summary>有効フラグ表示文字列</summary>
		public string EX_ValidFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSGROUP, Constants.FIELD_CSGROUP_VALID_FLG, this.DataSource[Constants.FIELD_CSGROUP_VALID_FLG]);
			}
		}
	}
}
