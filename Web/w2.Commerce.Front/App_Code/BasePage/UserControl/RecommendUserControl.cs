/*
=========================================================================================================
  Module      : レコメンド用基底ユーザコントロール(RecommendUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.Domain.Recommend;
using w2.App.Common.Order;
using w2.App.Common.Recommend;

/// <summary>
/// レコメンド用基底ユーザーコントロール
/// </summary>
public class RecommendUserControl : BaseUserControl
{
	#region メソッド
	/// <summary>
	/// レコメンド設定セット
	/// </summary>
	/// <param name="targetCartList">レコメンド商品投入対象カート</param>
	/// <param name="currentCartList">カートオブジェクトリスト</param>
	/// <param name="orderUserId">注文者ユーザーID</param>
	/// <param name="buttonId">商品投入ボタンID</param>
	/// <param name="displayPage">レコメンド表示ページ</param>
	/// <returns>レコメンドモデル</returns>
	protected RecommendModel SetRecommend(
		CartObject[] targetCartList,
		CartObjectList currentCartList,
		string orderUserId,
		string buttonId,
		string displayPage = Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_PAGE_ORDER_CONFIRM)
	{
		// 条件に一致するレコメンド設定を抽出
		var recommend = new RecommendExtractor(orderUserId, this.LoginUserMemberRankId, currentCartList.Items.ToArray())
			.Exec(targetCartList, displayPage);
		// レコメンド設定が存在しなければ処理を抜ける
		if (recommend == null) return null;
		this.Recommend = recommend;

		// レコメンド表示をセット
		this.RecommendDisplay =
			new RecommendDisplayConverter(recommend, buttonId, this.IsSmartPhone).ExecAndGetRecommendHtml();

		// レコメンド設定適用履歴登録
		this.RecommendHistoryNo = new RecommendService().GetNewRecommendHistoryNoAndInsertRecommendHistory(new RecommendHistoryModel
		{
			ShopId = recommend.ShopId,
			RecommendId = recommend.RecommendId,
			UserId = orderUserId,
			TargetOrderId = targetCartList.First().OrderId ?? string.Empty,
			DisplayKbn = Constants.FLG_RECOMMENDHISTORY_DISPLAY_KBN_FRONT,
			OrderedFlg = Constants.FLG_RECOMMENDHISTORY_ORDERED_FLG_DISP,
			LastChanged = Constants.FLG_LASTCHANGED_USER
		});

		return recommend;
	}
	#endregion

	#region プロパティ
	/// <summary>レコメンド設定</summary>
	protected RecommendModel Recommend
	{
		get { return (RecommendModel)ViewState["Recommend"]; }
		set { ViewState["Recommend"] = value; }
	}
	/// <summary>レコメンド表示</summary>
	protected string RecommendDisplay
	{
		get { return StringUtility.ToEmpty(ViewState["RecommendDisplay"]); }
		set { ViewState["RecommendDisplay"] = value; }
	}
	/// <summary>レコメンド表示履歴枝番</summary>
	protected int RecommendHistoryNo
	{
		get { return (int)ViewState["RecommendHistoryNo"]; }
		set { ViewState["RecommendHistoryNo"] = value; }
	}
	#endregion
}