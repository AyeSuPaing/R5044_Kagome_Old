/*
=========================================================================================================
  Module      : 自動翻訳ワード管理モデル (AutoTranslationWordModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.AutoTranslationWord
{
	/// <summary>
	/// 自動翻訳ワード管理モデル
	/// </summary>
	[Serializable]
	public partial class AutoTranslationWordModel : ModelBase<AutoTranslationWordModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AutoTranslationWordModel()
		{
			this.WordHashKey = "";
			this.LanguageCode = "";
			this.WordBefore = "";
			this.WordAfter = "";
			this.ClearFlg = Constants.FLG_AUTOTRANSLATIONWORD_CLEAR_FLG_ON;
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AutoTranslationWordModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AutoTranslationWordModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>翻訳前ワード ハッシュキー</summary>
		public string WordHashKey
		{
			get { return (string)this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_WORD_HASH_KEY]; }
			set { this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_WORD_HASH_KEY] = value; }
		}
		/// <summary>言語コード</summary>
		public string LanguageCode
		{
			get { return (string)this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_LANGUAGE_CODE]; }
			set { this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_LANGUAGE_CODE] = value; }
		}
		/// <summary>翻訳前ワード</summary>
		public string WordBefore
		{
			get { return (string)this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_WORD_BEFORE]; }
			set { this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_WORD_BEFORE] = value; }
		}
		/// <summary>翻訳後ワード</summary>
		public string WordAfter
		{
			get { return (string)this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_WORD_AFTER]; }
			set { this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_WORD_AFTER] = value; }
		}
		/// <summary>削除対象フラグ</summary>
		public string ClearFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_CLEAR_FLG]; }
			set { this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_CLEAR_FLG] = value; }
		}
		/// <summary>最終利用日時</summary>
		public DateTime DateUsed
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_DATE_USED]; }
			set { this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_DATE_USED] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_AUTOTRANSLATIONWORD_LAST_CHANGED] = value; }
		}
		#endregion
	}
}