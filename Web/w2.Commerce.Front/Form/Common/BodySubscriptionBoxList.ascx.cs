/*
=========================================================================================================
  Module      : 頒布会コース一覧出力コントローラ処理(BodySubscriptionBoxList.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.SubscriptionBox;

public partial class Form_Common_BodySubscriptionBoxList : ProductUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		// 頒布会OP・定期購入OPが無効の場合は何もしない
		if ((Constants.FIXEDPURCHASE_OPTION_ENABLED == false) || (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED == false)) return;

		if (!IsPostBack)
		{
			// 頒布会コース一覧の取得
			var subscriptionBoxList = GetSubscriptionBoxCourseList();

			this.CourseListCount = subscriptionBoxList.Length;

			WrSubscriptionBoxCourseList.DataSource = subscriptionBoxList;
			WrSubscriptionBoxCourseList.DataBind();
		}
	}

	/// <summary>
	/// コースリストの取得
	/// </summary>
	/// <returns>コースリスト</returns>
	protected SubscriptionBoxModel[] GetSubscriptionBoxCourseList()
	{
		var cache = DataCacheControllerFacade.GetSubscriptionBoxCacheController();
		var subscriptionBoxList = cache.CacheData;

		var result = subscriptionBoxList.Where(
			course => 
			{
				switch (course.OrderItemDeterminationType)
				{
					case Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_PERIOD:
						return course.DefaultOrderProducts
							.Any(i => i.IsInTerm(DateTime.Now));

					case Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_NUMBER_TIME:
						var defaultItem = course.DefaultOrderProducts
							.Where(item => item.Count == 1);

						foreach (var dItem in defaultItem)
						{
							var isValid = course.SelectableProducts
								.Where(item => (item.VariationId == dItem.VariationId))
								.Any(item => item.IsInTerm(DateTime.Now));
							if (isValid) return true;
						}

						return false;

					default:
						return false;
				}
			}).ToArray();

		var displayCourseList = (Constants.DISP_LIST_CONTENTS_COUNT_SUBSCRIPTION_BOX_LIST != 0)
			? result.OrderBy(item => item.DisplayPriority)
				.Take(Constants.DISP_LIST_CONTENTS_COUNT_SUBSCRIPTION_BOX_LIST).ToArray()
			: result.OrderBy(item => item.DisplayPriority).ToArray();

		return displayCourseList;
	}

	/// <summary>
	/// 詳細ページURLの作成
	/// </summary>
	/// <param name="courseId">コースID</param>
	/// <returns>URL</returns>
	protected string CreateDetailUrl(string courseId)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_SUBSCRIPTIONBOX_DETAIL)
			.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID, courseId)
			.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_FOR_COURSE_LIST, Constants.REDIRECT_TO_SUBSCRIPTION_BOX_DETAIL_FROM_COURSE_LIST )
			.CreateUrl();

		return url;
	}

	/// <summary>コース表示件数</summary>
	protected int CourseListCount { get; set; }
	/// <summary>頒布会コースリスト</summary>
	WrappedRepeater WrSubscriptionBoxCourseList { get { return GetWrappedControl<WrappedRepeater>("rSubscriptionBoxCourseList"); } }
}