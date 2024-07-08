/*
=========================================================================================================
  Module      : 対象LPページ設定モーダルビューモデル(LandingPageSettingModalModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.LandingPage.Helper;

namespace w2.Cms.Manager.ViewModels.AbTest
{
	/// <summary>
	/// 対象LPページ設定モーダルビューモデル
	/// </summary>
	public class LandingPageSettingModalModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="controller">コントローラー</param>
		public LandingPageSettingModalModel(string controller)
		{
			this.Controller = controller;
			this.ExtraId = string.Empty;
		}

		/// <summary>コントローラー</summary>
		public string Controller { get; set; }
		/// <summary>LPページ検索パラメタモデル</summary>
		public LandingPageSearchParamModel ParamModel { get; set; }
		/// <summary>追加するID</summary>
		public string ExtraId { get; set; }
		/// <summary>DivタグのID</summary>
		public string DivId
		{
			get { return "modal-item-list" + this.ExtraId; }
		}
	}
}