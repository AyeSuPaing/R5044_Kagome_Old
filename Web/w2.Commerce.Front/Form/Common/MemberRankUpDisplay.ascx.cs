/*
=========================================================================================================
  Module      : 会員ランクアップ情報出力コントローラ処理(MemberRankUpDisplay.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using w2.App.Common.Option;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.MemberRank;
using w2.Domain.MemberRankRule;
using w2.Domain.Order;
using w2.Domain.UserMemberRankHistory;

/// <summary>
/// 会員ランクアップ情報出力コントローラ処理
/// </summary>
public partial class Form_Common_MemberRankUpDisplay : BaseUserControl
{
	/// <summary>マイナスを含む受注商品数（返品交換受注を考慮）</summary>
	private const string COUNT_FIELD_ORDER_TOTAL_ITEM_QUANTITY = "total_item_quantity";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, EventArgs e)
	{
		if ((Constants.MEMBER_RANK_OPTION_ENABLED == false) || (Constants.MYPAGE_MEMBERRANKUP_DISPLAY == false)) return;

		CreateDispUntilRankUps();
		DisplayUntilRankUp();
	}

	/// <summary>
	/// 表示用クラスを作成
	/// </summary>
	private void CreateDispUntilRankUps()
	{
		this.MemberRankUps = GetMemberRankUpList();
		if (this.MemberRankUps != null)
		{
			this.MemberRankUpRules = GetMemberRankUpRuleList();

			if (this.MemberRankUpRules != null)
			{
				GetOrderHistoryList();

				this.DispUntilRankUps = this.MemberRankUpRules.Select(rule => new DispUntilRankUp(rule)).ToArray();
				foreach (var dispClass in this.DispUntilRankUps.OrderBy(rule => rule.ExacScheduleDate))
				{
					GetTotalPriceAndCount(dispClass);
					GetTotalDifference(
						dispClass,
						this.MemberRankUpRules.First(rule => dispClass.MemberRankRuleId == rule.MemberRankRuleId));
					dispClass.NextMemberRankName = MemberRankOptionUtility.GetMemberRankName(dispClass.NextMemberRankId);
					dispClass.NextMemberRankOrder = MemberRankOptionUtility.GetMemberRankNo(dispClass.NextMemberRankId);
				}

				// ランクアップが確定したルールが存在する場合、それ以外の同一ランクルールを除外
				if (this.RankUpConfirmFlag)
				{
					this.DispUntilRankUps = this.DispUntilRankUps.GroupBy(group => group.NextMemberRankId).SelectMany(
						group => group.OrderByDescending(g => g.RankUpConfirmFlag).First(
								rule => rule.RankUpConfirmFlag
									? rule.RankUpConfirmFlag
									: rule.RankUpConfirmFlag == false)
							.RankUpConfirmFlag
							? group.Where(rule => rule.RankUpConfirmFlag)
							: group.Where(rule => rule.RankUpConfirmFlag == false)).ToArray();
				}
				this.DispUntilRankUps = AssignOrFlag(this.DispUntilRankUps);
			}
		}
	}

	/// <summary>
	/// 画面表示
	/// </summary>
	private void DisplayUntilRankUp()
	{
		if (this.DispUntilRankUps.Length > 0)
		{
			switch (this.DispStandard)
			{
				case 0:
					this.DispUntilRankUps = this.DispUntilRankUps.OrderByDescending(rule => rule.NextMemberRankOrder).ToArray();
					break;
				case 1:
					this.DispUntilRankUps = this.DispUntilRankUps.OrderBy(rule => rule.AggregationPeriodStart)
						.ThenByDescending(rule => rule.NextMemberRankOrder).ToArray();
					break;
				default:
					this.DispUntilRankUps = this.DispUntilRankUps.OrderByDescending(rule => rule.NextMemberRankOrder).ToArray();
					break;
			}
			this.WrMemberRankUpDisplay.DataSource = this.DispUntilRankUps;
			this.WrMemberRankUpDisplay.DataBind();
		}
	}

	/// <summary>
	/// ランクアップ先の会員ランクリストを取得
	/// </summary>
	/// <returns>会員ランクのリスト</returns>
	private MemberRankModel[] GetMemberRankUpList()
	{
		var currentMemberOrder = MemberRankOptionUtility.GetMemberRankNo(this.LoginUserMemberRankId);

		// 有効フラグが立っている, ランク順位が現在ランクより上位である, 降順(現在ランクに近い順)で取得
		var memberRankUpList = MemberRankOptionUtility.GetMemberRankList()
			.Where(
				(memberRank => (memberRank.ValidFlg == Constants.FLG_MEMBERRANK_VALID_FLG_VALID)
					&& ((memberRank.MemberRankOrder - currentMemberOrder) < 0)))
			.OrderByDescending(memberRank => memberRank.MemberRankOrder).ToArray();
		return memberRankUpList;
	}
	
	/// <summary>
	/// ランクアップ先の会員ランク変動ルールのリストを取得
	/// </summary>
	/// <returns>会員ランク変動ルールのリスト</returns>
	private MemberRankRuleModel[] GetMemberRankUpRuleList()
	{
		var memberRankRules = MemberRankOptionUtility.GetMemberRankRuleList();
		var rankUpDestinationRankId = this.MemberRankUps.Select(m => m.MemberRankId).ToArray();
		var mergeList = new ArrayList();

		// 「有効フラグ」が立っている
		// かつ「ランク付与方法」がランクアップ
		// かつ「実行タイミング」が「スケジュール実行」
		// かつ「合計購入金額」または「合計購入回数」のどちらかに下限値がある
		// かつ「スケジュール実行」が「１回のみ」でない（「日単位」「週単位」「月単位」のいずれか）または、「１回のみ」かつ指定日が未来日
		// を満たす会員ランク変動ルールを抽出
		var rankUpRules = memberRankRules.Where(
			rule => ((rule.ValidFlg == Constants.FLG_MEMBERRANKRULE_VALID_FLG_VALID)
				&& (rule.RankChangeType == Constants.FLG_MEMBERRANKRULE_RANK_CHANGE_TYPE_UP)
				&& (rule.ExecTiming == Constants.FLG_MEMBERRANKRULE_EXEC_TIMING_SCHEDULE)
				&& ((rule.TargetExtractTotalPriceFrom != null) || (rule.TargetExtractTotalCountFrom != null))
				&& ((rule.ScheduleKbn != Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN__ONCE)
					|| ((Constants.MYPAGE_SCHEDULE_KBN_ONCE_MEMBERRANKUP_DISPLAY)
						&& (rule.ScheduleKbn == Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN__ONCE)
						&& (rule.IsFutureDate(rule.GetExecScheduleTime())))))).ToArray();

		foreach (var rankUpId in rankUpDestinationRankId)
		{
			var rankUpRuleOfRankId = rankUpRules.Where(rule => rule.RankChangeRankId == rankUpId).ToArray();
			if (rankUpRuleOfRankId.Length > 0)
			{
				foreach (var rule in rankUpRuleOfRankId)
				{
					if (rule.TargetExtractOldRankFlg == Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG_OFF)
					{
						if (this.ChangeRankDate == null)
						{
							GetChangedRankDate();
						}
						if (this.ChangeRankDate != null)
						{
							rule.ChangeRankDate = DateTime.Parse(this.ChangeRankDate.ToString());
						}
					}
					rule.SynthesizeExecSchedule();
					rule.CalcTargetExtract();
				}
				rankUpRuleOfRankId = rankUpRuleOfRankId
					.Where(rule => rule.AdjustedTargetExtractStart <= rule.AdjustedTargetExtractEnd).ToArray();

				if (rankUpRuleOfRankId.Length > 1)
				{
					// 条件が購入金額のみ（最小の閾値を抽出）
					var verifiedOnlyPriceList = rankUpRuleOfRankId.Where(rule => rule.TargetExtractTotalCountFrom == null)
						.GroupBy(
							rule => new
							{
								Start = rule.AdjustedTargetExtractStart.Date,
								End = rule.AdjustedTargetExtractEnd.Date
							}).Select(
							group => group.Count() > 1
								? group.OrderBy(g => g.TargetExtractTotalPriceFrom).First()
								: group.First()).ToArray();

					// 条件が購入回数のみ（最小の閾値を抽出）
					var verifiedOnlyCountList = rankUpRuleOfRankId.Where(rule => rule.TargetExtractTotalPriceFrom == null)
						.GroupBy(
							rule => new
							{
								Start = rule.AdjustedTargetExtractStart.Date,
								End = rule.AdjustedTargetExtractEnd.Date
							}).Select(
							group => group.Count() > 1
								? group.OrderBy(g => g.TargetExtractTotalCountFrom).First()
								: group.First()).ToArray();

					// 条件が購入金額と購入回数の組み合わせ（最小の閾値の組み合わせを抽出）
					// ※50000円＆5回、60000円＆6回の組み合わせでは前者のみ抽出
					// ※50000円＆5回、40000円＆6回の組み合わせではどちらも抽出
					// ※重複している組み合わせはランク付与ルールIDが若いものを抽出
					var verifiedBothPriceAndCountList = rankUpRuleOfRankId.Where(
						rule => (rule.TargetExtractTotalPriceFrom != null)
							&& (rule.TargetExtractTotalCountFrom != null)).GroupBy(
						rule => new
						{
							Start = rule.AdjustedTargetExtractStart.Date,
							End = rule.AdjustedTargetExtractEnd.Date
						}).SelectMany(
						group =>
						{
							if (group.Count() == 1) return new[] { group.First() };
							var limit = group.First();
							foreach (var list in group.OrderByDescending(rule => rule.MemberRankRuleId))
							{
								if ((limit.TargetExtractTotalPriceFrom >= list.TargetExtractTotalPriceFrom)
									&& (limit.TargetExtractTotalCountFrom >= list.TargetExtractTotalCountFrom))
								{
									limit = list;
								}
							}
							return group.Where(
								list => (list.TargetExtractTotalCountFrom < limit.TargetExtractTotalCountFrom)
									|| (list.TargetExtractTotalPriceFrom < limit.TargetExtractTotalPriceFrom)
									|| (list == limit));
						}).ToArray();

					mergeList.AddRange(verifiedOnlyPriceList);
					mergeList.AddRange(verifiedOnlyCountList);
					mergeList.AddRange(verifiedBothPriceAndCountList);
				}
				else
				{
					if (rankUpRuleOfRankId.Length == 0) continue;

					mergeList.Add(rankUpRuleOfRankId.First());
				}
			}
			if (this.DispAllHigherRankFlag == false) break;
		}
		var rankUpRule = mergeList
			.Cast<MemberRankRuleModel>()
			.OrderBy(rule => rule.AdjustedTargetExtractStart)
			.ThenBy(rule => rule.AdjustedTargetExtractEnd).ToArray();
		return rankUpRule;
	}

	/// <summary>
	/// 現在の会員ランクになった日を取得
	/// </summary>
	private void GetChangedRankDate()
	{
		var history = new UserMemberRankHistoryService().GetByUserId(this.LoginUserId);
		if (history != null)
		{
			this.ChangeRankDate = history.DateCreated;
		}
	}

	/// <summary>
	/// ユーザの注文履歴を取得
	/// </summary>
	private void GetOrderHistoryList()
	{
		this.OrderHistoryList = new OrderService().GetOrderHistoryList(this.LoginUserId);
	}

	/// <summary>
	/// 現在の合計購入金額と合計購入回数を取得
	/// </summary>
	/// <param name="dispClass">ランクアップ表示用クラス</param>
	private void GetTotalPriceAndCount(DispUntilRankUp dispClass)
	{
		if (this.OrderHistoryList != null)
		{
			var targetOrgOrders = this.OrderHistoryList
				.Where(order =>
					((dispClass.AggregationPeriodStart.Date.CompareTo(order.OrderShippedDate) <= 0)
						&& (dispClass.AggregationPeriodEnd.Date.CompareTo(order.OrderShippedDate) >= 0))
					|| ((order.OrderShippedDate == null)
						&& (dispClass.AggregationPeriodStart.Date.CompareTo(order.OrderDeliveringDate) <= 0)
						&& (dispClass.AggregationPeriodEnd.Date.CompareTo(order.OrderDeliveringDate) >= 0)))
				.ToArray();
			var orderIdLists = targetOrgOrders.Select(order => order.OrderId);
			var targetReturnAndExchangeOrders = this.OrderHistoryList
				.Where(order => orderIdLists.Contains(order.OrderIdOrg))
				.ToArray();
			var targetGroupOrders = targetOrgOrders.Concat(targetReturnAndExchangeOrders)
				.Select(order =>
				{
					if (string.IsNullOrEmpty(order.OrderIdOrg) == false)
					{
						order.OrderId = order.OrderIdOrg;
					}
					return order;
				})
				.GroupBy(order => order.OrderId)
				.ToArray();
			dispClass.CurrentTotalPrice = (dispClass.CalcPriceFlag)
				? targetGroupOrders.Sum(group => group.Sum(order => order.OrderPriceTotal))
				: (decimal?)null;

			dispClass.CurrentTotalCount = (decimal?)null;
			if (dispClass.CalcCountFlag)
			{
				var orderForTotalCount = targetGroupOrders
					.Where(group => group.Sum(order => (int)order.DataSource[COUNT_FIELD_ORDER_TOTAL_ITEM_QUANTITY]) > 0)
					.ToArray();
				dispClass.CurrentTotalCount = orderForTotalCount.Length;
			}
		}
	}

	/// <summary>
	/// 差分を取得
	/// </summary>
	/// <param name="dispClass">ランクアップ表示用クラス</param>
	/// <param name="model">会員ランク変動ルールモデル</param>
	private void GetTotalDifference(DispUntilRankUp dispClass, MemberRankRuleModel model)
	{
		if ((dispClass.CalcPriceFlag)
			&& (dispClass.CurrentTotalPrice != null)
			&& (model.TargetExtractTotalPriceFrom != null))
		{
			var difPrice = (int)(model.TargetExtractTotalPriceFrom - dispClass.CurrentTotalPrice);
			dispClass.DifferenceTotalPrice = (difPrice >= 0) ? difPrice : 0;
		}

		if ((dispClass.CalcCountFlag)
			&& (dispClass.CurrentTotalCount != null)
			&& (model.TargetExtractTotalCountFrom != null))
		{
			var difCount = (int)(model.TargetExtractTotalCountFrom - dispClass.CurrentTotalCount);
			dispClass.DifferenceTotalCount = (difCount >= 0) ? difCount : 0;
		}

		if (((dispClass.DifferenceTotalPrice == 0) && (dispClass.DifferenceTotalCount == 0))
			|| (dispClass.CalcPriceFlag == false) && (dispClass.DifferenceTotalCount == 0)
			|| (dispClass.CalcCountFlag == false) && (dispClass.DifferenceTotalPrice == 0))
		{
			dispClass.RankUpConfirmFlag = true;
			this.RankUpConfirmFlag = true;
		}
	}

	/// <summary>
	/// "または"の表示を検証
	/// </summary>
	/// <param name="dispClasses">表示用クラス</param>
	/// <returns>検証済み表示用クラス</returns>
	private DispUntilRankUp[] AssignOrFlag(DispUntilRankUp[] dispClasses)
	{
		var mergeFlag = false;
		DispUntilRankUp prevClass = null;
		foreach (var dispClass in dispClasses.OrderByDescending(rule=> rule.NextMemberRankOrder))
		{
			if ((prevClass != null) && (prevClass.NextMemberRankId != dispClass.NextMemberRankId))
			{
				prevClass = null;
			}

			// 集計期間が同じなら"または"フラグを立てる
			if ((prevClass != null) && (mergeFlag == false))
			{
				if ((prevClass.AggregationPeriodStart.Date.CompareTo(dispClass.AggregationPeriodStart.Date) == 0)
					&& (prevClass.AggregationPeriodEnd.Date.CompareTo(dispClass.AggregationPeriodEnd.Date) == 0)
					&& (dispClass.NextMemberRankId == prevClass.NextMemberRankId))
				{
					dispClass.DisplayOrFlag = true;
					continue;
				}
			}

			// 条件が購入金額のみなら統合フラグを立てる
			if ((dispClass.CalcCountFlag == false) && (mergeFlag == false))
			{
				prevClass = dispClass;
				mergeFlag = true;
				continue;
			}

			// 統合フラグが立っている、かつ集計期間が同一であれば"購入回数のみ"を"購入金額のみ"にマージ
			if ((prevClass != null) && (mergeFlag) && (dispClass.CalcPriceFlag == false))
			{
				if ((prevClass.AggregationPeriodStart.Date == dispClass.AggregationPeriodStart.Date)
					&& (prevClass.AggregationPeriodEnd.Date == dispClass.AggregationPeriodEnd.Date)
					&& (dispClass.NextMemberRankId == prevClass.NextMemberRankId))
				{
					prevClass.MergeOnlyFlag = true;
					prevClass.CalcCountFlag = true;
					prevClass.CurrentTotalCount = dispClass.CurrentTotalCount;
					prevClass.DifferenceTotalCount = dispClass.DifferenceTotalCount;
					prevClass.MergedSourceId = dispClass.MemberRankRuleId;
					// マージされた変動ルールはメソッドの最後で最終的に除外
					dispClass.CalcCountFlag = false;
				}

				prevClass.DisplayOrFlag = false;
				mergeFlag = false;
				continue;
			}

			mergeFlag = false;
			prevClass = dispClass;
		}

		// マージされた変動ルールを除外
		var assignedlist = dispClasses.Where(rule => (rule.CalcPriceFlag) || (rule.CalcCountFlag)).ToArray();
		return assignedlist;
	}

	#region ラップ済コントロール宣言
	private WrappedRepeater WrMemberRankUpDisplay { get { return GetWrappedControl<WrappedRepeater>("rMemberRankUpDisplay"); } }
	# endregion

	/// <summary>全上位ランク表示フラグ（外部から設定可能）</summary>
	public bool DispAllHigherRankFlag { set; get; }
	/// <summary>表示基準（0: ランク順位降順, 1: 集計期間昇順）（外部から設定可能）</summary>
	public int DispStandard { set; get; }
	/// <summary>現在の会員ランクになった日付</summary>
	private DateTime? ChangeRankDate { get; set; }
	/// <summary>ランクアップ先会員ランクリスト</summary>
	private MemberRankModel[] MemberRankUps { get; set; }
	/// <summary>ランクアップ先会員ランク変動ルールリスト</summary>
	private MemberRankRuleModel[] MemberRankUpRules { get; set; }
	/// <summary>ユーザ注文履歴情報</summary>
	private OrderModel[] OrderHistoryList { get; set; }
	/// <summary>ランクアップ表示用クラス配列</summary>
	private DispUntilRankUp[] DispUntilRankUps { get; set; }
	/// <summary>ランクアップ確定フラグ</summary>
	private bool RankUpConfirmFlag { get; set; }
}

/// <summary>
/// ランクアップ表示用クラス
/// </summary>
public class DispUntilRankUp
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public DispUntilRankUp()
	{
		this.MemberRankRuleId = null;
		this.MemberRankRuleName = null;
		this.NextMemberRankId = null;
		this.NextMemberRankName = null;
		this.NextMemberRankOrder = null;
		this.TargetExtractStart = null;
		this.TargetExtractEnd = null;
		this.AggregationPeriodStart = DateTime.MinValue;
		this.AggregationPeriodEnd = DateTime.MaxValue;
		this.CurrentTotalPrice = null;
		this.CurrentTotalCount = null;
		this.DifferenceTotalPrice = null;
		this.DifferenceTotalCount = null;
		this.ExacScheduleDate = null;
		this.MergeOnlyFlag = false;
		this.MergedSourceId = null;
		this.CalcPriceFlag = false;
		this.CalcCountFlag = false;
		this.RankUpConfirmFlag = false;
		this.DisplayOrFlag = false;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public DispUntilRankUp(MemberRankRuleModel model)
		: this()
	{
		this.MemberRankRuleId = model.MemberRankRuleId;
		this.MemberRankRuleName = model.MemberRankRuleName;
		this.NextMemberRankId = model.RankChangeRankId;
		this.TargetExtractStart = model.TargetExtractStart;
		this.TargetExtractEnd = model.TargetExtractEnd;
		this.AggregationPeriodStart = model.AdjustedTargetExtractStart;
		this.AggregationPeriodEnd = model.AdjustedTargetExtractEnd;
		this.ExacScheduleDate = model.ExacScheduleDate;
		this.CalcPriceFlag = (model.TargetExtractTotalPriceFrom != null);
		this.CalcCountFlag = (model.TargetExtractTotalCountFrom != null);
	}

	/// <summary>ランク付与ルールID</summary>
	public string MemberRankRuleId { get; set; }
	/// <summary>ランク付与ルール名</summary>
	public string MemberRankRuleName { get; set; }
	/// <summary>ランクアップ先ランクID</summary>
	public string NextMemberRankId { get; set; }
	/// <summary>ランクアップ先ランク名</summary>
	public string NextMemberRankName { get; set; }
	/// <summary>ランクアップ先ランク順位</summary>
	public int? NextMemberRankOrder { get; set; }
	/// <summary>集計期間開始日（管理画面設定値）</summary>
	public DateTime? TargetExtractStart { get; set; }
	/// <summary>集計期間終了日（管理画面設定値）</summary>
	public DateTime? TargetExtractEnd { get; set; }
	/// <summary>集計期間開始日（抽出条件反映後）</summary>
	public DateTime AggregationPeriodStart { get; set; }
	/// <summary>集計期間終了日（抽出条件反映後）</summary>
	public DateTime AggregationPeriodEnd { get; set; }
	/// <summary>集計期間内の現在の合計購入金額</summary>
	public decimal? CurrentTotalPrice { get; set; }
	/// <summary>集計期間内の現在の合計購入回数</summary>
	public decimal? CurrentTotalCount { get; set; }
	/// <summary>購入金額の組み合わせの差分</summary>
	public decimal? DifferenceTotalPrice { get; set; }
	/// <summary>購入回数の組み合わせの差分</summary>
	public decimal? DifferenceTotalCount { get; set; }
	/// <summary>スケジュール実行日</summary>
	public DateTime? ExacScheduleDate { get; set; }
	/// <summary>統合フラグ</summary>
	public bool MergeOnlyFlag { get; set; }
	/// <summary>統合元ランク付与ルールID</summary>
	public string MergedSourceId { get; set; }
	/// <summary>購入金額算出フラグ</summary>
	public bool CalcPriceFlag { get; set; }
	/// <summary>購入回数算出フラグ</summary>
	public bool CalcCountFlag { get; set; }
	/// <summary>ランクアップ確定フラグ</summary>
	public bool RankUpConfirmFlag { get; set; }
	/// <summary>"または"画面表示フラグ</summary>
	public bool DisplayOrFlag { get; set; }
}