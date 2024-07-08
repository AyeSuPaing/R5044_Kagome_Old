/*
=========================================================================================================
  Module      : パーツ管理 メインビューモデル(MainViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Web.Mvc;
using w2.App.Common.Design;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.PageDesign;
using w2.Cms.Manager.ParamModels.PartsDesign;
using w2.Common.Util;
using w2.Domain.PartsDesign;

namespace w2.Cms.Manager.ViewModels.PartsDesign
{
	/// <summary>
	/// パーツ管理 メインビューモデル
	/// </summary>
	public class MainViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainViewModel()
		{
			this.ParamModel = new PartsDesignListSearchParamModel();

			var defaultGroup = new[]
			{
				new SelectListItem
				{
					Value = string.Empty,
					Text = string.Empty
				}
			};

			this.StandardPartsItems = PartsDesignUtility.GetValueItemListStandardParts()
				.Where(s => PartsDesignCommon.ValidParts(s.Value))
				.Select(
				s => new SelectListItem
				{
					Value = s.Value,
					Text = s.Text
				}).ToArray();

			this.GroupItems = defaultGroup.Concat(
				new PartsDesignService().GetAllGroup().Select(
					g => new SelectListItem
					{
						Value = g.GroupId.ToString(),
						Text = g.GroupName
					})).ToArray();


			var otherGroup = PageDesignUtility.OtherPageGroupModel;
			this.GroupItems = this.GroupItems.Concat(
				new[]
				{
					new SelectListItem
					{
						Value = otherGroup.GroupId.ToString(),
						Text = otherGroup.GroupName
					}
				}).ToArray();

			this.UseTypes = ValueText.GetValueItemArray(Constants.VALUE_TEXT_KEY_CMS_COMMON, Constants.VALUE_TEXT_FIELD_USE_TYPE).Select(
				item => new SelectListItem()
				{
					Text = item.Text,
					Value = item.Value
				}).ToArray();
		}

		/// <summary>検索パラメータ</summary>
		public PartsDesignListSearchParamModel ParamModel { get; set; }
		/// <summary>グループリスト</summary>
		public SelectListItem[] GroupItems { get; private set; }
		/// <summary>テンプレートファイル一覧</summary>
		public SelectListItem[] StandardPartsItems { get; set; }
		/// <summary>ページ利用状態</summary>
		public SelectListItem[] UseTypes { get; private set; }
	}
}