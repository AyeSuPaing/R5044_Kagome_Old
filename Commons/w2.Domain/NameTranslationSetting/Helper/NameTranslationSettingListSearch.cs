/*
=========================================================================================================
  Module      : 名称翻訳設定一覧情報取得のためのヘルパクラス (NameTranslationSettingListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.NameTranslationSetting.Helper
{
	/// <summary>
	/// 名称翻訳設定一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class NameTranslationSettingListSearchCondition : BaseDbMapModel
	{
		/// <summary>対象データ区分</summary>
		[DbMapName(Constants.FIELD_NAMETRANSLATIONSETTING_DATA_KBN)]
		public string DataKbn { get; set; }
		/// <summary>翻訳対象項目</summary>
		[DbMapName(Constants.FIELD_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN)]
		public string TranslationTargetColumn { get; set; }
		/// <summary>マスタID1</summary>
		[DbMapName(Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID1)]
		public string MasterId1 { get; set; }
		/// <summary>マスタID2</summary>
		[DbMapName(Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID2)]
		public string MasterId2 { get; set; }
		/// <summary>マスタID1(LIKE検索用)</summary>
		[DbMapName(Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID1 + "_like_escaped")]
		public string MasterId1LikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.MasterId1); }
		}
		/// <summary>マスタID2(LIKE検索用)</summary>
		[DbMapName(Constants.FIELD_NAMETRANSLATIONSETTING_MASTER_ID2 + "_like_escaped")]
		public string MasterId2LikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.MasterId2); }
		}
		/// <summary>言語コード</summary>
		[DbMapName(Constants.FIELD_NAMETRANSLATIONSETTING_LANGUAGE_CODE)]
		public string LanguageCode { get; set; }
		/// <summary>言語ロケールID</summary>
		[DbMapName(Constants.FIELD_NAMETRANSLATIONSETTING_LANGUAGE_LOCALE_ID)]
		public string LanguageLocaleId { get; set; }
		/// <summary>開始行番号</summary>
		[DbMapName(Constants.FIELD_COMMON_BEGIN_NUM)]
		public int BeginRowNumber { get; set; }
		/// <summary>終了行番号</summary>
		[DbMapName(Constants.FIELD_COMMON_END_NUM)]
		public int EndRowNumber { get; set; }
	}
}