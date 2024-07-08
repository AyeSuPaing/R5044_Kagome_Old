/*
=========================================================================================================
  Module      : 店舗管理者Registerビューモデル(RegisterViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Cms.Manager.Codes;
using w2.Domain.MenuAuthority;

namespace w2.Cms.Manager.ViewModels.ShopOperator
{
	/// <summary>
	/// 店舗管理者Registerビューモデル
	/// </summary>
	[Serializable]
	public class RegisterViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RegisterViewModel()
		{
			this.MenuAuthorities = new MenuAuthorityModel[0];
		}

		/// <summary>メニュー権限一覧</summary>
		public MenuAuthorityModel[] MenuAuthorities { get; set; }
		/// <summary>オペレータID</summary>
		public string OperatorId { get; set; }
		/// <summary>オペレーター名</summary>
		public string Name { get; set; }
		/// <summary>メニュー権限</summary>
		public int? MenuAccessLevel { get; set; }
		/// <summary>ログインID</summary>
		public string LoginId { get; set; }
		/// <summary>パスワード</summary>
		public string Password { get; set; }
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; set; }
		/// <summary>有効か</summary>
		public bool IsValid
		{
			get { return (this.ValidFlg == Constants.FLG_SHOPOPERATOR_VALID_FLG_VALID); }
		}
		/// <summary>メールアドレス</summary>
		public string MailAddr { get; set; }
	}
}