/*
=========================================================================================================
  Module      : コンテンツビューモデル(ContentsViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.TreeView;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.ViewModels.ContentsManager
{
	/// <summary>
	/// コンテンツビューモデル
	/// </summary>
	public class ContentsViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ContentsViewModel()
		{
			this.Root = new ContentsManagerWorkerService().CreateTreeView(new []{""});
			this.Input = new ContentsManagerInput();
			this.ShortCut = Constants.CONTENTSMANAGER_CONTENTS_SHORTCUT_LIST;
		}

		/// <summary>ルートノード</summary>
		public IEnumerable<TreeNode> Root { get; set; }
		/// <summary>コンテンツマネージャー入力</summary>
		public ContentsManagerInput Input { get; set; }
		/// <summary>ショートカット</summary>
		public List<KeyValuePair<string, string>> ShortCut { get; set; }
	}
}