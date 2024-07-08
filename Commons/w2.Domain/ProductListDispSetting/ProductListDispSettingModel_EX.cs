/*
=========================================================================================================
  Module      : 商品一覧表示設定マスタモデル (ProductListDispSettingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Security.Permissions;
using w2.Domain.NameTranslationSetting;

namespace w2.Domain.ProductListDispSetting
{
	/// <summary>
	/// 商品一覧表示設定マスタモデル
	/// </summary>
	public partial class ProductListDispSettingModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>デフォルト表示グラグがONか</summary>
		public bool IsDefaultDispFlgOn
		{
			get { return (this.DefaultDispFlg == Constants.FLG_PRODUCTLISTDISPSETTING_DEFAULT_DISP_FLG_ON); }
		}
		/// <summary>設定名(翻訳前)</summary>
		public string SettingNameBeforeTranslation
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_NAME + "_before_translation"]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_NAME + "_before_translation"] = value; }
		}
		/// <summary>設定名翻訳設定情報</summary>
		public NameTranslationSettingModel[] SettingNameTranslationData
		{
			get { return (NameTranslationSettingModel[])this.DataSource["settingname_translation_data"]; }
			set { this.DataSource["settingname_translation_data"] = value; }
		}
		/// <summary>件数の設定か</summary>
		public bool IsCountSetting
		{
			get { return (this.SettingKbn == Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_COUNT); }
		}
		#endregion
	}
}
