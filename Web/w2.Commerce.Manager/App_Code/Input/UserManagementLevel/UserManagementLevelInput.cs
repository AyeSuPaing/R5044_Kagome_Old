/*
=========================================================================================================
  Module      : ユーザー管理レベル入力クラス (UserManagementLevelInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Input;
using w2.Domain.UserManagementLevel;

/// <summary>
/// ユーザー管理レベルマスタ入力クラス
/// </summary>
[Serializable]
public class UserManagementLevelInput : InputBase<UserManagementLevelModel>
{	
	/// <summary>削除フラグ：有効</summary>
	public const string FLG_DELETE_VALID = "1";
	/// <summary>削除フラグ：無効</summary>
	public const string FLG_DELETE_INVALID = "0";
	/// <summary>削除フラグ：フィールド名</summary>
	private const string FIELD_DELETE_FLG = "del_flg";
	/// <summary>前回値用</summary>
	private const string TAG_OLD = "_old";

	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public UserManagementLevelInput()
	{
		this.SeqNo = string.Empty;
		this.UserManagementLevelId = string.Empty;
		this.UserManagementLevelName = string.Empty;
		this.DisplayOrder = "1";
		this.DateCreated = string.Empty;
		this.DateChanged = string.Empty;
		this.LastChanged = string.Empty;
		this.UserManagementLevelNameOld = this.UserManagementLevelName;
		this.DisplayOrderOld = this.DisplayOrder;
		this.DelFlg = FLG_DELETE_INVALID;
		this.DelFlgOld = FLG_DELETE_INVALID;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public UserManagementLevelInput(UserManagementLevelModel model)
		: this()
	{
		this.SeqNo = model.SeqNo.ToString();
		this.UserManagementLevelId = model.UserManagementLevelId;
		this.UserManagementLevelName = model.UserManagementLevelName;
		this.DisplayOrder = model.DisplayOrder.ToString();
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
		this.UserManagementLevelNameOld = this.UserManagementLevelName;
		this.DisplayOrderOld = this.DisplayOrder;
		this.DelFlg = FLG_DELETE_INVALID;
		this.DelFlgOld = FLG_DELETE_INVALID;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override UserManagementLevelModel CreateModel()
	{
		var model = new UserManagementLevelModel
		{
			UserManagementLevelId = this.UserManagementLevelId,
			UserManagementLevelName = this.UserManagementLevelName,
			DisplayOrder = int.Parse(this.DisplayOrder),
			LastChanged = this.LastChanged,
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate()
	{
		var errorMessage = Validator.Validate("userManagementLevel", this.DataSource)
			.Replace("@@ 1 @@", this.UserManagementLevelId);
		return errorMessage;
	}
	#endregion

	#region プロパティ
	/// <summary>シーケンス番号</summary>
	public string SeqNo
	{
		get { return (string)this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_SEQ_NO]; }
		set { this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_SEQ_NO] = value; }
	}
	/// <summary>ユーザー管理レベルID</summary>
	public string UserManagementLevelId
	{
		get { return (string)this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_ID]; }
		set { this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_ID] = value; }
	}
	/// <summary>ユーザー管理レベル名</summary>
	public string UserManagementLevelName
	{
		get { return (string)this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_NAME]; }
		set { this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_NAME] = value; }
	}
	/// <summary>表示順</summary>
	public string DisplayOrder
	{
		get { return (string)this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_DISPLAY_ORDER]; }
		set { this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_DISPLAY_ORDER] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_LAST_CHANGED] = value; }
	}
	/// <summary>ユーザ管理レベル名 前回値</summary>
	public string UserManagementLevelNameOld
	{
		get { return (string)this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_NAME + TAG_OLD]; }
		set { this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_NAME + TAG_OLD] = value; }
	}
	/// <summary>表示順 更新用 前回値</summary>
	public string DisplayOrderOld
	{
		get { return (string)this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_DISPLAY_ORDER + TAG_OLD]; }
		set { this.DataSource[Constants.FIELD_USERMANAGEMENTLEVEL_DISPLAY_ORDER + TAG_OLD] = value; }
	}
	/// <summary>削除フラグ 更新用</summary>
	public string DelFlg
	{
		get { return (string)this.DataSource[FIELD_DELETE_FLG]; }
		set { this.DataSource[FIELD_DELETE_FLG] = value; }
	}
	/// <summary>削除フラグ 更新用 前回値</summary>
	public string DelFlgOld
	{
		get { return (string)this.DataSource[FIELD_DELETE_FLG + TAG_OLD]; }
		set { this.DataSource[FIELD_DELETE_FLG + TAG_OLD] = value; }
	}
	#endregion
}
