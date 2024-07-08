/*
=========================================================================================================
  Module      : ユーザ拡張項目マスタ入力クラス(UserExtendInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.CrossPoint.User;
using w2.App.Common.DataCacheController;
using w2.App.Common.Input;
using w2.App.Common.User;
using w2.Domain.User;

/// <summary>
/// ユーザ拡張項目マスタ入力クラス
/// </summary>
[Serializable]
public class UserExtendInput : InputBase<UserExtendModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public UserExtendInput()
	{
		this.UserId = string.Empty;
		this.DataSource = new Hashtable();
		this.UserExtendDataValue = new Dictionary<string, string>();
		this.UserExtendDataText = new Dictionary<string, string>();
		this.UserExtendColumns = new List<string>();
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="userExtend">ユーザ拡張項目モデル</param>
	public UserExtendInput(UserExtendModel userExtend)
		: this()
	{
		this.UserId = userExtend.UserId;
		this.DateCreated = userExtend.DateCreated;
		this.DateChanged = userExtend.DateChanged;
		this.LastChanged = userExtend.LastChanged;

		this.DataSource = userExtend.DataSource;
		this.UserExtendDataValue = userExtend.UserExtendDataValue;
		this.UserExtendDataText = userExtend.UserExtendDataText;
		this.UserExtendColumns = userExtend.UserExtendColumns;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override UserExtendModel CreateModel()
	{
		var constroller = DataCacheControllerFacade.GetUserExtendSettingCacheController();
		var userExtendSettings = constroller.GetModifyUserExtendSettingList(false, Constants.FLG_USEREXTENDSETTING_DISPLAY_EC);
		var userExtend = new UserExtendModel(userExtendSettings)
		{
			UserId = this.UserId,
			LastChanged = this.LastChanged,
			UserExtendDataValue = new UserExtendModel.UserExtendData(this.UserExtendDataValue),
			UserExtendDataText = new UserExtendModel.UserExtendData(this.UserExtendDataText),
			UserExtendColumns = this.UserExtendColumns,
		};
		return userExtend;
	}
	#endregion

	#region プロパティ
	/// <summary>ユーザID</summary>
	public string UserId
	{
		get { return (string)this.DataSource[Constants.FIELD_USEREXTEND_USER_ID]; }
		set { this.DataSource[Constants.FIELD_USEREXTEND_USER_ID] = value; }
	}
	/// <summary>作成日</summary>
	public DateTime DateCreated
	{
		get { return (DateTime)this.DataSource[Constants.FIELD_USEREXTEND_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_USEREXTEND_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public DateTime DateChanged
	{
		get { return (DateTime)this.DataSource[Constants.FIELD_USEREXTEND_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_USEREXTEND_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_USEREXTEND_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_USEREXTEND_LAST_CHANGED] = value; }
	}
	/// <summary>ユーザ拡張項目の選択値 ※キーはユーザ拡張項目IDです</summary>
	public Dictionary<string, string> UserExtendDataValue { get; set; }
	/// <summary>ユーザ拡張項目の選択値に紐づく表示名 ※キーはユーザ拡張項目IDです　※CheckBoxはカンマ連結</summary>
	public Dictionary<string, string> UserExtendDataText { get; set; }
	/// <summary>ユーザ拡張項目のカラムID一覧</summary>
	public List<string> UserExtendColumns { get; set; }
	/// <summary>Shop card no</summary>
	public string CrossPointShopCardNo
	{
		get { return this.UserExtendDataText[Constants.CROSS_POINT_USREX_SHOP_CARD_NO]; }
		set { this.DataSource[Constants.CROSS_POINT_USREX_SHOP_CARD_NO] = value; }
	}
	/// <summary>Shop card pin</summary>
	public string CrossPointShopCardPin
	{
		get { return this.UserExtendDataText[Constants.CROSS_POINT_USREX_SHOP_CARD_PIN]; }
		set { this.DataSource[Constants.CROSS_POINT_USREX_SHOP_CARD_PIN] = value; }
	}
	#endregion
}