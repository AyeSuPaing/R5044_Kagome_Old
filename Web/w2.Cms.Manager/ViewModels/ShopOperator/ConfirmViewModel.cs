/*
=========================================================================================================
  Module      : 店舗管理者Confirmビューモデル(ConfirmViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Common.Util;
using w2.Domain.MenuAuthority;
using w2.Domain.MenuAuthority.Helper;
using w2.Domain.ShopOperator;

namespace w2.Cms.Manager.ViewModels.ShopOperator
{
	/// <summary>
	/// 店舗管理者Confirmビューモデル
	/// </summary>
	[Serializable]
	public class ConfirmViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="pageLayout">ページレイアウト</param>
		/// <param name="shopOperator">店舗管理者</param>
		public ConfirmViewModel(ActionStatus actionStatus, string pageLayout, ShopOperatorModel shopOperator = null)
		{
			this.ActionStatus = actionStatus;
			this.PageLayout = pageLayout;
			SetOperaor(actionStatus, shopOperator);
		}

		/// <summary>
		/// オペレーター情報セット
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="shopOperator">店舗管理者</param>
		private void SetOperaor(ActionStatus actionStatus, ShopOperatorModel shopOperator)
		{
			if (shopOperator == null) return;

			this.OperatorId = shopOperator.OperatorId;
			this.Name = shopOperator.Name;
			this.MenuAccessLevel = shopOperator.GetMenuAccessLevel(MenuAuthorityHelper.ManagerSiteType.Cms);
			this.MenuAuthorityName = Constants.STRING_UNACCESSABLEUSER_NAME; // デフォルトは権限なし
			if (this.MenuAccessLevel.HasValue)
			{
				if (this.MenuAccessLevel.Value == Constants.KBN_OPERATOR_LEVEL_SUPERUSER)
				{
					this.MenuAuthorityName = Constants.STRING_SUPERUSER_NAME;
				}
				else
				{
					var menuAuthorities = new MenuAuthorityService().Get(
						shopOperator.ShopId,
						MenuAuthorityHelper.ManagerSiteType.Cms,
						this.MenuAccessLevel.Value);
					if (menuAuthorities.Length != 0)
					{
						this.MenuAuthorityName = menuAuthorities[0].MenuAuthorityName;
					}
				}
			}
			this.LoginId = shopOperator.LoginId;
			this.Password = shopOperator.Password;
			this.MailAddr = shopOperator.MailAddr;
			this.ValidFlg = ValueText.GetValueText(
				Constants.TABLE_SHOPOPERATOR,
				Constants.FIELD_SHOPOPERATOR_VALID_FLG,
				shopOperator.ValidFlg);
			if (actionStatus == ActionStatus.Detail)
			{
				this.DateCreated = DateTimeUtility.ToStringForManager(shopOperator.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
				this.DateChanged = DateTimeUtility.ToStringForManager(shopOperator.DateChanged, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
				this.LastChanged = shopOperator.LastChanged;
			}
		}

		/// <summary>オペレータID</summary>
		public string OperatorId { get; set; }
		/// <summary>オペレーター名</summary>
		public string Name { get; set; }
		/// <summary>メニューアクセスレベル</summary>
		public int? MenuAccessLevel { get; set; }
		/// <summary>メニュー権限名</summary>
		public string MenuAuthorityName { get; set; }
		/// <summary>ログインID</summary>
		public string LoginId { get; set; }
		/// <summary>パスワード</summary>
		public string Password { get; set; }
		/// <summary>パスワード</summary>
		public string PasswordForDisp
		{
			get { return string.IsNullOrEmpty(this.Password) ? "（パスワード変更なし）" : StringUtility.ChangeToAster(this.Password); }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; set; }
		/// <summary>作成日</summary>
		public string DateCreated { get; set; }
		/// <summary>更新日</summary>
		public string DateChanged { get; set; }
		/// <summary>最終更新者</summary>
		public string LastChanged { get; set; }
		/// <summary>ページレイアウト</summary>
		public string PageLayout { get; set; }
		/// <summary>メールアドレス</summary>
		public string MailAddr { get; set; }
	}
}