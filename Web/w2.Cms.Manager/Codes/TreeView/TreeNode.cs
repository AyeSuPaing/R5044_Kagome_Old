/*
=========================================================================================================
  Module      : ツリーノード(TreeNode.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;

namespace w2.Cms.Manager.Codes.TreeView
{
	/// <summary>
	/// ツリーノード
	/// </summary>
	public class TreeNode
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TreeNode()
		{
			this.ID = 0;
			this.Childs = new List<TreeNode>();
			this.ErrorMessage = string.Empty;
		}
		/// <summary>ID</summary>
		public int ID { get; set; }
		/// <summary>表示テキスト</summary>
		public string Text { get; set; }
		/// <summary>値</summary>
		public string Value { get; set; }
		/// <summary>選択中か</summary>
		public bool IsSelected { get; set; }
		/// <summary>ディレクトリか</summary>
		public bool IsDir { get; set; }
		/// <summary>展開しているか</summary>
		public bool IsOpen { get; set; }
		/// <summary>表示画像</summary>
		public string ImageUrl { get; set; }
		/// <summary>親ID</summary>
		public int? Pid { get; set; }
		/// <summary>親</summary>
		public virtual TreeNode Parent { get; set; }
		/// <summary>子ノード</summary>
		public virtual List<TreeNode> Childs { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}
}