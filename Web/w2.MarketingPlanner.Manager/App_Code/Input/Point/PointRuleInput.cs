/*
=========================================================================================================
  Module      : ポイントルール入力情報(PointRuleInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Input;
using w2.Domain.Point;
using System.Linq;
using System.Web;
using w2.Common.Util.Security;
using w2.Common.Web;

namespace Input.Point
{
	/// <summary>
	/// ポイントルール入力情報
	/// </summary>
	[Serializable]
	public class PointRuleInput : InputBase<PointRuleModel>
	{
		/// <summary>期間限定ポイント：有効期限を設定する</summary>
		public const string LIMTIED_TERM_POINT_EXPIRE_KBN_RELATIVE = "ExpireRelative";
		/// <summary>期間限定ポイント：有効期間を設定する</summary>
		public const string LIMTIED_TERM_POINT_EXPIRE_KBN_ABSOLUTE = "ExpireAbsolute";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PointRuleInput()
		{
			this.RuleDate = new PointRuleDateInput[] { };
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public PointRuleInput(PointRuleModel model)
			: this()
		{
			this.DeptId = model.DeptId;
			this.PointRuleId = model.PointRuleId;
			this.PointRuleName = model.PointRuleName;
			this.PointRuleKbn = model.PointRuleKbn;
			this.PointKbn = model.PointKbn;
			this.UseTempFlg = model.UseTempFlg;
			this.PointIncKbn = model.PointIncKbn;
			this.IncType = model.IncType;
			this.IncNum = model.IncNum.ToString();
			this.IncRate = model.IncRate.ToString();
			this.PointExpExtend = model.PointExpExtend;
			this.ExpBgn = model.ExpBgn.ToString();
			this.ExpEnd = model.ExpEnd.ToString();
			this.CampaignTermKbn = model.CampaignTermKbn;
			this.CampaignTermValue = model.CampaignTermValue;
			this.Priority = model.Priority.ToString();
			this.ValidFlg = model.ValidFlg;
			this.Kbn1 = model.Kbn1;
			this.Kbn2 = model.Kbn2;
			this.Kbn3 = model.Kbn3;
			this.Kbn4 = model.Kbn4;
			this.Kbn5 = model.Kbn5;
			this.DateCreated = model.DateCreated.ToString();
			this.DateChanged = model.DateChanged.ToString();
			this.LastChanged = model.LastChanged;
			this.EffectiveOffset = model.EffectiveOffset.ToString();
			this.EffectiveOffsetType = model.EffectiveOffsetType;
			this.Term = model.Term.ToString();
			this.TermType = model.TermType;
			this.PeriodBegin = model.PeriodBegin.ToString();
			this.PeriodEnd = model.PeriodEnd.ToString();
			this.AllowDuplicateApplyFlg = model.AllowDuplicateApplyFlg;
			this.IncFixedPurchaseType = model.IncFixedPurchaseType;
			this.IncFixedPurchaseNum = model.IncFixedPurchaseNum.ToString();
			this.IncFixedPurchaseRate = model.IncFixedPurchaseRate.ToString();
			this.RuleDate = (model.RuleDate != null)
				? model.RuleDate.Select(i => new PointRuleDateInput(i)).ToArray()
				: new PointRuleDateInput[] { };
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override PointRuleModel CreateModel()
		{
			// TryParse用一時変数
			DateTime dtTemp;
			int iTemp;
			var model = new PointRuleModel
			{
				DeptId = this.DeptId,
				PointRuleId = this.PointRuleId,
				PointRuleName = this.PointRuleName,
				PointRuleKbn = this.PointRuleKbn,
				PointKbn = this.PointKbn,
				UseTempFlg = this.UseTempFlg,
				PointIncKbn = this.PointIncKbn,
				IncType = this.IncType,
				IncNum = string.IsNullOrEmpty(this.IncNum) ? 0 : decimal.Parse(this.IncNum),
				IncRate = string.IsNullOrEmpty(this.IncRate) ? 0 : decimal.Parse(this.IncRate),
				PointExpExtend = this.PointExpExtend,
				ExpBgn = DateTime.TryParse(this.ExpBgn, out dtTemp) ? dtTemp : (DateTime?)null,
				ExpEnd = DateTime.TryParse(this.ExpEnd, out dtTemp) ? dtTemp : (DateTime?)null,
				CampaignTermKbn = this.CampaignTermKbn,
				CampaignTermValue = this.CampaignTermValue,
				Priority = int.Parse(this.Priority),
				ValidFlg = this.ValidFlg,
				Kbn1 = this.Kbn1,
				Kbn2 = this.Kbn2,
				Kbn3 = this.Kbn3,
				Kbn4 = this.Kbn4,
				Kbn5 = this.Kbn5,
				LastChanged = this.LastChanged,
				EffectiveOffset = int.TryParse(this.EffectiveOffset, out iTemp) ? iTemp : (int?)null,
				EffectiveOffsetType = this.EffectiveOffsetType,
				Term = int.TryParse(this.Term, out iTemp) ? iTemp : (int?)null,
				TermType = this.TermType,
				PeriodBegin = DateTime.TryParse(this.PeriodBegin, out dtTemp) ? dtTemp : (DateTime?)null,
				PeriodEnd = DateTime.TryParse(this.PeriodEnd, out dtTemp) ? dtTemp : (DateTime?)null,
				AllowDuplicateApplyFlg = this.AllowDuplicateApplyFlg,
				RuleDate = (this.RuleDate != null)
					? this.RuleDate.Select(i => i.CreateModel()).ToArray()
					: new PointRuleDateModel[] { },
				IncFixedPurchaseType = this.IncFixedPurchaseType,
				IncFixedPurchaseNum = string.IsNullOrEmpty(this.IncFixedPurchaseNum) ? 0 : decimal.Parse(this.IncFixedPurchaseNum),
				IncFixedPurchaseRate = string.IsNullOrEmpty(this.IncFixedPurchaseRate) ? 0 : decimal.Parse(this.IncFixedPurchaseRate)
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate()
		{
			var errorMessages = Validate(string.IsNullOrEmpty(this.PointRuleId) ? "PointRuleRegist" : "PointRuleModify");
			return errorMessages;
		}
		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="validatorKey">バリデータのKey</param>
		/// <returns></returns>
		public string Validate(string validatorKey)
		{
			var errorMessages = Validator.Validate(validatorKey, this.DataSource);

			if ((this.UseScheduleIncKbnPointRule == false) && (Validator.CheckDateRange(this.ExpBgn, this.ExpEnd) == false))
			{
				errorMessages += WebMessages.GetMessages(WebMessages.INPUTCHECK_DATERANGE).Replace("@@ 1 @@", "ルール有効期間");
			}

			// ポイント有効期間のチェック
			if (this.IsLimitedTermPoint
				&& (this.LimitedTermPointExpireKbn == LIMTIED_TERM_POINT_EXPIRE_KBN_ABSOLUTE))
			{
				// 日付逆転チェック
				if ((Validator.IsDate(this.PeriodBegin) && Validator.IsDate(this.PeriodEnd))
					&& (DateTime.Parse(this.PeriodBegin) > DateTime.Parse(this.PeriodEnd)))
				{
					errorMessages += WebMessages.GetMessages(WebMessages.INPUTCHECK_DATERANGE).Replace("@@ 1 @@", "期間限定ポイント有効期間");
				}
				// ポイント有効期間とルール有効期間の妥当性チェック
				// ポイントの有効期間終了後にルールが有効になるのは不可(ポイントが絶対に発行されない)
				else if ((this.UseScheduleIncKbnPointRule == false)
					&& (Validator.IsDate(this.PeriodEnd) && Validator.IsDate(this.ExpBgn))
					&& (DateTime.Parse(this.PeriodEnd) < DateTime.Parse(this.ExpBgn)))
				{
					errorMessages += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_POINT_MAY_NOT_BE_ISSUED);
				}
				// ルール有効期間内にポイントの有効期間が終了するものは不可
				else if ((this.UseScheduleIncKbnPointRule == false)
					&& (Validator.IsDate(this.PeriodEnd) && Validator.IsDate(this.ExpEnd))
					&& (DateTime.Parse(this.PeriodEnd) < DateTime.Parse(this.ExpEnd)))
				{
					errorMessages += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_POINT_MAY_NOT_BE_ISSUED);
				}
			}

			return errorMessages;
		}

		/// <summary>
		/// クリックポイント発行URL作成
		/// </summary>
		/// <returns>クリックポイント発行URL</returns>
		public string ClikPointUrlCreator()
		{
			var encryptPointRuleId = PointRuleIdEncrypt(this.PointRuleId);
			var url = new UrlCreator(
					Constants.URL_FRONT_PC.Replace(Constants.PROTOCOL_HTTP, Constants.PROTOCOL_HTTPS)
					+ Constants.PAGE_FRONT_CLICKPOINT)
				.AddParam(Constants.REQUEST_KEY_POINT_RULE_ID, encryptPointRuleId)
				.CreateUrl();
			return url;
		}

		/// <summary>
		/// ポイントルールID暗号化
		/// </summary>
		/// <param name="pointRuleId">ポイントルールID</param>
		/// <returns>暗号化したポイントルールID</returns>
		private string PointRuleIdEncrypt(string pointRuleId)
		{
			var rcPointRuleId = new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
			return rcPointRuleId.Encrypt(pointRuleId);
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_DEPT_ID] = value; }
		}
		/// <summary>ポイントルールID</summary>
		public string PointRuleId
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_POINT_RULE_ID]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_POINT_RULE_ID] = value; }
		}
		/// <summary>ポイントルール名</summary>
		public string PointRuleName
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_POINT_RULE_NAME]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_POINT_RULE_NAME] = value; }
		}
		/// <summary>ポイントルール区分</summary>
		public string PointRuleKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_POINT_RULE_KBN]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_POINT_RULE_KBN] = value; }
		}
		/// <summary>対象ポイント区分</summary>
		public string PointKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_POINT_KBN]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_POINT_KBN] = value; }
		}
		/// <summary>仮ポイント使用フラグ</summary>
		public string UseTempFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_USE_TEMP_FLG]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_USE_TEMP_FLG] = value; }
		}
		/// <summary>ポイント加算区分</summary>
		public string PointIncKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_POINT_INC_KBN]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_POINT_INC_KBN] = value; }
		}
		/// <summary>ポイント加算方法</summary>
		public string IncType
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_INC_TYPE]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_INC_TYPE] = value; }
		}
		/// <summary>ポイント加算数</summary>
		public string IncNum
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_INC_NUM]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_INC_NUM] = value; }
		}
		/// <summary>ポイント加算率</summary>
		public string IncRate
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_INC_RATE]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_INC_RATE] = value; }
		}
		/// <summary>ポイント有効期限延長</summary>
		public string PointExpExtend
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_POINT_EXP_EXTEND]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_POINT_EXP_EXTEND] = value; }
		}
		/// <summary>有効期間（開始）</summary>
		public string ExpBgn
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_EXP_BGN]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_EXP_BGN] = value; }
		}
		/// <summary>有効期間（終了）</summary>
		public string ExpEnd
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_EXP_END]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_EXP_END] = value; }
		}
		/// <summary>キャンペーン期間区分</summary>
		public string CampaignTermKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_CAMPAIGN_TERM_KBN]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_CAMPAIGN_TERM_KBN] = value; }
		}
		/// <summary>キャンペーン期間値</summary>
		public string CampaignTermValue
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_CAMPAIGN_TERM_VALUE]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_CAMPAIGN_TERM_VALUE] = value; }
		}
		/// <summary>優先順位</summary>
		public string Priority
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_PRIORITY]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_PRIORITY] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_VALID_FLG] = value; }
		}
		/// <summary>予備区分1</summary>
		public string Kbn1
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_KBN1]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_KBN1] = value; }
		}
		/// <summary>予備区分2</summary>
		public string Kbn2
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_KBN2]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_KBN2] = value; }
		}
		/// <summary>予備区分3</summary>
		public string Kbn3
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_KBN3]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_KBN3] = value; }
		}
		/// <summary>予備区分4</summary>
		public string Kbn4
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_KBN4]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_KBN4] = value; }
		}
		/// <summary>予備区分5</summary>
		public string Kbn5
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_KBN5]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_KBN5] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_LAST_CHANGED] = value; }
		}
		/// <summary>期間限定ポイント発効オフセット</summary>
		public string EffectiveOffset
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_EFFECTIVE_OFFSET]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_EFFECTIVE_OFFSET] = value; }
		}
		/// <summary>期間限定ポイント発効オフセット種別</summary>
		public string EffectiveOffsetType
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_EFFECTIVE_OFFSET_TYPE]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_EFFECTIVE_OFFSET_TYPE] = value; }
		}
		/// <summary>期間限定ポイント有効期間（相対）</summary>
		public string Term
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_TERM]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_TERM] = value; }
		}
		/// <summary>期間限定ポイント有効期間（相対）種別</summary>
		public string TermType
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_TERM_TYPE]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_TERM_TYPE] = value; }
		}
		/// <summary>期間限定ポイント有効期間（絶対）開始日</summary>
		public string PeriodBegin
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_PERIOD_BEGIN]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_PERIOD_BEGIN] = value; }
		}
		/// <summary>期間限定ポイント有効期間（絶対）終了日</summary>
		public string PeriodEnd
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_PERIOD_END]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_PERIOD_END] = value; }
		}
		/// <summary>キャンペーン：基本ルールとの二重適用を許可するか</summary>
		public string AllowDuplicateApplyFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_ALLOW_DUPLICATE_APPLY_FLG]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_ALLOW_DUPLICATE_APPLY_FLG] = value; }
		}
		/// <summary>定期ポイント加算方法</summary>
		public string IncFixedPurchaseType
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_TYPE]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_TYPE] = value; }
		}
		/// <summary>定期ポイント加算数</summary>
		public string IncFixedPurchaseNum
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_NUM]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_NUM] = value; }
		}
		/// <summary>定期ポイント加算率</summary>
		public string IncFixedPurchaseRate
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_RATE]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_RATE] = value; }
		}
		#endregion

		#region 拡張プロパティ
		/// <summary>
		/// 有効期間（開始）チェック用
		/// </summary>
		public string ExpBgnChk
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_EXP_BGN + "_chk"]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_EXP_BGN + "_chk"] = value; }
		}
		/// <summary>有効期間（終了）チェック用</summary>
		public string ExpEndChk
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_EXP_END + "_chk"]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_EXP_END + "_chk"] = value; }
		}
		/// <summary>ポイント加算ルール(必須チェック用)</summary>
		public string Inc
		{
			get { return (string)this.DataSource["inc"]; }
			set { this.DataSource["inc"] = value; }
		}
		/// <summary>ポイントルール日付</summary>
		public PointRuleDateInput[] RuleDate
		{
			get { return (PointRuleDateInput[])this.DataSource["ruledate"]; }
			set { this.DataSource["ruledate"] = value; }
		}
		/// <summary>優先度(チェック用)</summary>
		public string PriorityChk
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULE_PRIORITY + "_chk"]; }
			set { this.DataSource[Constants.FIELD_POINTRULE_PRIORITY + "_chk"] = value; }
		}
		/// <summary>ポイント有効期限の延長設定（月）</summary>
		public string PointExpExtendMonth
		{
			get { return this.PointExpExtend.Length >= 7 ? this.PointExpExtend.Substring(3, 2) : string.Empty; }
		}
		/// <summary>ポイント加算区分がスケジュール利用できるか？</summary>
		public bool UseScheduleIncKbnPointRule
		{
			get
			{
				return ((this.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_VERSATILE_POINT_RULE)
					|| (this.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_BIRTHDAY_POINT));
			}
		}
		/// <summary>クリックポイントルールか</summary>
		public bool IsClickPoint
		{
			get { return this.PointIncKbn == Constants.FLG_POINTRULE_POITN_INC_KBN_CLICK; }
		}
		/// <summary>通常ポイントか</summary>
		public bool IsBasePoint
		{
			get { return (this.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE); }
		}
		/// <summary>期間限定ポイントか</summary>
		public bool IsLimitedTermPoint
		{
			get { return (this.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_LIMITED_TERM_POINT); }
		}
		/// <summary>期間限定ポイント：ポイントルールが有効期限の設定か有効期間の設定か</summary>
		public string LimitedTermPointExpireKbn { get; set; }
		/// <summary>仮ポイントを使用する必要があるか(ポイント加算区分は考慮しない)</summary>
		public bool NeedsToUseTemporaryPointInSetting
		{
			get { return Constants.MARKETINGPLANNER_USE_TEMPORARY_POINT; }
		}
		/// <summary>定期ポイント加算ルール(必須チェック用)</summary>
		public string IncFixedPurchase
		{
			get { return (string)this.DataSource["incFixedPurchase"]; }
			set { this.DataSource["incFixedPurchase"] = value; }
		}
		#endregion
	}
}