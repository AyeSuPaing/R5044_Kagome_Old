/*
=========================================================================================================
  Module      : 商品一覧を設定するモーダルビューモデル(ProductSettingModalModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web.Mvc;
using w2.Domain.Product.Helper;
using w2.Domain.ProductGroup;

namespace w2.Cms.Manager.ViewModels.Shared
{
	/// <summary>
	/// 商品一覧を設定するモーダルビューモデル
	/// </summary>
	public class ProductSettingModalModel
	{
		const string SET_PRODUCT_CONTROLLER_NAME_DEFAULT = "SetProductList";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="controller">コントローラー</param>
		public ProductSettingModalModel(string shopId, string controller)
		{
			this.Controller = controller;
			this.ProductGroup = new ProductGroupService().GetAllProductGroup();

			this.UseGroupSelectTab = true;
			this.UseProductListInputTab = true;
			this.IsVariationList = false;
			this.SetProductControllerActionName = ProductSettingModalModel.SET_PRODUCT_CONTROLLER_NAME_DEFAULT;
			this.ExtraId = string.Empty;
		}

		/// <summary>
		/// デフォルトドロップダウンアイテム
		/// </summary>
		/// <returns></returns>
		public SelectListItem CreateDefaultSelectListItem()
		{
			return new SelectListItem
			{
				Value = string.Empty,
				Text = string.Empty,
				Selected = true,
			};
		}

		/// <summary>コントローラー</summary>
		public string Controller { get; set; }
		/// <summary>商品検索パラメタモデル</summary>
		public ProductSearchParamModel ParamModel { get; set; }
		/// <summary>商品グループ</summary>
		public ProductGroupModel[] ProductGroup { get; set; }
		/// <summary>商品グループ選択タブを使うかどうか（デフォルト：True）</summary>
		public bool UseGroupSelectTab { get; set; }
		/// <summary>商品IDリスト入力タブを使うかどうか（デフォルト：True）</summary>
		public bool UseProductListInputTab { get; set; }
		/// <summary>商品一覧がバリエーションかどうか（デフォルト：False）</summary>
		public bool IsVariationList { get; set; }
		/// <summary>商品設定の際のコントローラーのアクション名（デフォルト：SetProductList）</summary>
		public string SetProductControllerActionName { get; set; }
		/// <summary>追加するID</summary>
		public string ExtraId { get; set; }
		/// <summary>DivタグのID</summary>
		public string DivId
		{
			get { return "modal-item-list" + this.ExtraId; }
		}
	}
}