/*
=========================================================================================================
  Module      : サイト基本情報翻訳設定情報クラス (SiteInformationTranslation.cs)
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
	/// サイト基本情報翻訳設定情報
	/// </summary>
	[Serializable]
	public class SiteInformationTranslation : NameTranslationSettingModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="groupedDataByLanguageCodeAndLanguageLocaleId">言語コード、言語ロケールIDでグルーピングされたデータ</param>
		public SiteInformationTranslation(IEnumerable<NameTranslationSettingModel> groupedDataByLanguageCodeAndLanguageLocaleId)
		{
			this.LanguageCode = groupedDataByLanguageCodeAndLanguageLocaleId.First().LanguageCode;
			this.LanguageLocaleId = groupedDataByLanguageCodeAndLanguageLocaleId.First().LanguageLocaleId;

			foreach (var data in groupedDataByLanguageCodeAndLanguageLocaleId)
			{
				var targetColumn = data.TranslationTargetColumn;
				var afterTranslationalName = data.AfterTranslationalName;
				switch (targetColumn)
				{
					case Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_SHOP_NAME:
						this.ShopName = afterTranslationalName;
						break;

					case Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_COMPANY_NAME:
						this.CompanyName = afterTranslationalName;
						break;

					case Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_CONTACT_CENTER_INFO:
						this.ContactCenterInfo = afterTranslationalName;
						break;
				}
			}
		}
		#endregion

		#region プロパティ
		/// <summary>サイト名</summary>
		public string ShopName
		{
			get { return (string)this.DataSource[Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_SHOP_NAME]; }
			set { this.DataSource[Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_SHOP_NAME] = value; }
		}
		/// <summary>企業名</summary>
		public string CompanyName
		{
			get { return (string)this.DataSource[Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_COMPANY_NAME]; }
			set { this.DataSource[Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_COMPANY_NAME] = value; }
		}
		/// <summary>問い合わせ窓口情報</summary>
		public string ContactCenterInfo
		{
			get { return (string)this.DataSource[Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_CONTACT_CENTER_INFO]; }
			set { this.DataSource[Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_CONTACT_CENTER_INFO] = value; }
		}
		#endregion
	}
}