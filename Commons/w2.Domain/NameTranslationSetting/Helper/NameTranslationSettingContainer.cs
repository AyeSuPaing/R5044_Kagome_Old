/*
=========================================================================================================
  Module      : 名称翻訳設定表示用コンテナクラス (NameTranslationSettingContainer.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.Common.Util;

namespace w2.Domain.NameTranslationSetting.Helper
{
	/// <summary>
	/// 名称翻訳設定一覧表示用コンテナ
	/// </summary>
	public class NameTranslationSettingContainer
	{
		/// <summary>対象データ区分</summary>
		public string DataKbn { get; set; }
		/// <summary>対象データ区分名</summary>
		public string DataKbnName
		{
			get { return ValueText.GetValueText(Constants.TABLE_NAMETRANSLATIONSETTING, Constants.FIELD_NAMETRANSLATIONSETTING_DATA_KBN, this.DataKbn); }
		}
		/// <summary>翻訳対象項目</summary>
		public string TranslationTargetColumn { get; set; }
		/// <summary>翻訳対象項目名</summary>
		public string TranslationTargetColumnName
		{
			get { return ValueText.GetValueText(Constants.TABLE_NAMETRANSLATIONSETTING, NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[this.DataKbn], this.TranslationTargetColumn); }
		}
		/// <summary>マスタID1</summary>
		public string MasterId1 { get; set; }
		/// <summary>マスタID2</summary>
		public string MasterId2 { get; set; }
		/// <summary>翻訳前名称</summary>
		public string BeforeTranslationalName { get; set; }
		/// <summary>言語毎データ</summary>
		public IEnumerable<NameTranslationSettingModel> Languages { get; set; }
	}
}
