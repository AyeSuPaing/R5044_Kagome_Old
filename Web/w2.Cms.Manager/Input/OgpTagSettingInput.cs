/*
=========================================================================================================
  Module      : OGPタグ設定入力クラス (OgpTagSettingInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes.Common;
using w2.Domain.OgpTagSetting;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// OGPタグ設定入力クラス
	/// </summary>
	public class OgpTagSettingInput : InputBase<OgpTagSettingModel>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OgpTagSettingInput()
		{
			this.DataKbn = string.Empty;
			this.SiteTitle = string.Empty;
			this.PageTitle = string.Empty;
			this.Description = string.Empty;
			this.ImageUrl = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public OgpTagSettingInput(OgpTagSettingModel model)
			: this()
		{
			if (model == null) return;

			this.DataKbn = model.DataKbn;
			this.SiteTitle = model.SiteTitle;
			this.PageTitle = model.PageTitle;
			this.Description = model.Description;
			this.ImageUrl = model.ImageUrl;
			this.LastChanged = model.LastChanged;
		}

		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override OgpTagSettingModel CreateModel()
		{
			var model = new OgpTagSettingModel(this.DataSource);
			return model;
		}

		/// <summary>
		/// 入力検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate()
		{
			var message = Validator.Validate("OgpTagSettingModify", this.DataSource);
			return message;
		}

		/// <summary>データ区分</summary>
		public string DataKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_OGPTAGSETTING_DATA_KBN]; }
			set { this.DataSource[Constants.FIELD_OGPTAGSETTING_DATA_KBN] = value; }
		}
		/// <summary>サイト名</summary>
		public string SiteTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_OGPTAGSETTING_SITE_TITLE]; }
			set { this.DataSource[Constants.FIELD_OGPTAGSETTING_SITE_TITLE] = value; }
		}
		/// <summary>ページ名</summary>
		public string PageTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_OGPTAGSETTING_PAGE_TITLE]; }
			set { this.DataSource[Constants.FIELD_OGPTAGSETTING_PAGE_TITLE] = value; }
		}
		/// <summary>ディスクリプション</summary>
		public string Description
		{
			get { return (string)this.DataSource[Constants.FIELD_OGPTAGSETTING_DESCRIPTION]; }
			set { this.DataSource[Constants.FIELD_OGPTAGSETTING_DESCRIPTION] = value; }
		}
		/// <summary>画像URL</summary>
		public string ImageUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_OGPTAGSETTING_IMAGE_URL]; }
			set { this.DataSource[Constants.FIELD_OGPTAGSETTING_IMAGE_URL] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_OGPTAGSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_OGPTAGSETTING_LAST_CHANGED] = value; }
		}
		/// <summary>画像インプット</summary>
		public ImageInput ImageInput { get; set; }
	}
}