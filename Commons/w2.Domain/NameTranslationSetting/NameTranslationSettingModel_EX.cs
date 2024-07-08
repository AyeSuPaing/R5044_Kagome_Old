/*
=========================================================================================================
  Module      : 名称翻訳設定マスタモデル (NameTranslationSettingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.Product;
using w2.Domain.SetPromotion;

namespace w2.Domain.NameTranslationSetting
{
	/// <summary>
	/// 名称翻訳設定マスタモデル
	/// </summary>
	public partial class NameTranslationSettingModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>翻訳前名称</summary>
		public string BeforeTranslationalName
		{
			get
			{
				if (this.DataSource["before_translational_name"] == DBNull.Value) return null;
				return (string)this.DataSource["before_translational_name"];
			}
			set { this.DataSource["before_translational_name"] = value; }
		}
		/// <summary>翻訳前HTML区分</summary>
		public string BeforeTranslationalDisplayKbn
		{
			get { return (string)this.DataSource["before_translational_display_kbn"]; }
			set { this.DataSource["before_translational_display_kbn"] = value; }
		}

		#region セットプロモーションの名称切り替えに使用
		/// <summary>表示用セットプロモーション名</summary>
		public string SetPromotionDispName
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_DISP_NAME]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_DISP_NAME] = value; }
		}
		/// <summary>表示用文言</summary>
		public string Description
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION] = value; }
		}
		/// <summary>表示用文言HTML区分</summary>
		public string DescriptionKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION_KBN]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION_KBN] = value; }
		}
		#endregion

		#region 商品一覧表示設定の名称切り替えに使用
		/// <summary>表示名</summary>
		public string ProductListDispSettingName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_NAME] = value; }
		}
		#endregion

		#region ユーザ拡張項目設定の名称切り替えに使用
		/// <summary>名称</summary>
		public string SettingName
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTENDSETTING_SETTING_NAME]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_SETTING_NAME] = value; }
		}
		/// <summary>ユーザ拡張項目概要表示区分</summary>
		public string OutlineKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTENDSETTING_OUTLINE_KBN]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_OUTLINE_KBN] = value; }
		}
		/// <summary>ユーザ拡張項目概要</summary>
		public string Outline
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTENDSETTING_OUTLINE]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_OUTLINE] = value; }
		}
		#endregion

		#region 新着情報の名称切り替えに使用
		/// <summary>本文区分</summary>
		public string NewsTextKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_KBN]; }
			set { this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_KBN] = value; }
		}
		/// <summary>本文</summary>
		public string NewsText
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT]; }
			set { this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT] = value; }
		}
		#endregion
		
		/// <summary>
		/// 拡張表示HTML区分
		/// </summary>
		public string ExtendDisplayKbn
		{
			get
			{
				if (this.IsDisplayKbnZeroOrOne)
				{
					return this.DisplayKbn;
				}
				else if (this.IsDisplayKbnTextOrHtml)
				{
					return (this.DisplayKbn == Constants.FLG_USEREXTENDSETTING_OUTLINE_TEXT) ? "0" : "1";
				}
				else
				{
					return "0";
				}
			}
			set
			{
				if (this.IsDisplayKbnZeroOrOne)
				{
					this.DisplayKbn = value;
				}
				else if (this.IsDisplayKbnTextOrHtml)
				{
					this.DisplayKbn = (value == "0") ? Constants.FLG_USEREXTENDSETTING_OUTLINE_TEXT : Constants.FLG_USEREXTENDSETTING_OUTLINE_HTML;
				}
				else
				{
					this.DisplayKbn = string.Empty;
				}
			}
		}
		/// <summary>表示HTML区分が0、または、1の対象データ区分と翻訳対象項目の組</summary>
		private bool IsDisplayKbnZeroOrOne
		{
			get
			{
				return ((this.DataKbn == Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT)
						&& ((this.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_OUTLINE)
							|| (this.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL1)
							|| (this.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL2)
							|| (this.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL3)
							|| (this.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL4)))
						|| ((this.DataKbn == Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION)
							&& (this.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SETPROMOTION_DESCRIPTION))
						|| ((this.DataKbn == Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET)
							&& (this.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTSET_DESCRIPTION))
						|| ((this.DataKbn == Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS)
							&& (this.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_NEWS_NEWS_TEXT));
			}
		}
		/// <summary>表示HTML区分がTEXT、または、HTMLの対象データ区分と翻訳対象項目の組</summary>
		private bool IsDisplayKbnTextOrHtml
		{
			get
			{
				return ((this.DataKbn == Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING)
						&& (this.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_USEREXTENDSETTING_OUTLINE));
			}
		}
		#endregion
	}
}