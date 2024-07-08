/*
=========================================================================================================
  Module      : 広告コードマスタ入力クラス(AdvCodeInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Input;
using w2.Domain.AdvCode;
using System.Linq;

namespace Input.AdvCode
{
	/// <summary>
	/// 広告コードマスタ入力クラス
	/// </summary>
	public class AdvCodeInput : InputBase<AdvCodeModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AdvCodeInput()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public AdvCodeInput(AdvCodeModel model)
			: this()
		{
			this.AdvcodeNo = model.AdvcodeNo.ToString();
			this.DeptId = model.DeptId;
			this.AdvertisementCode = model.AdvertisementCode;
			this.MediaName = model.MediaName;
			this.ValidFlg = model.ValidFlg;
			this.DateCreated = model.DateCreated.ToString();
			this.DateChanged = model.DateChanged.ToString();
			this.LastChanged = model.LastChanged;
			this.AdvertisementDate = (model.AdvertisementDate != null) ? model.AdvertisementDate.ToString() : null;
			this.MediaCost = (model.MediaCost != null) ? model.MediaCost.ToString() : null;
			this.Memo = model.Memo;
			this.PublicationDateFrom = (model.PublicationDateFrom != null) ? model.PublicationDateFrom.ToString() : null;
			this.PublicationDateTo = (model.PublicationDateTo != null) ? model.PublicationDateTo.ToString() : null;
			this.AdvcodeMediaTypeId = model.AdvcodeMediaTypeId;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override AdvCodeModel CreateModel()
		{
			var model = new AdvCodeModel
			{
				AdvcodeNo = long.Parse(this.AdvcodeNo),
				DeptId = this.DeptId,
				AdvertisementCode = this.AdvertisementCode,
				MediaName = this.MediaName,
				ValidFlg = this.ValidFlg,
				LastChanged = this.LastChanged,
				AdvertisementDate = (this.AdvertisementDate != null) ? DateTime.Parse(this.AdvertisementDate) : (DateTime?)null,
				MediaCost = (this.MediaCost != null) ? decimal.Parse(this.MediaCost) : (decimal?)null,
				Memo = this.Memo,
				PublicationDateFrom = (this.PublicationDateFrom != null) ? DateTime.Parse(this.PublicationDateFrom) : (DateTime?)null,
				PublicationDateTo = (this.PublicationDateTo != null) ? DateTime.Parse(this.PublicationDateTo) : (DateTime?)null,
				AdvcodeMediaTypeId = this.AdvcodeMediaTypeId,
				MemberRankIdGrantedAtAccountRegistration = this.MemberRankIdGrantedAtAccountRegistration,
				UserManagementLevelIdGrantedAtAccountRegistration = this.UserManagementLevelIdGrantedAtAccountRegistration,
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate(string actionStatus, string deptId, string currentAdvertisementCode)
		{
			var errorMessages = string.Empty;
			var validateType = string.Empty;

			switch (actionStatus)
			{
				case Constants.ACTION_STATUS_INSERT:
				case Constants.ACTION_STATUS_COPY_INSERT:
					validateType = "AdvertisementCodeRegist";
					break;

				case Constants.ACTION_STATUS_UPDATE:
					validateType = "AdvertisementCodeModify";
					break;

				default:
					//  ステータスが設定されていない場合
					return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			}

			errorMessages = Validator.Validate(validateType, this.DataSource);

			if (string.IsNullOrEmpty(errorMessages) == false) return errorMessages;

			// Check duplication
			if ((actionStatus == Constants.ACTION_STATUS_INSERT)
				|| (actionStatus == Constants.ACTION_STATUS_COPY_INSERT)
				|| ((actionStatus == Constants.ACTION_STATUS_UPDATE)
					&& (this.AdvertisementCode != currentAdvertisementCode)))
			{
				var advCodeModel = new AdvCodeService().GetAdvCodeFromAdvertisementCode(this.AdvertisementCode);
				if (advCodeModel != null)
				{
					errorMessages += WebMessages.GetMessages(WebMessages.INPUTCHECK_DUPLICATION).Replace("@@ 1 @@", "広告コード");
				}
			}

			// Check public date
			if ((this.PublicationDateTo != null) && (Validator.CheckDateRange(this.PublicationDateFrom, this.PublicationDateTo) == false))
			{
				errorMessages += WebMessages.GetMessages(WebMessages.INPUTCHECK_DATERANGE).Replace("@@ 1 @@", "媒体掲載期間");
			}

			return errorMessages;
		}
		#endregion

		#region プロパティ
		/// <summary>広告コードNO</summary>
		public string AdvcodeNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_ADVCODE_NO]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_ADVCODE_NO] = value; }
		}
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_DEPT_ID] = value; }
		}
		/// <summary>広告コード</summary>
		public string AdvertisementCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE] = value; }
		}
		/// <summary>媒体名</summary>
		public string MediaName
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_MEDIA_NAME]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_MEDIA_NAME] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_LAST_CHANGED] = value; }
		}
		/// <summary>出稿日</summary>
		public string AdvertisementDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_ADVERTISEMENT_DATE]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_ADVERTISEMENT_DATE] = value; }
		}
		/// <summary>媒体費</summary>
		public string MediaCost
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_MEDIA_COST]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_MEDIA_COST] = value; }
		}
		/// <summary>備考</summary>
		public string Memo
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_MEMO]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_MEMO] = value; }
		}
		/// <summary>媒体掲載期間(From)</summary>
		public string PublicationDateFrom
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_PUBLICATION_DATE_FROM]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_PUBLICATION_DATE_FROM] = value; }
		}
		/// <summary>媒体掲載期間(To)</summary>
		public string PublicationDateTo
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_PUBLICATION_DATE_TO]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_PUBLICATION_DATE_TO] = value; }
		}
		/// <summary>広告媒体区分</summary>
		public string AdvcodeMediaTypeId
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_ADVCODE_MEDIA_TYPE_ID]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_ADVCODE_MEDIA_TYPE_ID] = value; }
		}
		/// <summary>会員登録時紐づけ会員ランクID</summary>
		public string MemberRankIdGrantedAtAccountRegistration
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_MEMBER_RANK_ID_GRANTED_AT_ACCOUNT_REGISTRATION]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_MEMBER_RANK_ID_GRANTED_AT_ACCOUNT_REGISTRATION] = value; }
		}
		/// <summary>会員登録時紐づけユーザー管理レベルID</summary>
		public string UserManagementLevelIdGrantedAtAccountRegistration
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODE_USER_MANAGEMENT_LEVEL_ID_GRANTED_AT_ACCOUNT_REGISTRATION]; }
			set { this.DataSource[Constants.FIELD_ADVCODE_USER_MANAGEMENT_LEVEL_ID_GRANTED_AT_ACCOUNT_REGISTRATION] = value; }
		}
		#endregion
	}
}