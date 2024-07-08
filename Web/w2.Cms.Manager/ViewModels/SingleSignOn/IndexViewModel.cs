/*
=========================================================================================================
  Module      : インデックスビューモデル(IndexViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.Domain.ShopOperator;

namespace w2.Cms.Manager.ViewModels.SingleSignOn
{
	/// <summary>
	/// インデックスビューモデル
	/// </summary>
	public class IndexViewModel
	{
		/// <summary>オペレータ</summary>
		public ShopOperatorModel ShopOperator { get; set; }
		/// <summary>ログインURL</summary>
		public string LoginUrl { get; set; }
		/// <summary>遷移先URL</summary>
		public string NextUrl { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}
}