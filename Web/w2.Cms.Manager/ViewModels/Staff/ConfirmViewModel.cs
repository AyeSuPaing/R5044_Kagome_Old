/*
=========================================================================================================
  Module      : スタッフConfirmビューモデル(ConfirmViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.WorkerServices;
using w2.Common.Util;
using w2.Domain.Staff;

namespace w2.Cms.Manager.ViewModels.Staff
{
	/// <summary>
	/// スタッフConfirmビューモデル
	/// </summary>
	[Serializable]
	public class ConfirmViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="pageLayout">レイアウト</param>
		/// <param name="staff">スタッフ</param>
		public ConfirmViewModel(ActionStatus actionStatus, string shopId, string pageLayout, StaffModel staff = null)
		{
			this.Image = new ImageInput();
			this.PageLayout = pageLayout;
			this.ActionStatus = actionStatus;
			SetModel(actionStatus, shopId, staff);
		}

		/// <summary>
		/// スタッフ情報セット
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="staff">スタッフ</param>
		private void SetModel(ActionStatus actionStatus, string shopId, StaffModel staff)
		{
			if (staff == null) return;

			this.StaffId = staff.StaffId;
			this.StaffName = staff.StaffName;
			this.StaffHeight = staff.StaffHeight;
			this.StaffProfile = staff.StaffProfile;
			this.StaffSex = ValueText.GetValueText(
				Constants.TABLE_STAFF,
				Constants.FIELD_STAFF_STAFF_SEX,
				staff.StaffSex);
			this.ModelFlg = ValueText.GetValueText(
				Constants.TABLE_STAFF,
				Constants.FIELD_STAFF_MODEL_FLG,
				staff.ModelFlg);
			this.OperatorId = staff.OperatorId;
			this.RealShopId = staff.RealShopId;
			this.StaffInstagramId = staff.StaffInstagramId;
			this.ValidFlg = ValueText.GetValueText(
				Constants.TABLE_STAFF,
				Constants.FIELD_STAFF_VALID_FLG,
				staff.ValidFlg);
			this.StaffThumbnail = StaffWorkerService.GetStaffThumbnail(staff.StaffId);
			if (actionStatus == ActionStatus.Detail)
			{
				this.DateCreated = DateTimeUtility.ToStringForManager(staff.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
				this.DateChanged = DateTimeUtility.ToStringForManager(staff.DateChanged, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
				this.LastChanged = staff.LastChanged;
			}
		}

		/// <summary>スタッフID</summary>
		public string StaffId { get; set; }
		/// <summary>スタッフ名</summary>
		public string StaffName { get; set; }
		/// <summary>身長</summary>
		public int StaffHeight { get; set; }
		/// <summary>性別</summary>
		public string StaffSex { get; set; }
		/// <summary>プロフィールテキスト</summary>
		public string StaffProfile { get; set; }
		/// <summary>モデルフラグ</summary>
		public string ModelFlg { get; set; }
		/// <summary>オペレータID</summary>
		public string OperatorId { get; set; }
		/// <summary>リアル店舗ID</summary>
		public string RealShopId { get; set; }
		/// <summary>インスタグラムID</summary>
		public string StaffInstagramId { get; set; }
		/// <summary>サムネイル</summary>
		public string StaffThumbnail { get; set; }
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; set; }
		/// <summary>作成日</summary>
		public string DateCreated { get; set; }
		/// <summary>更新日</summary>
		public string DateChanged { get; set; }
		/// <summary>最終更新者</summary>
		public string LastChanged { get; set; }
		/// <summary>イメージ</summary>
		public ImageInput Image { get; set; }
		/// <summary>ページレイアウト</summary>
		public string PageLayout { get; set; }
	}
}