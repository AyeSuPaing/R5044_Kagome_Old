/*
=========================================================================================================
  Module      : ポイントルール入力情報(AdvCodeMediaTypeInput.cs)
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
	/// 広告媒体区分マスタ入力クラス
	/// </summary>
	public class AdvCodeMediaTypeInput : InputBase<AdvCodeMediaTypeModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AdvCodeMediaTypeInput()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public AdvCodeMediaTypeInput(AdvCodeMediaTypeModel model)
			: this()
		{
			this.AdvcodeMediaTypeId = model.AdvcodeMediaTypeId;
			this.AdvcodeMediaTypeName = model.AdvcodeMediaTypeName;
			this.DisplayOrder = model.DisplayOrder.ToString();
			this.DateCreated = model.DateCreated.ToString();
			this.DateChanged = model.DateChanged.ToString();
			this.LastChanged = model.LastChanged;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override AdvCodeMediaTypeModel CreateModel()
		{
			var model = new AdvCodeMediaTypeModel
			{
				AdvcodeMediaTypeId = this.AdvcodeMediaTypeId,
				AdvcodeMediaTypeName = this.AdvcodeMediaTypeName,
				DisplayOrder = int.Parse(this.DisplayOrder),
				LastChanged = this.LastChanged,
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate(string actionStatus)
		{
			var errorMessage = string.Empty;

			if (actionStatus == Constants.ACTION_STATUS_INSERT)
			{
				errorMessage = Validator.Validate("AdvCodeMediaTypeRegist", this.DataSource);
			}
			else
			{
				errorMessage = Validator.Validate("AdvCodeMediaTypeModify", this.DataSource);
			}

			return errorMessage;
		}
		#endregion

		#region プロパティ
		/// <summary>区分ID</summary>
		public string AdvcodeMediaTypeId
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID]; }
			set { this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID] = value; }
		}
		/// <summary>媒体区分名</summary>
		public string AdvcodeMediaTypeName
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME]; }
			set { this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME] = value; }
		}
		/// <summary>表示順</summary>
		public string DisplayOrder
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_DISPLAY_ORDER] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}