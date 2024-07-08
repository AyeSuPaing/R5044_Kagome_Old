/*
=========================================================================================================
  Module      : インシデントモデルのパーシャルクラス(CsIncidentModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.Common.Util;
using w2.App.Common.Cs.IncidentCategory;
using w2.App.Common.Cs.Message;

namespace w2.App.Common.Cs.Incident
{
	/// <summary>
	/// インシデントモデルのパーシャルクラス
	/// </summary>
	public partial class CsIncidentModel : ModelBase<CsIncidentModel>
	{
		/// <summary>
		/// 値に応じて "(無効)" 文言を文字列末尾に追加する
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>インシデントモデル</returns>
		public void EX_SetInvalidLavel(string deptId)
		{
			// 表示文言の組み立て：カテゴリ欄
			if (this.IncidentCategoryId != "")
			{
				CsIncidentCategoryService catService = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
				CsIncidentCategoryModel[] categories = catService.GetAll(deptId);
				if (categories.Any(p => (p.CategoryId == this.IncidentCategoryId) && (p.EX_IsParentValid == false))) this.EX_IncidentCategoryName += "(無効)";
			}

			// 表示文言の組み立て：担当オペレータ欄
			if (this.OperatorId != "")
			{
				if (this.EX_IsOperatorValid == false) this.EX_CsOperatorName += "(無効)";
			}

			// 表示文言の組み立て：担当グループ欄
			if (this.CsGroupId != "")
			{
				if (this.EX_IsGroupValid == false) this.EX_CsGroupName += "(無効)";
			}
		}

		/// <summary>
		/// インシデント最後のメッセージが存在するか
		/// </summary>
		/// <returns>存在する場合：true</returns>
		public bool IsExistLastMessage()
		{
			return this.LastMessage != null;
		}

		#region プロパティ
		/// <summary>総件数</summary>
		public int EX_SearchCount
		{
			get { return (int)this.DataSource["row_count"]; }
		}
		/// <summary>インシデントカテゴリ名</summary>
		public string EX_IncidentCategoryName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENTCATEGORY_CATEGORY_NAME]); }
			set { this.DataSource[Constants.FIELD_CSINCIDENTCATEGORY_CATEGORY_NAME] = value; }
		}
		/// <summary>VOC文字列</summary>
		public string EX_VocText
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENTVOC_VOC_TEXT]); }
			set { this.DataSource[Constants.FIELD_CSINCIDENTVOC_VOC_TEXT] = value; }
		}
		/// <summary>担当者名</summary>
		public string EX_CsOperatorName
		{
			get { return StringUtility.ToEmpty(this.DataSource["operator_name"]); }
			set { this.DataSource["operator_name"] = value; }
		}
		/// <summary>担当グループ名</summary>
		public string EX_CsGroupName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSGROUP_CS_GROUP_NAME]); }
			set { this.DataSource[Constants.FIELD_CSGROUP_CS_GROUP_NAME] = value; }
		}
		/// <summary>ステータス文言</summary>
		public string EX_StatusText
		{
			get { return ValueText.GetValueText(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_STATUS, StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENT_STATUS])); }
		}
		/// <summary>重要度文言</summary>
		public string EX_ImportanceText
		{
			get { return ValueText.GetValueText(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_IMPORTANCE, StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENT_IMPORTANCE])); }
		}
		/// <summary>インシデント集計区分値リスト</summary>
		public CsIncidentSummaryValueModel[] EX_SummaryValues
		{
			get { return (CsIncidentSummaryValueModel[])(this.DataSource["EX_SummaryValues"] ?? new CsIncidentSummaryValueModel[0]); }
			set { this.DataSource["EX_SummaryValues"] = value; }
		}
		/// <summary>ロックステータス名称</summary>
		public string EX_LockStatusName
		{
			get { return ValueText.GetValueText(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_LOCK_STATUS, StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENT_LOCK_STATUS])); }
		}
		/// <summary>ロックオペレータ名</summary>
		public string EX_LockOperatorName
		{
			get { return StringUtility.ToEmpty(this.DataSource["lock_operator_name"]); }
		}
		/// <summary>変更前担当オペレータ（担当変更通知用）</summary>
		public string EX_OperatorIdBefore
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENT_OPERATOR_ID + "_before"]); }
		}
		/// <summary>担当者が有効かどうか</summary>
		public bool EX_IsOperatorValid
		{
			get { return StringUtility.ToEmpty(this.DataSource["operator_valid_flg"]) != Constants.FLG_SHOPOPERATOR_VALID_FLG_INVALID; }
		}
		/// <summary>担当グループが有効かどうか</summary>
		public bool EX_IsGroupValid
		{
			get { return StringUtility.ToEmpty(this.DataSource["group_valid_flg"]) != Constants.FLG_CSGROUP_VALID_FLG_INVALID; }
		}
		/// <summary>紐付きユーザー氏名</summary>
		public string EX_UserName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_USER_NAME]); }
		}
		/// <summary>紐付きユーザーメールアドレス1</summary>
		public string EX_UserMailAddr1
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_USER_MAIL_ADDR]); }
		}
		/// <summary>紐付きユーザーメールアドレス2</summary>
		public string EX_UserMailAddr2
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_USER_MAIL_ADDR2]); }
		}
		/// <summary>紐付きユーザー電話番号</summary>
		public string EX_UserTel1
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_USER_TEL1]); }
		}
		/// <summary>紐付きユーザー電話番号2</summary>
		public string EX_UserTel2
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_USER_TEL2]); }
		}
		/// <summary>紐付きユーザー会員種別</summary>
		private string EX_UserKbn
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_USER_USER_KBN]); }
		}
		/// <summary>紐付きユーザー会員種別表示名</summary>
		public string EX_UserKbnText
		{
			get { return ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN, this.EX_UserKbn); }
		}
		/// <summary>インシデント最後のメッセージ</summary>
		public CsMessageModel LastMessage { get; set; }
		#endregion
	}
}
