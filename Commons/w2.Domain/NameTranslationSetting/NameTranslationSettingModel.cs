/*
=========================================================================================================
  Module      : 名称翻訳設定マスタモデル (NameTranslationSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.NameTranslationSetting
{
	/// <summary>
	/// 名称翻訳設定マスタモデル
	/// </summary>
	[Serializable]
	public partial class NameTranslationSettingModel : ModelBase<NameTranslationSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public NameTranslationSettingModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public NameTranslationSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public NameTranslationSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>対象データ区分</summary>
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
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_NAMETRANSLATIONSETTING_DATE_CHANGED] = value; }
		}
		#endregion
	}
}
