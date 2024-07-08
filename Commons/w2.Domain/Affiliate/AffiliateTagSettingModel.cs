/*
=========================================================================================================
  Module      : アフィリエイトタグ設定マスタモデル (AffiliateTagSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Affiliate
{
	/// <summary>
	/// アフィリエイトタグ設定マスタモデル
	/// </summary>
	[Serializable]
	public partial class AffiliateTagSettingModel : ModelBase<AffiliateTagSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AffiliateTagSettingModel()
		{
			this.AffiliateKbn = Constants.FLG_AFFILIATETAGSETTING_AFFILIATE_KBN_PC_SP;
			this.DisplayOrder = 10;
			this.ValidFlg = "1";
			this.AffiliateProductTagId = null;
			this.SessionName1 = string.Empty;
			this.SessionName2 = string.Empty;
			this.UserAgentCoopKbn = string.Empty;
			this.AffiliateTag1 = string.Empty;
			this.AffiliateTag2 = string.Empty;
			this.AffiliateTag3 = string.Empty;
			this.AffiliateTag4 = string.Empty;
			this.AffiliateTag5 = string.Empty;
			this.AffiliateTag6 = string.Empty;
			this.AffiliateTag7 = string.Empty;
			this.AffiliateTag8 = string.Empty;
			this.AffiliateTag9 = string.Empty;
			this.AffiliateTag10 = string.Empty;
			this.OutputLocation = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AffiliateTagSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AffiliateTagSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>アフィリエイトID</summary>
		public int AffiliateId
		{
			get { return (int)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_ID]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_ID] = value; }
		}
		/// <summary>アフィリエイト名</summary>
		public string AffiliateName
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_NAME]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_NAME] = value; }
		}
		/// <summary>アフィリエイト区分</summary>
		public string AffiliateKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_KBN]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_KBN] = value; }
		}
		/// <summary>セッション変数名1</summary>
		public string SessionName1
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_SESSION_NAME1]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_SESSION_NAME1] = value; }
		}
		/// <summary>セッション変数名2</summary>
		public string SessionName2
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_SESSION_NAME2]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_SESSION_NAME2] = value; }
		}
		/// <summary>ユーザーエージェント連携区分</summary>
		public string UserAgentCoopKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_USER_AGENT_COOP_KBN]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_USER_AGENT_COOP_KBN] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_DISPLAY_ORDER] = value; }
		}
		/// <summary>アフィリエイトタグ１</summary>
		public string AffiliateTag1
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG1]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG1] = value; }
		}
		/// <summary>アフィリエイトタグ２</summary>
		public string AffiliateTag2
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG2]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG2] = value; }
		}
		/// <summary>アフィリエイトタグ３</summary>
		public string AffiliateTag3
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG3]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG3] = value; }
		}
		/// <summary>アフィリエイトタグ４</summary>
		public string AffiliateTag4
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG4]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG4] = value; }
		}
		/// <summary>アフィリエイトタグ５</summary>
		public string AffiliateTag5
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG5]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG5] = value; }
		}
		/// <summary>アフィリエイトタグ６</summary>
		public string AffiliateTag6
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG6]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG6] = value; }
		}
		/// <summary>アフィリエイトタグ７</summary>
		public string AffiliateTag7
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG7]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG7] = value; }
		}
		/// <summary>アフィリエイトタグ８</summary>
		public string AffiliateTag8
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG8]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG8] = value; }
		}
		/// <summary>アフィリエイトタグ９</summary>
		public string AffiliateTag9
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG9]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG9] = value; }
		}
		/// <summary>アフィリエイトタグ１０</summary>
		public string AffiliateTag10
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG10]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG10] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_LAST_CHANGED] = value; }
		}
		/// <summary>アフィリエイト商品タグID</summary>
		public int? AffiliateProductTagId
		{
			get
			{
				if (this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_PRODUCT_TAG_ID] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_PRODUCT_TAG_ID];
			}
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_PRODUCT_TAG_ID] = value; }
		}
		/// <summary>出力箇所</summary>
		public string OutputLocation
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_OUTPUT_LOCATION]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_OUTPUT_LOCATION] = value; }
		}
		#endregion
	}
}
