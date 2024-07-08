/*
=========================================================================================================
  Module      : 公開範囲 入力(ReleaseRangeSettingInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using System.Web.Mvc;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes.Common;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.MemberRank;
using w2.Domain.TargetList;
using Constants = w2.App.Common.Constants;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// 公開範囲 入力
	/// </summary>
	public class ReleaseRangeSettingInput
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ReleaseRangeSettingInput()
		{
			this.ConditionMemberOnlyType = Constants.FLG_PAGEDESIGN_MEMBER_ONLY_TYPE_ALL;
			this.ConditionMemberRankId = string.Empty;
			this.ConditionTargetListIds = string.Empty;
			this.ConditionTargetListType = Constants.FLG_PAGEDESIGN_CONDITION_TARGET_LIST_TYPE_OR;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="id">識別ID</param>
		/// <param name="baseName">バインド時の名前</param>
		/// <param name="releaseRangeSettingModel">公開範囲モデル</param>
		/// <param name="useDate">公開期間利用</param>
		/// <param name="useMemberRank">公開メンバーランク利用</param>
		/// <param name="useTargetList">公開ターゲットリスト利用</param>
		/// <param name="targetListModels">ターゲットリストモデル 設定がある場合に利用 無い場合はメソッド内で取得</param>
		public ReleaseRangeSettingInput(
			string id,
			string baseName,
			ReleaseRangeSettingModel releaseRangeSettingModel,
			bool useDate = true,
			bool useMemberRank = true,
			bool useTargetList = true,
			TargetListModel[] targetListModels = null)
		{
			this.Id = id;
			this.BaseName = baseName;
			this.UseDate = useDate;
			this.UseMemberRank = useMemberRank;
			this.UseTargetList = useTargetList;

			if (Constants.MEMBER_RANK_OPTION_ENABLED && this.UseMemberRank)
			{
				this.MemberRankItems = DomainFacade.Instance.MemberRankService.GetMemberRankList()
					.OrderBy(m => m.MemberRankOrder)
					.Select(
						m => new SelectListItem
						{
							Value = m.MemberRankId,
							Text = string.Format("{0} : {1}", m.MemberRankOrder, m.MemberRankName)
						}).ToArray();
				this.ConditionMemberOnlyType = releaseRangeSettingModel.ConditionMemberOnlyType;
				this.ConditionMemberRankId = StringUtility.ToEmpty(releaseRangeSettingModel.ConditionMemberRankId);
			}
			else
			{
				this.ConditionMemberOnlyType = Constants.FLG_PAGEDESIGN_MEMBER_ONLY_TYPE_ALL;
				this.ConditionMemberRankId = string.Empty;
			}

			if (this.UseDate)
			{
				this.ConditionPublishDateFromDate = (releaseRangeSettingModel.ConditionPublishDateFrom == null)
					? string.Empty
					: releaseRangeSettingModel.ConditionPublishDateFrom.Value.ToString("yyyy/MM/dd");

				this.ConditionPublishDateFromTime = (releaseRangeSettingModel.ConditionPublishDateFrom == null)
					? ""
					: releaseRangeSettingModel.ConditionPublishDateFrom.Value.ToString("HH:mm");

				this.ConditionPublishDateToDate = (releaseRangeSettingModel.ConditionPublishDateTo == null)
					? string.Empty
					: releaseRangeSettingModel.ConditionPublishDateTo.Value.ToString("yyyy/MM/dd");

				this.ConditionPublishDateToTime = (releaseRangeSettingModel.ConditionPublishDateTo == null)
					? ""
					: releaseRangeSettingModel.ConditionPublishDateTo.Value.ToString("HH:mm");
			}

			if (this.UseTargetList)
			{
				this.ConditionTargetListIds = StringUtility.ToEmpty(releaseRangeSettingModel.ConditionTargetListIds);
				var allTargetList = (targetListModels ?? new TargetListService().GetAll(Constants.CONST_DEFAULT_SHOP_ID))
					.Where(t => (t.DelFlg == Constants.FLG_TARGETLIST_DEL_FLG_INVALID))
					.ToArray();
				this.TargetListSelect = (string.IsNullOrEmpty(releaseRangeSettingModel.ConditionTargetListIds))
					? new TargetListModel[] { }
					: this.ConditionTargetListIds.Split(',')
						.Select(tid =>
						{
							var model = allTargetList.FirstOrDefault(m => m.TargetId == tid);
							return (model != null)
								? new TargetListModel { DataSource = model.DataSource }
								: new TargetListModel { TargetId = tid };
						}).ToArray();
				this.ConditionTargetListType = releaseRangeSettingModel.ConditionTargetListType;
			}
		}

		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>公開範囲モデル</returns>
		public ReleaseRangeSettingModel CreateModel()
		{
			DateTime dateFrom;
			DateTime dateTo;
			var model = new ReleaseRangeSettingModel
			{
				ConditionPublishDateFrom = (DateTime.TryParse(this.ConditionPublishDateFromDate + " " + this.ConditionPublishDateFromTime, out dateFrom))
						? (DateTime?)dateFrom
						: null,
				ConditionPublishDateTo = (DateTime.TryParse(this.ConditionPublishDateToDate + " " + this.ConditionPublishDateToTime, out dateTo))
					? (DateTime?)dateTo
					: null,
				ConditionMemberOnlyType = this.ConditionMemberOnlyType,
				ConditionMemberRankId = this.ConditionMemberRankId,
				ConditionTargetListIds = this.ConditionTargetListIds,
				ConditionTargetListType = this.ConditionTargetListType
			};

			return model;
		}

		/// <summary>
		/// バリデーション
		/// </summary>
		/// <returns>結果</returns>
		public string Validate()
		{
			var errorMessage = string.Empty;
			errorMessage += DateRangeValidate();

			return errorMessage;
		}

		/// <summary>
		/// 公開期間バリデーション
		/// </summary>
		/// <returns>結果</returns>
		private string DateRangeValidate()
		{
			if (this.UseDate == false) return string.Empty;

			var inputDateFrom = false;
			var dateFrom = new DateTime();
			if (string.IsNullOrEmpty(this.ConditionPublishDateFromDate + this.ConditionPublishDateFromTime) == false)
			{
				if ((DateTime.TryParse(this.ConditionPublishDateFromDate + " " + this.ConditionPublishDateFromTime, out dateFrom))
					&& DateTimeUtility.IsValidSqlDateTimeTerm(dateFrom))
				{
					inputDateFrom = true;
				}
				else
				{
					return WebMessages.ReleaseRangeSettingDateRangeError;
				}
			}

			var inputDateTo = false;
			var dateTo = new DateTime();
			if (string.IsNullOrEmpty(this.ConditionPublishDateToDate + this.ConditionPublishDateToTime) == false)
			{
				if (DateTime.TryParse(this.ConditionPublishDateToDate + " " + this.ConditionPublishDateToTime, out dateTo)
					&& DateTimeUtility.IsValidSqlDateTimeTerm(dateTo))
				{
					inputDateTo = true;
				}
				else
				{
					return WebMessages.ReleaseRangeSettingDateRangeError;
				}
			}

			// 日付が正しく入力されていて、開始 > 終了の場合はエラー
			if (inputDateFrom && inputDateTo && (dateFrom > dateTo))
			{
				return WebMessages.ReleaseRangeSettingDateRangeError;
			}

			return string.Empty;
		}
		/// <summary>識別ID</summary>
		public string Id { get; private set; }
		/// <summary>バインド時の名前</summary>
		public string BaseName { get; private set; }
		/// <summary>公開期間 : 開始日</summary>
		public string ConditionPublishDateFromDate { get; set; }
		/// <summary>公開期間 : 開始時間</summary>
		public string ConditionPublishDateFromTime { get; set; }
		/// <summary>公開期間 : 終了日</summary>
		public string ConditionPublishDateToDate { get; set; }
		/// <summary>公開期間 : 開始時間</summary>
		public string ConditionPublishDateToTime { get; set; }
		/// <summary>公開メンバーランク : 公開タイプ</summary>
		public string ConditionMemberOnlyType { get; set; }
		/// <summary>公開期間 : 公開対象会員ラングID</summary>
		public string ConditionMemberRankId { get; set; }
		/// <summary>公開ターゲットリスト : ID文字列 カンマ区切り</summary>
		public string ConditionTargetListIds { get; set; }
		/// <summary>公開ターゲットリスト : 条件タイプ</summary>
		public string ConditionTargetListType { get; set; }
		/// <summary>公開ターゲットリスト : 選択ターゲットリスト</summary>
		public TargetListModel[] TargetListSelect { get; private set; }
		/// <summary>公開期間利用</summary>
		public bool UseDate { get; set; }
		/// <summary>公開メンバーランク利用</summary>
		public bool UseMemberRank { get; set; }
		/// <summary>公開ターゲットリスト利用</summary>
		public bool UseTargetList { get; set; }
		/// <summary>公開メンバーランク選択肢</summary>
		public SelectListItem[] MemberRankItems { get; private set; }
	}

	/// <summary>
	/// 公開範囲モデル
	/// </summary>
	public class ReleaseRangeSettingModel
	{
		/// <summary>公開期間 : 開始日時</summary>
		public DateTime? ConditionPublishDateFrom { get; set; }
		/// <summary>公開期間 : 終了日時</summary>
		public DateTime? ConditionPublishDateTo { get; set; }
		/// <summary>公開メンバーランク : 公開タイプ</summary>
		public string ConditionMemberOnlyType { get; set; }
		/// <summary>公開期間 : 公開対象会員ラングID</summary>
		public string ConditionMemberRankId { get; set; }
		/// <summary>公開ターゲットリスト : ID文字列 カンマ区切り</summary>
		public string ConditionTargetListIds { get; set; }
		/// <summary>公開ターゲットリスト : 条件タイプ</summary>
		public string ConditionTargetListType { get; set; }
	}
}
