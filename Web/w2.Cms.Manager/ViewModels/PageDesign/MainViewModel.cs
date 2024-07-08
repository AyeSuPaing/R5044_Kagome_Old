/*
=========================================================================================================
  Module      : ページ管理 メインビューモデル(MainViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.PageDesign;
using w2.Cms.Manager.ParamModels.PageDesign;
using w2.Common.Util;
using w2.Domain.PageDesign;

namespace w2.Cms.Manager.ViewModels.PageDesign
{
	/// <summary>
	/// ページ管理 メインビューモデル
	/// </summary>
	public class MainViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainViewModel()
		{
			this.ParamModel = new PageDesignListSearchParamModel();

			var defaultGroup = new[] 
			{
				new SelectListItem
				{
					Value = string.Empty,
					Text = string.Empty
				}
			};

			this.GroupItems = defaultGroup.Concat(
				new PageDesignService().GetAllGroup().Select(
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
		public PageDesignListSearchParamModel ParamModel { get; set; }
		/// <summary>グループリスト</summary>
		public SelectListItem[] GroupItems { get; private set; }
		/// <summary>ページ利用状態</summary>
		public SelectListItem[] UseTypes { get; private set; }
	}
}