/*
=========================================================================================================
  Module      : マスタ出力定義入力クラス (MasterExportSettingInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Input;
using w2.Domain.MasterExportSetting;

/// <summary>
/// マスタ出力定義入力クラス
/// </summary>
public class MasterExportSettingInput : InputBase<MasterExportSettingModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public MasterExportSettingInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public MasterExportSettingInput(MasterExportSettingModel model)
		: this()
	{
		this.ShopId = model.ShopId;
		this.MasterKbn = model.MasterKbn;
		this.SettingId = model.SettingId;
		this.SettingName = model.SettingName;
		this.Fields = model.Fields;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
		this.ExportFileType = model.ExportFileType;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override MasterExportSettingModel CreateModel()
	{
		var model = new MasterExportSettingModel
		{
			ShopId = this.ShopId,
			MasterKbn = this.MasterKbn,
			SettingId = this.SettingId,
			SettingName = this.SettingName,
			Fields = this.Fields,
			LastChanged = this.LastChanged,
			ExportFileType = this.ExportFileType,
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <param name="isRegister">新規登録か</param>
	/// <returns>エラーメッセージ</returns>
	public string Validate(bool isRegister)
	{
		var errorMessage = Validator.Validate(
			isRegister ? "MasterExportSettingRegist" : "MasterExportSettingModify",
			this.DataSource);

		var isDuplication = new MasterExportSettingService().CheckNameDuplication(
			this.ShopId,
			this.MasterKbn,
			this.SettingName);
		if (isDuplication)
		{
			errorMessage += WebMessages.GetMessages(WebMessages.INPUTCHECK_DUPLICATION).Replace("@@ 1 @@", "設定名");
		}
		return errorMessage;
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_SHOP_ID] = value; }
	}
	/// <summary>マスタ区分</summary>
	public string MasterKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN]; }
		set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN] = value; }
	}
	/// <summary>設定ID</summary>
	public string SettingId
	{
		get { return (string)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_SETTING_ID]; }
		set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_SETTING_ID] = value; }
	}
	/// <summary>設定名</summary>
	public string SettingName
	{
		get { return (string)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_SETTING_NAME]; }
		set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_SETTING_NAME] = value; }
	}
	/// <summary>フィールド列</summary>
	public string Fields
	{
		get { return (string)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_FIELDS]; }
		set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_FIELDS] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_LAST_CHANGED] = value; }
	}
	/// <summary>出力ファイル形式</summary>
	public string ExportFileType
	{
		get { return (string)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_EXPORT_FILE_TYPE]; }
		set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_EXPORT_FILE_TYPE] = value; }
	}
	#endregion
}
