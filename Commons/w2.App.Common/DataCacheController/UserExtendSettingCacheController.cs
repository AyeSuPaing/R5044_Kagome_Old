/*
=========================================================================================================
  Module      : ユーザー拡張項目設定キャッシュプロバイダ(UserExtendSettingCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.User.Helper;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// ユーザー拡張項目設定キャッシュプロバイダ
	/// </summary>
	public class UserExtendSettingCacheController : DataCacheControllerBase<UserExtendSettingList>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal UserExtendSettingCacheController()
			: base(RefreshFileType.UserExtendSetting)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			var userService = DomainFacade.Instance.UserService;
			this.CacheData = userService.GetUserExtendSettingList();
		}

		/// <summary>
		/// ユーザー拡張項目設定を取得
		/// </summary>
		/// <param name="isModify">会員情報変更用</param>
		/// <param name="displayKbn">ユーザー拡張情報の表示区分</param>
		/// <returns>ユーザー拡張項目設定</returns>
		public UserExtendSettingList GetModifyUserExtendSettingList(bool isModify, string displayKbn)
		{
			var result = new UserExtendSettingList();
			result.Items.AddRange(
				this.CacheData.Items.Where(userExtend =>
					(userExtend.DisplayKbn.Contains(displayKbn)
						&& (userExtend.IsDiaplayableForShopAppMember == false))
					&& (isModify ? (userExtend.InitOnlyFlg == Constants.FLG_USEREXTENDSETTING_UPDATABLE) : true)
			));
			return result;
		}
	}
}
