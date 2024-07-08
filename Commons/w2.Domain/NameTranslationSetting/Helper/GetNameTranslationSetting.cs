/*
=========================================================================================================
  Module      : 名称翻訳設定マスタ取得のためのヘルパクラス (GetNameTranslationSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.NameTranslationSetting.Helper
{
	/// <summary>
	/// 名称翻訳設定マスタ検索条件クラス
	/// </summary>
	[Serializable]
	public class NameTranslationSettingSearchCondition : BaseDbMapModel
	{
		#region プロパティ
		/// <summary>データ区分</summary>
		[DbMapName("data_kbn")]
		public string DataKbn { get; set; }
		/// <summary>翻訳対象項目</summary>
		[DbMapName("translation_target_column")]
		public string TranslationTargetColumn { get; set; }
		/// <summary>マスタID1</summary>
		[DbMapName("master_id1")]
		public string MasterId1 { get; set; }
		/// <summary>マスタID2</summary>
		[DbMapName("master_id2")]
		public string MasterId2 { get; set; }
		/// <summary>マスタID3</summary>
		[DbMapName("master_id3")]
		public string MasterId3 { get; set; }
		/// <summary>言語コード</summary>
		[DbMapName("language_code")]
		public string LanguageCode { get; set; }
		/// <summary>言語ロケールID</summary>
		[DbMapName("language_locale_id")]
		public string LanguageLocaleId { get; set; }
		/// <summary>検索対象マスタID1リスト</summary>
		public List<string> MasterId1List { get; set; }
		/// <summary>検索対象マスタID1配列(シングルクォーテーションエスケープ済み)</summary>
		public string[] MasterId1ArrayEscapedSingleQuotation
		{
			get { return this.MasterId1List.Select(id => id.Replace("'", "''")).ToArray(); }
		}
		#endregion
	}
}
