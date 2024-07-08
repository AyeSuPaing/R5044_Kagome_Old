/*
=========================================================================================================
  Module      : 商品グループプレビューモデル (ListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Cms.Manager.ViewModels.ProductGroup
{
	/// <summary>
	/// 商品グループプレビューモデル
	/// </summary>
	public class PreviewViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PreviewViewModel()
		{
			this.PreviewUrl = "";
		}

		/// <summary>プレビューURL</summary>
		public string PreviewUrl { get; set; }
	}
}