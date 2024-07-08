/*
=========================================================================================================
  Module      : ビューモデル基底クラス(ViewModelBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Cms.Manager.Codes;

namespace w2.Cms.Manager.ViewModels
{
	/// <summary>
	/// ビューモデル基底クラス
	/// </summary>
	[Serializable]
	public class ViewModelBase
	{
		/// <summary>一覧ステータスか</summary>
		public bool IsActionStatusList { get { return (this.ActionStatus == ActionStatus.List); } }
		/// <summary>確認ステータスか</summary>
		public bool IsActionStatusConfirm { get { return (this.ActionStatus == ActionStatus.Confirm); } }
		/// <summary>登録ステータスか</summary>
		public bool IsActionStatusInsert { get { return (this.ActionStatus == ActionStatus.Insert); } }
		/// <summary>更新ステータスか</summary>
		public bool IsActionStatusUpdate { get { return (this.ActionStatus == ActionStatus.Update); } }
		/// <summary>詳細ステータスか</summary>
		public bool IsActionStatusDetail { get { return (this.ActionStatus == ActionStatus.Detail); } }
		/// <summary>コピー新規登録ステータスか</summary>
		public bool IsActionStatusCopyInsert { get { return (this.ActionStatus == ActionStatus.CopyInsert); } }
		/// <summary>アクションステータス</summary>
		public ActionStatus ActionStatus { get; set; }
	}
}