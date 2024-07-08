/*
=========================================================================================================
  Module      : ユーザー拡張項目設定翻訳設定情報クラス (UserExtendSettingTranslation.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace w2.Domain.NameTranslationSetting.Helper
{
	/// <summary>
	/// ユーザー拡張項目翻訳設定情報
	/// </summary>
	[Serializable]
	public class UserExtendSettingTranslation : NameTranslationSettingModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserExtendSettingTranslation()
		{
			this.SettingNameTranslationList = new List<UserExtendSettingTranslation>();
			this.OutlineTranslationList = new List<UserExtendSettingTranslation>();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="groupedDataBySettingId">設定IDでグルーピングされたデータ</param>
		public UserExtendSettingTranslation(IEnumerable<NameTranslationSettingModel> groupedDataBySettingId)
		{
			this.MasterId1 = groupedDataBySettingId.First().MasterId1;
			this.SettingNameTranslationList = new List<UserExtendSettingTranslation>();
			this.OutlineTranslationList = new List<UserExtendSettingTranslation>();

			foreach (var data in groupedDataBySettingId)
			{
				var model = new UserExtendSettingTranslation
				{
					DataKbn = data.DataKbn,
					MasterId1 = data.MasterId1,
					LanguageCode = data.LanguageCode,
					LanguageLocaleId = data.LanguageLocaleId,
					AfterTranslationalName = data.AfterTranslationalName,
					TranslationTargetColumn = data.TranslationTargetColumn,
				};

				switch(data.TranslationTargetColumn)
				{
					case Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_USEREXTENDSETTING_SETTING_NAME:
						model.SettingName = data.AfterTranslationalName;
						this.SettingNameTranslationList.Add(model);
						break;

					case Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_USEREXTENDSETTING_OUTLINE:
						model.Outline = data.AfterTranslationalName;
						this.OutlineTranslationList.Add(model);
						break;
				}
			}
		}
		#endregion

		#region プロパティ
		/// <summary>名称(ユーザー拡張項目)</summary>
		public new string SettingName
		{
			get { return (string)this.DataSource["setting_name"]; }
			set { this.DataSource["setting_name"] = value; }
		}
		/// <summary>ユーザ拡張項目概要</summary>
		public new string Outline
		{
			get { return (string)this.DataSource["outline"]; }
			set { this.DataSource["outline"] = value; }
		}
		/// <summary>名称(ユーザー拡張項目)翻訳設定情報リスト</summary>
		public List<UserExtendSettingTranslation> SettingNameTranslationList
		{
			get { return (List<UserExtendSettingTranslation>)this.DataSource["setting_name_translation_list"]; }
			set { this.DataSource["setting_name_translation_list"] = value; }
		}
		/// <summary>ユーザ拡張項目概要翻訳設定情報リスト</summary>
		public List<UserExtendSettingTranslation> OutlineTranslationList
		{
			get { return (List<UserExtendSettingTranslation>)this.DataSource["outline_translation_list"]; }
			set { this.DataSource["outline_translation_list"] = value; }
		}
		#endregion
	}
}
