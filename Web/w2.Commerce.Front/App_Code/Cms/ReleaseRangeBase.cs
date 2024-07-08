/*
=========================================================================================================
  Module      : 公開範囲設定 基底クラス(ReleaseRangeBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.Domain.MemberRank;

/// <summary>
/// 公開範囲設定 基底クラス
/// </summary>
public abstract class ReleaseRangeBase
{
	/// <summary>チェック有無</summary>
	public enum CheckType
	{
		/// <summary>チェックする</summary>
		Check,
		/// <summary>チェックしない</summary>
		NoCheck,
	}

	/// <summary>
	/// 条件判定
	/// </summary>
	/// <param name="accessUser">アクセスユーザ</param>
	/// <param name="publish">公開状態 チェック有無</param>
	/// <param name="publishDate">公開期間 チェック有無</param>
	/// <param name="memberRank">会員ランク チェック有無</param>
	/// <param name="targetList">ターゲットリスト チェック有無</param>
	/// <returns></returns>
	public ReleaseRangeResult Check(
		ReleaseRangeAccessUser accessUser,
		CheckType publish = CheckType.Check,
		CheckType publishDate = CheckType.Check,
		CheckType memberRank = CheckType.Check,
		CheckType targetList = CheckType.Check)
	{
		var result = new ReleaseRangeResult();

		// 公開状態 判定
		if (publish == CheckType.Check)
		{
			result.Publish = (this.ReleaseRangeCondition.Publish == this.ReleaseRangeDefinition.PublishPublic)
				? ReleaseRangeResult.RangeResult.In
				: ReleaseRangeResult.RangeResult.Out;
		}

		// 公開期間 判定
		if (publishDate == CheckType.Check)
		{
			result.PublishDate =
				((this.ReleaseRangeCondition.PublishDateFrom == null)
					|| (this.ReleaseRangeCondition.PublishDateFrom <= accessUser.Now))
				&& ((this.ReleaseRangeCondition.PublishDateTo == null)
					|| (this.ReleaseRangeCondition.PublishDateTo > accessUser.Now))
					? ReleaseRangeResult.RangeResult.In
					: ReleaseRangeResult.RangeResult.Out;
		}

		// 会員ランク 判定
		var memberRankRequiredLoginFlg = false;
		if (Constants.MEMBER_RANK_OPTION_ENABLED
			&& (this.ReleaseRangeCondition.MemberOnlyType != this.ReleaseRangeDefinition.MemberAll)
			&& (memberRank == CheckType.Check))
		{
			memberRankRequiredLoginFlg = (accessUser.IsLoggedIn == false);

			if (accessUser.IsLoggedIn)
			{
				if (this.ReleaseRangeCondition.MemberOnlyType == this.ReleaseRangeDefinition.MemberPartialOnly)
				{
					result.MemberRank = ((accessUser.MemberRankInfo != null)
						&& (accessUser.MemberRankInfo.MemberRankOrder <= this.ReleaseRangeCondition.MemberRankInfo.MemberRankOrder))
							? ReleaseRangeResult.RangeResult.In
							: ReleaseRangeResult.RangeResult.Out;
				}
				else if (this.ReleaseRangeCondition.MemberOnlyType == this.ReleaseRangeDefinition.MemberOnly)
				{
					result.MemberRank = ReleaseRangeResult.RangeResult.In;
				}
			}
			else
			{
				result.MemberRank = ReleaseRangeResult.RangeResult.Out;
			}
		}

		// ターゲットリスト 判定
		var targetListRequiredLoginFlg = false;
		if ((this.ReleaseRangeCondition.TargetListIds.Length > 0) && (targetList == CheckType.Check))
		{
			targetListRequiredLoginFlg = (accessUser.IsLoggedIn == false);

			if (accessUser.IsLoggedIn)
			{
				var targetListResult = true;
				foreach (var targetListId in this.ReleaseRangeCondition.TargetListIds)
				{
					targetListResult = accessUser.HitTargetListId.Any(id => id == targetListId);

					if ((this.ReleaseRangeCondition.TargetListType == this.ReleaseRangeDefinition.TargetListJudgmentTypeAnd)
						&& (targetListResult == false))
					{
						break;
					}

					if ((this.ReleaseRangeCondition.TargetListType == this.ReleaseRangeDefinition.TargetListJudgmentTypeOr)
						&& targetListResult)
					{
						break;
					}
				}

				result.TargetList = (targetListResult)
					? ReleaseRangeResult.RangeResult.In
					: ReleaseRangeResult.RangeResult.Out;
			}
			else
			{
				result.TargetList = ReleaseRangeResult.RangeResult.Out;
			}
		}

		// ログインの必要有無
		result.RequiredLoginStatus = (memberRankRequiredLoginFlg || targetListRequiredLoginFlg)
			? ReleaseRangeResult.RequiredLogin.Required
			: ReleaseRangeResult.RequiredLogin.NotRequired;

		return result;
	}

	/// <summary>条件 定義値</summary>
	protected ReleaseRangeDefinition ReleaseRangeDefinition { get; set; }
	/// <summary>条件内容</summary>
	protected ReleaseRangeCondition ReleaseRangeCondition { get; set; }
}

/// <summary>
/// 公開範囲設定 結果
/// </summary>
public class ReleaseRangeResult
{
	/// <summary>ログイン必須有無</summary>
	public enum RequiredLogin
	{
		/// <summary>ログイン必要</summary>
		Required,
		/// <summary>ログイン不要</summary>
		NotRequired
	}

	/// <summary>範囲結果</summary>
	public enum RangeResult
	{
		/// <summary>条件範囲内</summary>
		In,
		/// <summary>条件範囲外</summary>
		Out
	}

	/// <summary>
	/// コンストラクタ
	/// </summary>
	public ReleaseRangeResult()
	{
		this.RequiredLoginStatus = RequiredLogin.NotRequired;
		this.Publish = RangeResult.In;
		this.PublishDate = RangeResult.In;
		this.MemberRank = RangeResult.In;
		this.TargetList = RangeResult.In;
	}

	/// <summary>公開反映設定範囲外フラグ(TRUE：範囲外 FALSE：範囲内)</summary>
	public bool IsReleaseRangeOut
	{
		get
		{
			return ((this.Publish == ReleaseRangeResult.RangeResult.Out)
				|| (this.PublishDate == ReleaseRangeResult.RangeResult.Out)
				|| (this.MemberRank == ReleaseRangeResult.RangeResult.Out)
				|| (this.TargetList == ReleaseRangeResult.RangeResult.Out));
		}
	}
	/// <summary>ログイン必須有無 ステータス</summary>
	public RequiredLogin RequiredLoginStatus { get; set; }
	/// <summary>公開範囲 結果</summary>
	public RangeResult Publish { get; set; }
	/// <summary>公開期間 結果</summary>
	public RangeResult PublishDate { get; set; }
	/// <summary>会員ランク 結果</summary>
	public RangeResult MemberRank { get; set; }
	/// <summary>ターゲットリスト 結果</summary>
	public RangeResult TargetList { get; set; }
}

/// <summary>
/// 公開範囲設定 アクセスユーザ
/// </summary>
public class ReleaseRangeAccessUser
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public ReleaseRangeAccessUser()
	{
		this.Now = DateTime.Now;
		this.MemberRankInfo = null;
		this.IsLoggedIn = false;
		this.HitTargetListId = new string[] { };
	}

	/// <summary>アクセス時刻</summary>
	public DateTime Now { get; set; }
	/// <summary>会員ランクID</summary>
	public MemberRankModel MemberRankInfo { get; set; }
	/// <summary>ログイン状態</summary>
	public bool IsLoggedIn { get; set; }
	/// <summary>ヒットするターゲットリストID一覧</summary>
	public string[] HitTargetListId { get; set; }
}

/// <summary>
/// 公開範囲設定 条件内容
/// </summary>
public class ReleaseRangeCondition
{
	/// <summary>
	/// ターゲットリスト一覧のセット
	/// </summary>
	/// <param name="targetListIds">ターゲットリスト一覧</param>
	public void SetTargetListIds(string targetListIds)
	{
		this.TargetListIds = (string.IsNullOrEmpty(targetListIds))
			? new string[] { }
			: targetListIds.Split(',').ToArray();
	}

	/// <summary>公開状態</summary>
	public string Publish { get; set; }
	/// <summary>公開期間 開始日</summary>
	public DateTime? PublishDateFrom { get; set; }
	/// <summary>公開期間 終了日</summary>
	public DateTime? PublishDateTo { get; set; }
	/// <summary>会員ランク 条件タイプ</summary>
	public string MemberOnlyType { get; set; }
	/// <summary>会員ランク 会員限定時の対象会員ランク</summary>
	public MemberRankModel MemberRankInfo { get; set; }
	/// <summary>ターゲットリストID一覧</summary>
	public string[] TargetListIds { get; set; }
	/// <summary>ターゲットリスト 条件タイプ</summary>
	public string TargetListType { get; set; }
}

/// <summary>
/// 公開範囲設定 定義内容
/// </summary>
public class ReleaseRangeDefinition
{
	/// <summary>
	/// 公開状態 定義セット
	/// </summary>
	/// <param name="publishPublic">公開状態 公開</param>
	/// <param name="publishPrivate">公開状態 非公開</param>
	public void SetPublish(string publishPublic, string publishPrivate)
	{
		this.PublishPublic = publishPublic;
		this.PublishPrivate = publishPrivate;
	}

	/// <summary>
	/// 会員ランク 定義セット
	/// </summary>
	/// <param name="all">会員ランク 全ユーザ</param>
	/// <param name="only">会員ランク 全会員</param>
	/// <param name="partialOnly">会員ランク 一部会員のみ</param>
	public void SetMember(string all, string only, string partialOnly)
	{
		this.MemberAll = all;
		this.MemberOnly = only;
		this.MemberPartialOnly = partialOnly;
	}

	/// <summary>
	/// ターゲットリスト 定義セット
	/// </summary>
	/// <param name="or">ターゲットリスト OR条件</param>
	/// <param name="and">ターゲットリスト AND条件</param>
	public void SetTargetListJudgment(string or, string and)
	{
		this.TargetListJudgmentTypeOr = or;
		this.TargetListJudgmentTypeAnd = and;
	}

	/// <summary>公開状態 公開</summary>
	public string PublishPublic { get; private set; }
	/// <summary>公開状態 非公開</summary>
	public string PublishPrivate { get; private set; }
	/// <summary>会員ランク 全ユーザ</summary>
	public string MemberAll { get; private set; }
	/// <summary>会員ランク 全会員</summary>
	public string MemberOnly { get; private set; }
	/// <summary>会員ランク 一部会員のみ</summary>
	public string MemberPartialOnly { get; private set; }
	/// <summary>ターゲットリスト OR条件</summary>
	public string TargetListJudgmentTypeOr { get; private set; }
	/// <summary>ターゲットリスト AND条件</summary>
	public string TargetListJudgmentTypeAnd { get; private set; }
}