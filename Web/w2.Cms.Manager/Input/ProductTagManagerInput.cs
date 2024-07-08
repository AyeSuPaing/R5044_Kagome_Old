/*
=========================================================================================================
  Module      : 商品タグマネージャー Input(ProductTagManagerInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Input;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Domain.Affiliate;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// 商品タグマネージャー Input
	/// </summary>
	public class ProductTagManagerInput : InputBase<AffiliateProductTagSettingModel>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductTagManagerInput()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">変換モデル</param>
		public ProductTagManagerInput(AffiliateProductTagSettingModel model)
			: this()
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
		/// <returns>商品タグモデル</returns>
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
		public string Validate(ActionStatus actionStatus)
		{
			var errorMessage = Validator.Validate((actionStatus == ActionStatus.Insert)
				? "ProductTagManagerRegister"
				: "ProductTagManagerModify"
					, this.DataSource);

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
		/// <summary>更新前 タグ名称</summary>
		public string BeforeTagName { get; set; }
		/// <summary>更新前 タグ内容</summary>
		public string BeforeTagContent { get; set; }
		/// <summary>更新前 区切り文字</summary>
		public string BeforeTagDelimiter { get; set; }
		#endregion
	}
}