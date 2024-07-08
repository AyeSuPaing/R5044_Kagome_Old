/*
=========================================================================================================
  Module      : メニュー権限設定入力クラス(MenuAuthorityInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Linq;
using w2.App.Common.Input;
using w2.App.Common.Manager.Menu;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Domain.MenuAuthority;
using w2.Domain.MenuAuthority.Helper;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// メニュー権限設定入力クラス
	/// </summary>
	public class MenuAuthorityInput : InputBase<MenuAuthorityModel>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MenuAuthorityInput()
		{
			
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override MenuAuthorityModel CreateModel()
		{
			return null;
		}
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <returns>モデル配列</returns>
		public MenuAuthorityModel[] CreateModels(ActionStatus actionStatus, string shopId, string loginOperatorName)
		{
			var menuAuthorityList = this.MenuLarges.SelectMany(
				ml => ml.MenuSmalls.Where(ms => ms.Valid).Select(
					ms => new MenuAuthorityModel
					{
						ShopId = shopId,
						PkgKbn = MenuAuthorityHelper.GetPkgKbn(Constants.ManagerSiteType),
						MenuAuthorityLevel = (actionStatus == ActionStatus.Update) ? this.MenuAuthorityLevel : 0,
						MenuAuthorityName = this.Name,
						MenuPath = ms.MenuPath,
						DefaultDispFlg = (ms.MenuPath == this.DispDefault)
							? Constants.FLG_MENUAUTHORITY_DEFAULT_DISP_FLG_ON
							: Constants.FLG_MENUAUTHORITY_DEFAULT_DISP_FLG_OFF,
						LastChanged = loginOperatorName,
						FunctionLevel = ms.MenuFunctions != null ? ms.MenuFunctions.Where(mf => mf.Enable).Sum(mf => mf.Level) : 0
					})).ToArray();

			return menuAuthorityList;
		}

		/// <summary>
		/// Inputを初期化
		/// </summary>
		/// <param name="menuLarges">ログインオペレーターメニュー</param>
		internal void Initialize(MenuLarge[] menuLarges)
		{
			this.MenuLarges = menuLarges.Select(
				ml => new MenuLargeInput
				{
					Name = ml.Name,
					MenuSmalls = ml.SmallMenus.Select(
						ms => new MenuSmallInput
						{
							Name = ms.Name,
							MenuPath = ms.MenuPath,
							DefaultPageRadioButton = ms.MenuPath,
							MenuFunctions = ms.Functions.Select(
								mf => new MenuFunctionInput
								{
									Name = mf.Name,
									Level = mf.Level
								}).ToArray()
						}).ToArray()
				}).ToArray();
		}

		/// <summary>
		/// InputにMenuAuthorityModelのリストの値をセット
		/// </summary>
		/// <param name="menuLarges">ログインオペレーターメニュー</param>
		/// <param name="menuAuthorityList">メニュー権限設定リスト</param>
		internal void SetMenuAuthorityInput(MenuLarge[] menuLarges, MenuAuthorityModel[] menuAuthorityList)
		{
			this.Name = menuAuthorityList[0].MenuAuthorityName;
			var defaultPage = menuAuthorityList.FirstOrDefault(
				ma => ma.DefaultDispFlg == Constants.FLG_MENUAUTHORITY_DEFAULT_DISP_FLG_ON);
			if (defaultPage != null) this.DispDefault = defaultPage.MenuPath;
			this.MenuAuthorityLevel = menuAuthorityList[0].MenuAuthorityLevel;

			this.MenuLarges = menuLarges.Select(
				ml => new MenuLargeInput
				{
					Name = ml.Name,
					MenuSmalls = ml.SmallMenus.Select(
						ms => new MenuSmallInput
						{
							Valid = menuAuthorityList.Any(ma => ma.MenuPath == ms.MenuPath), // 小メニューのチェックボックスをセット
							Name = ms.Name,
							MenuPath = ms.MenuPath,
							DefaultPageRadioButton = ms.MenuPath,
							MenuFunctions = ms.Functions.Select(
								mf => new MenuFunctionInput
								{
									Name = mf.Name,
									Level = mf.Level,
									Enable = menuAuthorityList.Any(ma => ma.MenuPath == ms.MenuPath)
										&& (mf.Level & menuAuthorityList.First(ma => ma.MenuPath == ms.MenuPath).FunctionLevel) != 0 // 機能のチェックボックスをセット
								}).ToArray()
						}).ToArray()
				}).ToArray();
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate(ActionStatus actionStatus, string shopId)
		{
			this.DataSource = new Hashtable
			{
				{Constants.FIELD_MENUAUTHORITY_SHOP_ID, shopId},
				{Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_NAME, this.Name},
				{"menu_authority_count", this.MenuLarges.Sum(
					ml => ml.MenuSmalls.Count(ms => ms.Valid)).ToString()},
			};

			var errorMessage = Validator.Validate(
				(actionStatus == ActionStatus.Insert)
					? "MenuAuthorityRegister"
					: (actionStatus == ActionStatus.Update)
						? "MenuAuthorityModify"
						: "",
				this.DataSource);

			var defaultMenuFlg = this.MenuLarges.Any(
				ml => ml.MenuSmalls.Any(ms => (ms.DefaultPageRadioButton == this.DispDefault) && (ms.Valid)));

			return defaultMenuFlg == false ? WebMessages.MenuauthorityNoDefaltPage : errorMessage;
		}
		#endregion

		#region プロパティ
		/// <summary>メニュー権限名</summary>
		public string Name { get; set; }
		/// <summary>メニュー権限レベル</summary>
		public int MenuAuthorityLevel { get; set; }
		/// <summary>デフォルト表示</summary>
		public string DispDefault { get; set; }
		/// <summary>大メニューの入力情報リスト</summary>
		public MenuLargeInput[] MenuLarges { get; set; }
		/// <summary>登録か</summary>
		public bool IsInsert { get; set; }

		/// <summary>大メニューの入力</summary>
		public class MenuLargeInput
		{
			/// <summary>大メニュー名</summary>
			public string Name { get; set; }
			/// <summary>小メニューの入力リスト</summary>
			public MenuSmallInput[] MenuSmalls { get; set; }
		}

		/// <summary>小メニューの入力</summary>
		public class MenuSmallInput
		{
			/// <summary>有効</summary>
			public bool Valid { get; set; }
			/// <summary>デフォルトページラジオボタン</summary>
			public string DefaultPageRadioButton { get; set; }
			/// <summary>小メニュー名</summary>
			public string Name { get; set; }
			/// <summary>メニューパス</summary>
			public string MenuPath { get; set; }
			/// <summary>機能メニューの入力リスト</summary>
			public MenuFunctionInput[] MenuFunctions { get; set; }
		}

		/// <summary>機能メニューの入力</summary>
		public class MenuFunctionInput
		{
			/// <summary>機能名</summary>
			public string Name { get; set; }
			/// <summary>有効か</summary>
			public bool Enable { get; set; }
			/// <summary>機能レベル</summary>
			public int Level { get; set; }
		}
		#endregion
	}
}
