/*
=========================================================================================================
  Module      : アフィリエイトタグ入力クラス(AffiliateTagConditionInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Input;
using w2.Domain.Affiliate;

namespace Input.Affiliate
{
	/// <summary>
	/// アフィリエイトタグ入力クラス
	/// </summary>
	public class AffiliateTagInput : InputBase<AffiliateTagSettingModel>
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AffiliateTagInput()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public AffiliateTagInput(AffiliateTagSettingModel model) : this()
		{
			this.AffiliateId = model.AffiliateId.ToString();
			this.AffiliateName = model.AffiliateName;
			this.AffiliateKbn = model.AffiliateKbn;
			this.SessionName1 = model.SessionName1;
			this.SessionName2 = model.SessionName2;
			this.UserAgentCoopKbn = model.UserAgentCoopKbn;
			this.DisplayOrder = model.DisplayOrder.ToString();
			this.AffiliateTag1 = model.AffiliateTag1;
			this.AffiliateTag2 = model.AffiliateTag2;
			this.AffiliateTag3 = model.AffiliateTag3;
			this.AffiliateTag4 = model.AffiliateTag4;
			this.AffiliateTag5 = model.AffiliateTag5;
			this.AffiliateTag6 = model.AffiliateTag6;
			this.AffiliateTag7 = model.AffiliateTag7;
			this.AffiliateTag8 = model.AffiliateTag8;
			this.AffiliateTag9 = model.AffiliateTag9;
			this.AffiliateTag10 = model.AffiliateTag10;
			this.ValidFlg = model.ValidFlg;
			this.LastChanged = model.LastChanged;
			this.AffiliateProductTagId = (model.AffiliateProductTagId != null)
				? model.AffiliateProductTagId.ToString()
				: string.Empty;
			this.OutputLocation = model.OutputLocation;
		}

		/// <summary>
		/// モデル生成
		/// </summary>
		/// <returns>モデル</returns>
		public override AffiliateTagSettingModel CreateModel()
		{
			var model = new AffiliateTagSettingModel
			{
				AffiliateName = this.AffiliateName,
				AffiliateKbn = this.AffiliateKbn,
				SessionName1 = (string.IsNullOrEmpty(this.SessionName1) == false) ? this.SessionName1 : string.Empty,
				SessionName2 = (string.IsNullOrEmpty(this.SessionName2) == false) ? this.SessionName2 : string.Empty,
				UserAgentCoopKbn = (string.IsNullOrEmpty(this.UserAgentCoopKbn) == false)
					? this.UserAgentCoopKbn
					: string.Empty,
				DisplayOrder = int.Parse(this.DisplayOrder),
				AffiliateTag1 = (string.IsNullOrEmpty(this.AffiliateTag1) == false) ? this.AffiliateTag1 : string.Empty,
				AffiliateTag2 = (string.IsNullOrEmpty(this.AffiliateTag2) == false) ? this.AffiliateTag2 : string.Empty,
				AffiliateTag3 = (string.IsNullOrEmpty(this.AffiliateTag3) == false) ? this.AffiliateTag3 : string.Empty,
				AffiliateTag4 = (string.IsNullOrEmpty(this.AffiliateTag4) == false) ? this.AffiliateTag4 : string.Empty,
				AffiliateTag5 = (string.IsNullOrEmpty(this.AffiliateTag5) == false) ? this.AffiliateTag5 : string.Empty,
				AffiliateTag6 = (string.IsNullOrEmpty(this.AffiliateTag6) == false) ? this.AffiliateTag6 : string.Empty,
				AffiliateTag7 = (string.IsNullOrEmpty(this.AffiliateTag7) == false) ? this.AffiliateTag7 : string.Empty,
				AffiliateTag8 = (string.IsNullOrEmpty(this.AffiliateTag8) == false) ? this.AffiliateTag8 : string.Empty,
				AffiliateTag9 = (string.IsNullOrEmpty(this.AffiliateTag9) == false) ? this.AffiliateTag9 : string.Empty,
				AffiliateTag10 =
					(string.IsNullOrEmpty(this.AffiliateTag10) == false) ? this.AffiliateTag10 : string.Empty,
				ValidFlg = this.ValidFlg,
				LastChanged = this.LastChanged,
				AffiliateProductTagId = (string.IsNullOrEmpty(this.AffiliateProductTagId) == false)
					? int.Parse(this.AffiliateProductTagId)
					: (int?)null,
				OutputLocation =
					(string.IsNullOrEmpty(this.OutputLocation) == false) ? this.OutputLocation : string.Empty
			};

			if ((string.IsNullOrEmpty(this.AffiliateId) == false))
			{
				model.AffiliateId = int.Parse(this.AffiliateId);
			}

			return model;
		}

		/// <summary>
		/// バリデーション
		/// </summary>
		/// <returns>結果</returns>
		public string Validate()
		{
			var errorMessages = Validator.Validate("AffiliateTagSettingPC", this.DataSource);
			return errorMessages;
		}

		#region プロパティ
		/// <summary>アフィリエイトID</summary>
		public string AffiliateId
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_ID]; }
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
		public string DisplayOrder
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_DISPLAY_ORDER]; }
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
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_LAST_CHANGED] = value; }
		}
		/// <summary>アフィリエイト商品タグID</summary>
		public string AffiliateProductTagId
		{
			get
			{
				if (this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_PRODUCT_TAG_ID]
					== DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_PRODUCT_TAG_ID];
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