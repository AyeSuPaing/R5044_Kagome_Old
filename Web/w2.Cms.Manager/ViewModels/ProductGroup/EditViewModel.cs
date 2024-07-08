/*
=========================================================================================================
  Module      : 商品グループ編集登録ビューモデル (EditViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Web.Mvc;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Input;
using w2.Common.Web;
using w2.Domain.ProductGroup;

namespace w2.Cms.Manager.ViewModels.ProductGroup
{
	public class EditViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public EditViewModel()
		{
			Init();
			this.PageLayout = Constants.LAYOUT_PATH_DEFAULT;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public EditViewModel(ProductGroupModel model)
			: this()
		{
			this.Input = new ProductGroupInput(model);
			this.URL = this.CreateProductListUrlForProductGroup(model);
		}

		/// <summary>
		/// 商品グループページURL作成
		/// </summary>
		/// <param name="model">商品グループモデル</param>
		/// <returns>商品グループページURL</returns>
		private string CreateProductListUrlForProductGroup(ProductGroupModel model)
		{
			var url = new UrlCreator(Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC + Constants.PAGE_FRONT_PRODUCT_LIST)
				.AddParam(Constants.REQUEST_KEY_PRODUCT_GROUP_ID, model.ProductGroupId)
				.AddParam(Constants.REQUEST_KEY_SORT_KBN, Constants.KBN_SORT_PRODUCT_LIST_PRODUCT_GROUP_ITEM_NO_ASC)
				.CreateUrl();
			return url;
		}

		/// <summary>
		/// 初期設定
		/// </summary>
		private void Init()
		{
			this.Input = new ProductGroupInput();

			this.BeginDateYearItems = DateTimeUtility.GetShortRangeYearListItem()
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.BeginDateMonthItems = DateTimeUtility.GetMonthListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.BeginDateDayItems = DateTimeUtility.GetDayListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.BeginDateHourItems = DateTimeUtility.GetHourListItem("00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.BeginDateMinuteItems = DateTimeUtility.GetMinuteListItem("00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.BeginDateSecondItems = DateTimeUtility.GetSecondListItem("00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();

			this.EndDateYearItems = DateTimeUtility.GetShortRangeYearListItem()
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.EndDateMonthItems = DateTimeUtility.GetMonthListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.EndDateDayItems = DateTimeUtility.GetDayListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.EndDateHourItems = DateTimeUtility.GetHourListItem("00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.EndDateMinuteItems = DateTimeUtility.GetMinuteListItem("00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.EndDateSecondItems = DateTimeUtility.GetSecondListItem("00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();

			this.ErrorMessage = "";
			this.URL = "";
			this.UpdateInsertSuccessFlg = false;
		}

		/// <summary>入力内容</summary>
		public ProductGroupInput Input { get; private set; }
		/// <summary>更新・登録 成功フラグ</summary>
		public bool UpdateInsertSuccessFlg { get; set; }
		/// <summary>選択肢群 開始年</summary>
		public SelectListItem[] BeginDateYearItems { get; private set; }
		/// <summary>選択肢群 開始月</summary>
		public SelectListItem[] BeginDateMonthItems { get; private set; }
		/// <summary>選択肢群 開始日</summary>
		public SelectListItem[] BeginDateDayItems { get; private set; }
		/// <summary>選択肢群 開始時</summary>
		public SelectListItem[] BeginDateHourItems { get; private set; }
		/// <summary>選択肢群 開始分</summary>
		public SelectListItem[] BeginDateMinuteItems { get; private set; }
		/// <summary>選択肢群 開始秒</summary>
		public SelectListItem[] BeginDateSecondItems { get; private set; }
		/// <summary>選択肢群 終了年</summary>
		public SelectListItem[] EndDateYearItems { get; private set; }
		/// <summary>選択肢群 終了月</summary>
		public SelectListItem[] EndDateMonthItems { get; private set; }
		/// <summary>選択肢群 終了日</summary>
		public SelectListItem[] EndDateDayItems { get; private set; }
		/// <summary>選択肢群 終了時</summary>
		public SelectListItem[] EndDateHourItems { get; private set; }
		/// <summary>選択肢群 終了分</summary>
		public SelectListItem[] EndDateMinuteItems { get; private set; }
		/// <summary>選択肢群 終了秒</summary>
		public SelectListItem[] EndDateSecondItems { get; private set; }
		/// <summary>URL</summary>
		public string URL { get; private set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
		/// <summary>ページレイアウト</summary>
		public string PageLayout { get; set; }
	}
}