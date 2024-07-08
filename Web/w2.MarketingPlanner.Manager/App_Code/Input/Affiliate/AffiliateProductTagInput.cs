/*
=========================================================================================================
  Module      : アフィリエイト商品タグ入力クラス(AffiliateProductTagInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Input;
using w2.Domain.Affiliate;

namespace Input.Affiliate
{
	/// <summary>
	/// アフィリエイト商品タグ入力クラス
	/// </summary>
	public class AffiliateProductTagInput : InputBase<AffiliateProductTagSettingModel>
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AffiliateProductTagInput()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public AffiliateProductTagInput(AffiliateProductTagSettingModel model) : this()
		{
			this.AffiliateProductTagId = model.AffiliateProductTagId.ToString();
			this.TagName = model.TagName;
			this.TagContent = model.TagContent;
			this.TagDelimiter = model.TagDelimiter;
			this.LastChanged = model.LastChanged;
		}

		/// <summary>
		/// モデル生成
		/// </summary>
		/// <returns>モデル</returns>
		public override AffiliateProductTagSettingModel CreateModel()
		{
			var model = new AffiliateProductTagSettingModel
			{
				TagName = this.TagName,
				TagContent = this.TagContent,
				TagDelimiter = this.TagDelimiter,
				LastChanged = this.LastChanged,
			};

			if (string.IsNullOrEmpty(this.AffiliateProductTagId) == false)
			{
				model.AffiliateProductTagId = int.Parse(this.AffiliateProductTagId);
			}

			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate(string actionStatus)
		{
			var errorMessage = string.Empty;

			if (actionStatus == Constants.ACTION_STATUS_INSERT)
			{
				errorMessage = Validator.Validate("AffiliateProductTagRegist", this.DataSource);
			}
			else
			{
				errorMessage = Validator.Validate("AffiliateProductTagModify", this.DataSource);
			}

			return errorMessage;
		}

		#region プロパティ
		/// <summary>アフィリエイト商品タグID</summary>
		public string AffiliateProductTagId
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_AFFILIATE_PRODUCT_TAG_ID]; }
			set { this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_AFFILIATE_PRODUCT_TAG_ID] = value; }
		}
		/// <summary>タグ名称</summary>
		public string TagName
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_NAME]; }
			set { this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_NAME] = value; }
		}
		/// <summary>タグ内容</summary>
		public string TagContent
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_CONTENT]; }
			set { this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_CONTENT] = value; }
		}
		/// <summary>区切り文字</summary>
		public string TagDelimiter
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_DELIMITER]; }
			set { this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_DELIMITER] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}