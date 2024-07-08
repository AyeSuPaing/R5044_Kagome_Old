/*
=========================================================================================================
  Module      : ユーザ拡張項目設定リストクラス(UserExtendSettingList.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.Domain.UserExtendSetting;

namespace w2.Domain.User.Helper
{
	/// <summary>
	/// ユーザ拡張項目リストクラス
	/// </summary>
	[Serializable]
	public class UserExtendSettingList : IEnumerable
	{
		/// <summary>
		/// IEnumerable.GetEnumerator()の実装
		/// </summary>
		/// <returns>IEnumerator</returns>
		public IEnumerator GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserExtendSettingList()
		{
			this.Items = new List<UserExtendSettingModel>();
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="isModify">会員情報変更用（true:変更出来る項目のみ、false：全て項目）</param>
		/// <param name="displayKbn">ユーザー拡張情報の表示区分(空: 全て、PC: PC表示、EC: EC表示など)</param>
		/// <param name="userExtendSettings">会員拡張設定モデルの配列</param>
		/// <param name="operatorName">オペレータ名（指定無しの場合user固定とする）</param>
		public UserExtendSettingList(
			bool isModify,
			string displayKbn,
			UserExtendSettingModel[] userExtendSettings = null,
			string operatorName = "user")
			: this()
		{
			var items = (userExtendSettings) ?? new UserService().GetUserExtendSettingList().Items.ToArray();
			this.Items.AddRange(items.Where(userEx => 
				(userEx.DisplayKbn.Contains(displayKbn))
					&& (isModify ? (userEx.InitOnlyFlg == Constants.FLG_USEREXTENDSETTING_UPDATABLE) : true)));

			// DB参照して作成したインスタンスのため登録済みに変更する
			this.Items.ForEach(userExtendSetting => userExtendSetting.IsRegisted = true);

			SetOperatorName(operatorName);
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="operatorName">オペレータ名</param>
		public UserExtendSettingList(string operatorName)
			: this(false, string.Empty, null)
		{
			SetOperatorName(operatorName);
		}

		/// <summary>
		/// オペレータ名を設定
		/// </summary>
		/// <param name="operatorName">オペレータ名</param>
		private void SetOperatorName(string operatorName)
		{
			this.Items.ForEach(userExtendSetting => userExtendSetting.LastChanged = operatorName);
		}

		/// <summary>ユーザ拡張項目設定リスト</summary>
		public List<UserExtendSettingModel> Items { get; private set; }
	}
}