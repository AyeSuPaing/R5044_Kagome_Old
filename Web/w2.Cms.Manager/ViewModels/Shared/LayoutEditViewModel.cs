/*
=========================================================================================================
  Module      : レイアウト編集 ビューモデル(LayoutEditViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.PartsDesign;

namespace w2.Cms.Manager.ViewModels.Shared
{
	/// <summary>
	/// レイアウト編集 ビューモデル
	/// </summary>
	public class LayoutEditViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="layoutEditInputBaseName">呼び出し元でのプロパティ名</param>
		/// <param name="isFeaturePage">特集ページか</param>
		public LayoutEditViewModel(string layoutEditInputBaseName, bool isFeaturePage = false)
		{
			this.LayoutEditInputBaseName = layoutEditInputBaseName;
			this.IsCustomPage = true;
			this.UseDefaultMaster = true;
			this.Input = new LayoutEditInput(isFeaturePage);
			this.PartsParamModel = new PartsDesignListSearchParamModel();
			this.IsFeaturePage = isFeaturePage;
		}

		/// <summary>呼び出し元でのプロパティ名</summary>
		public string LayoutEditInputBaseName { get; private set; }
		/// <summary>編集対象のページはカスタムか?</summary>
		public bool IsCustomPage { get; set; }
		/// <summary>デフォルトのマスターページを利用してるか？</summary>
		public bool UseDefaultMaster { get; set; }
		/// <summary>シンプルレイアウトは利用可能か？</summary>
		public bool CanUseSimpleLayout
		{
			get { return (this.IsCustomPage && this.UseDefaultMaster); }
		}
		/// <summary>レイアウト編集 入力内容</summary>
		public LayoutEditInput Input { get; set; }
		/// <summary>パーツ検索パラメータ</summary>
		public PartsDesignListSearchParamModel PartsParamModel { get; set; }
		/// <summary>特集ページか</summary>
		public bool IsFeaturePage { get; set; }
	}
}