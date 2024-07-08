/*
=========================================================================================================
  Module      : 更新対象ユーザID格納クラス(UpdatedUser.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common;
using w2.Common.Web;
using w2.Domain.User;

namespace w2.App.Common.User.UpdateAddressOfUserandFixedPurchase
{
	/// <summary>
	/// 更新対象ユーザID格納クラス
	/// </summary>
	[Serializable()]
	public class UpdatedUser : IUpdated
	{
		/// <summary>
		/// URL生成
		/// </summary>
		/// <returns>URL</returns>
		public string CreateUrl()
		{
			return new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_CONFIRM)
				.AddParam(Constants.REQUEST_KEY_USER_ID, this.Id)
				.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
				.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP)
				.CreateUrl();
		}

		/// <summary>更新対象ID</summary>
		public string Id { get; set; }
		/// <summary>更新対象区分：ユーザー情報</summary>
		public UpdatedKbn UpdatedKbn { get { return UpdatedKbn.User; } }
		/// <summary>ユーザー情報</summary>
		public UserModel User { get; set; }
	}
}
