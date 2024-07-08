/*
=========================================================================================================
  Module      : 店舗管理者Listビューモデル(ListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.ParamModels.ShopOperator;
using w2.Domain.ShopOperator.Helper;

namespace w2.Cms.Manager.ViewModels.ShopOperator
{
	/// <summary>
	/// 店舗管理者Listビューモデル
	/// </summary>
	public class ListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="menus">MPオペレータ情報</param>
		public ListViewModel(List<ListItem> menus)
		{
			this.List = new ShopOperatorListSearchResult[0];
			this.SortKbnItems = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_SHOPOPERATOR, "sort")
				.Select(s => new SelectListItem
				{
					Value = s.Value,
					Text = s.Text,
					Selected = (s.Value == "1"),
				}).ToArray();
			this.ValidItems = ValueTextForCms.GetValueSelectListItems(
				Constants.TABLE_SHOPOPERATOR,
				Constants.FIELD_SHOPOPERATOR_VALID_FLG);
			this.MenuAccessLevelItem = ValueTextForCms.GetValueSelectListItems(
				Constants.TABLE_MENUAUTHORITY,
				Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_NAME);
			if (menus != null) { 
				this.MenuAccessLevelItem = menus.Select(
					s => new SelectListItem
					{
						Value = s.Value,
						Text = s.Text,
					}).ToArray();
			}
		}

		/// <summary>パラメタモデル</summary>
		public ListParamModel ParamModel { get; set; }
		/// <summary>選択肢群 並び順</summary>
		public SelectListItem[] SortKbnItems { get; private set; }
		/// <summary>選択肢群 有効フラグ</summary>
		public SelectListItem[] ValidItems { get; private set; }
		/// <summary>選択肢群 メニュー権限</summary>
		public SelectListItem[] MenuAccessLevelItem { get; private set; }
		/// <summary>ページャHTML</summary>
		public string PagerHtml { get; set; }
		/// <summary>リスト一覧</summary>
		public ShopOperatorListSearchResult[] List { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}
}