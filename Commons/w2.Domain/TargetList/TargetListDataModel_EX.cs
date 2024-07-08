/*
=========================================================================================================
  Module      : ターゲットリストデータモデル (TargetListDataModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.TargetList
{
	/// <summary>
	/// ターゲットリストデータモデル
	/// </summary>
	public partial class TargetListDataModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>表示言語コード</summary>
		public string DispLanguageCode
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_DISP_LANGUAGE_CODE]; }
			set { this.DataSource[Constants.FIELD_USER_DISP_LANGUAGE_CODE] = value; }
		}
		/// <summary>表示言語ロケールID</summary>
		public string DispLanguageLocaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID]; }
			set { this.DataSource[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID] = value; }
		}
		#endregion
	}
}
