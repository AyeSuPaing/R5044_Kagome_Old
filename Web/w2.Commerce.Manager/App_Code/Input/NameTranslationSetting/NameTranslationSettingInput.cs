/*
=========================================================================================================
  Module      : 名称翻訳設定入力クラス (NameTranslationSettingInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Input;
using w2.Domain.NameTranslationSetting;

/// <summary>
/// 名称翻訳設定マスタ入力クラス
/// </summary>
public class NameTranslationSettingInput : InputBase<NameTranslationSettingModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public NameTranslationSettingInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public NameTranslationSettingInput(NameTranslationSettingModel model)
		: this()
	{
		this.DataKbn = model.DataKbn;
		this.TranslationTargetColumn = model.TranslationTargetColumn;
		this.MasterId1 = model.MasterId1;
		this.MasterId2 = model.MasterId2;
		this.MasterId3 = model.MasterId3;
		this.LanguageCode = model.LanguageCode;
		this.LanguageLocaleId = model.LanguageLocaleId;
		this.AfterTranslationalName = model.AfterTranslationalName;
		this.DisplayKbn = model.DisplayKbn;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override NameTranslationSettingModel CreateModel()
	{
		var model = new NameTranslationSettingModel
		{
			DataKbn = this.DataKbn,
			TranslationTargetColumn = this.TranslationTargetColumn,
			MasterId1 = this.MasterId1,
			MasterId2 = this.MasterId2,
			MasterId3 = this.MasterId3,
			LanguageCode = this.LanguageCode,
			LanguageLocaleId = this.LanguageLocaleId,
			AfterTranslationalName = this.AfterTranslationalName,
			ExtendDisplayKbn = this.DisplayKbn,
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate()
	{
		var errorMessage = Validator.Validate("NameTranslationSettingRegister", this.DataSource);
		return errorMessage;
	}
	#endregion

	#region プロパティ
	public string DataKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_DATA_KBN]; }
		set { this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_DATA_KBN] = value; }
	}
	/// <summary>翻訳対象項目</summary>
	public string TranslationTargetColumn
	{
		get { return (string)this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN]; }
		set { this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN] = value; }
	}
	/// <summary>マスタID1</summary>
	public string MasterId1
	{
		get { return (string)this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID1]; }
		set { this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID1] = value; }
	}
	/// <summary>マスタID2</summary>
	public string MasterId2
	{
		get { return (string)this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID2]; }
		set { this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID2] = value; }
	}
	/// <summary>マスタID3</summary>
	public string MasterId3
	{
		get { return (string)this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID3]; }
		set { this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID3] = value; }
	}
	/// <summary>言語コード</summary>
	public string LanguageCode
	{
		get { return (string)this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_LANGUAGE_CODE]; }
		set { this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_LANGUAGE_CODE] = value; }
	}
	/// <summary>言語ロケールID</summary>
	public string LanguageLocaleId
	{
		get { return (string)this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_LANGUAGE_LOCALE_ID]; }
		set { this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_LANGUAGE_LOCALE_ID] = value; }
	}
	/// <summary>翻訳後名称</summary>
	public string AfterTranslationalName
	{
		get { return (string)this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_AFTER_TRANSLATIONAL_NAME]; }
		set { this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_AFTER_TRANSLATIONAL_NAME] = value; }
	}
	/// <summary>表示HTML区分</summary>
	public string DisplayKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_DISPLAY_KBN]; }
		set { this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_DISPLAY_KBN] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_DATE_CHANGED] = value; }
	}
	#endregion
}
