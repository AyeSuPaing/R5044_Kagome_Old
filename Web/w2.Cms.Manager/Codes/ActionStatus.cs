/*
=========================================================================================================
  Module      : アクションステータス(ActionStatus.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Cms.Manager.Codes
{
	/// <summary>
	/// アクションステータス
	/// </summary>
	public enum ActionStatus
	{
		/// <summary>一覧</summary>
		List,
		/// <summary>登録</summary>
		Insert,
		/// <summary>確認</summary>
		Confirm,
		/// <summary>更新</summary>
		Update,
		/// <summary>詳細</summary>
		Detail,
		/// <summary>削除</summary>
		Delete,
		/// <summary>セレクトチェンジ</summary>
		SelectChange,
		/// <summary>コピー新規登録</summary>
		CopyInsert,
		/// <summary>最上位カテゴリ登録</summary>
		RegistRootCategory,
		/// <summary>子カテゴリ登録</summary>
		RegistChildCategory,
		/// <summary>編集</summary>
		Edit,
		/// <summary>カテゴリ選択</summary>
		SelectCategory,
		/// <summary>戻る</summary>
		Back,
		/// <summary>カテゴリ詳細に戻る</summary>
		BackCategoryDetail,
		/// <summary>更新確認</summary>
		ConfirmUpdate,
		/// <summary>登録確認</summary>
		ConfirmRegist,
	}
}