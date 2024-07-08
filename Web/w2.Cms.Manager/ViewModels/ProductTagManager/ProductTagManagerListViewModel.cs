/*
=========================================================================================================
  Module      : 商品タグマネージャー リストViewモデル(ProductTagManagerListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Affiliate;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.ProductTagManager;
using w2.Common.Util;
using w2.Domain.Affiliate;

namespace w2.Cms.Manager.ViewModels.ProductTagManager
{
	/// <summary>
	///  商品タグマネージャー リストViewモデル
	/// </summary>
	public class ProductTagManagerListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductTagManagerListViewModel()
		{
			this.ParamModel = new ProductTagManagerListParamModel();
			this.ModifyInputs = new ProductTagManagerInput[0];
			this.RegisterInput = new ProductTagManagerInput();
			this.ReplaseTagDescriptionList = new ReplaceTagManager()
				.SelectPageReplaceTagList(TagSetting.ACTION_TYPE_CART_PRODUCT)
				.Select(m => new ListItem
				{
					Value = m,
					Text = ValueText.GetValueText("AffiliateTag", "ReplaseTagDescription", m)
				}).ToArray();
		}

		/// <summary>検索 パラメタモデル</summary>
		public ProductTagManagerListParamModel ParamModel { get; set; }
		/// <summary>一括更新 入力内容モデル</summary>
		public ProductTagManagerInput[] ModifyInputs { get; set; }
		/// <summary>登録 入力内容モデル</summary>
		public ProductTagManagerInput RegisterInput { get; set; }
		/// <summary>登録・更新確認用モデル</summary>
		public AffiliateProductTagSettingModel[] ConformModel { get; set; }
		/// <summary>置換タグリスト</summary>
		public ListItem[] ReplaseTagDescriptionList { get; set; }
		/// <summary>ページャHTML</summary>
		public string PagerHtml { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}
}