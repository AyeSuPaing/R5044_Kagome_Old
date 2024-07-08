/*
=========================================================================================================
  Module      : プログラムファイルデザイン設定ビューモデル(CSSDesignViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using w2.Cms.Manager.Codes.TreeView;

namespace w2.Cms.Manager.ViewModels.ProgramFileDesignSetting
{
	/// <summary>
	/// プログラムファイルデザイン設定ビューモデル
	/// </summary>
	public class ProgramFileDesignSettingViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="treeRootNode">ツリールートノード</param>
		public ProgramFileDesignSettingViewModel(IEnumerable<TreeNode> treeRootNode)
		{
			this.Root = treeRootNode;
		}

		/// <summary>ルートノード</summary>
		public IEnumerable<TreeNode> Root { get; private set; }
	}
}