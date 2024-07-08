/*
=========================================================================================================
  Module      : 商品一覧ビューモデル (ProductInputViewModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using w2.App.Common;
using w2.Cms.Manager.ViewModels.Shared;
using Validator = w2.Cms.Manager.Codes.Common.Validator;

namespace w2.Cms.Manager.ViewModels.FeaturePage
{
	/// <summary>
	/// 商品一覧ビューモデル
	/// </summary>
	public class ProductInputViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductInputViewModel()
		{
			this.BaseName = string.Empty;
			this.Title = string.Empty;
			this.Pagination = "1";
			this.GroupId = string.Empty;
			this.GroupName = string.Empty;
		}

		/// <summary>
		/// バリデーション
		/// </summary>
		/// <returns>結果</returns>
		public string Validate()
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_FEATUREPAGECONTENTS_DISPLAY_NUMBER, this.DispNum.ToString() },
			};

			var errorMessage = Validator.Validate("FeaturePageProductInput", input);
			return errorMessage;
		}

		/// <summary> 利用元のパス </summary>
		public string BaseName { get; set; }
		/// <summary>商品リスト</summary>
		public ProductViewModel[] ProductList { get; set; }
		/// <summary>商品一覧タイトル</summary>
		public string Title { get; set; }
		/// <summary>表示件数</summary>
		public int DispNum { get; set; }
		/// <summary>ページ送り</summary>
		public string Pagination { get; set; }
		/// <summary>グループID</summary>
		public string GroupId { get; set; }
		/// <summary>グループ名</summary>
		public string GroupName { get; set; }
	}
}