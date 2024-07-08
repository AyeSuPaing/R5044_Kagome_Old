/*
=========================================================================================================
  Module      : シングルサインオン実行（何もしない）クラス(DoNothingSingleSignOnExecuter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace w2.App.Common.SingleSignOn.Executer
{
	/// <summary>
	/// シングルサインオン実行（何もしない）クラス
	/// </summary>
	public class DoNothingSingleSignOnExecuter : BaseSingleSignOnExecuter
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DoNothingSingleSignOnExecuter(
			HttpContext context)
			: base(context)
		{
		}
		#endregion

		#region メソッド
		/// <summary>シングルサインオン実行</summary>
		protected override SingleSignOnResult OnExecute()
		{
			// 何もしない
			return new SingleSignOnResult(
				SingleSignOnDetailTypes.None,
				null);
		}
		#endregion

		#region プロパティ
		#endregion
	}
}