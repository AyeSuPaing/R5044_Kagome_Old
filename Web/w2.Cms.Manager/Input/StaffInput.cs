/*
=========================================================================================================
  Module      : スタッフ入力クラス (StaffInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Input;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Domain.Staff;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// スタッフ入力クラス
	/// </summary>
	public class StaffInput : InputBase<StaffModel>
	{
		#region メソッド

		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override StaffModel CreateModel()
		{
			var model = new StaffModel
			{
				StaffId = this.StaffId,
				StaffName = this.StaffName,
				StaffProfile = this.StaffProfile,
				StaffHeight = int.Parse(this.StaffHeight),
				StaffInstagramId = this.StaffInstagramId,
				StaffSex = this.StaffSex,
				ModelFlg = this.ModelFlg
					? Constants.FLG_STAFF_MODEL_FLG_VALID
					: Constants.FLG_STAFF_MODEL_FLG_INVALID,
				OperatorId = this.OperatorId,
				RealShopId = this.RealShopId,
				ValidFlg = this.ValidFlg
					? Constants.FLG_STAFF_VALID_FLG_VALID
					: Constants.FLG_STAFF_VALID_FLG_INVALID,
				LastChanged = this.LastChanged,
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(ActionStatus actionStatus)
		{
			var errorMessage = Validator.Validate(
				(actionStatus == ActionStatus.Insert)
					? "StaffRegister"
					: (actionStatus == ActionStatus.Update)
						? "StaffModify"
						: "",
				this.DataSource);
			return errorMessage;
		}

		#endregion

		#region プロパティ

		/// <summary>スタッフID</summary>
		public string StaffId
		{
			get { return (string) this.DataSource[Constants.FIELD_STAFF_STAFF_ID]; }
			set { this.DataSource[Constants.FIELD_STAFF_STAFF_ID] = value; }
		}
		/// <summary>氏名</summary>
		public string StaffName
		{
			get { return (string) this.DataSource[Constants.FIELD_STAFF_STAFF_NAME]; }
			set { this.DataSource[Constants.FIELD_STAFF_STAFF_NAME] = value; }
		}
		/// <summary>プロフィールテキスト</summary>
		public string StaffProfile
		{
			get { return (string) this.DataSource[Constants.FIELD_STAFF_STAFF_PROFILE]; }
			set { this.DataSource[Constants.FIELD_STAFF_STAFF_PROFILE] = value; }
		}
		/// <summary>身長</summary>
		public string StaffHeight
		{
			get { return (string) this.DataSource[Constants.FIELD_STAFF_STAFF_HEIGHT]; }
			set { this.DataSource[Constants.FIELD_STAFF_STAFF_HEIGHT] = value; }
		}
		/// <summary>インスタグラムID</summary>
		public string StaffInstagramId
		{
			get { return (string) this.DataSource[Constants.FIELD_STAFF_STAFF_INSTAGRAM_ID]; }
			set { this.DataSource[Constants.FIELD_STAFF_STAFF_INSTAGRAM_ID] = value; }
		}
		/// <summary>モデルフラグ</summary>
		public bool ModelFlg
		{
			get { return (bool)(this.DataSource[Constants.FIELD_STAFF_MODEL_FLG] ?? false); }
			set { this.DataSource[Constants.FIELD_STAFF_MODEL_FLG] = value; }
		}
		/// <summary>性別</summary>
		public string StaffSex
		{
			get { return (string) this.DataSource[Constants.FIELD_STAFF_STAFF_SEX]; }
			set { this.DataSource[Constants.FIELD_STAFF_STAFF_SEX] = value; }
		}
		/// <summary>オペレータID</summary>
		public string OperatorId
		{
			get { return (string) this.DataSource[Constants.FIELD_STAFF_OPERATOR_ID]; }
			set { this.DataSource[Constants.FIELD_STAFF_OPERATOR_ID] = value; }
		}
		/// <summary>リアル店舗ID</summary>
		public string RealShopId
		{
			get { return (string) this.DataSource[Constants.FIELD_STAFF_REAL_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_STAFF_REAL_SHOP_ID] = value; }
		}
		/// <summary>有効フラグ</summary>
		public bool ValidFlg
		{
			get { return (bool) (this.DataSource[Constants.FIELD_STAFF_VALID_FLG] ?? false); }
			set { this.DataSource[Constants.FIELD_STAFF_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string) this.DataSource[Constants.FIELD_STAFF_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_STAFF_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string) this.DataSource[Constants.FIELD_STAFF_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_STAFF_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string) this.DataSource[Constants.FIELD_STAFF_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_STAFF_LAST_CHANGED] = value; }
		}
		/// <summary>登録か</summary>
		public bool IsInsert { get; set; }
		#endregion
	}
}