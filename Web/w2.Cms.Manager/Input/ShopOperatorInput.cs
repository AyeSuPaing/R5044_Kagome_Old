/*
=========================================================================================================
  Module      : 店舗管理者入力クラス(ShopOperatorInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Input;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Domain.ShopOperator;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// 店舗管理者入力クラス
	/// </summary>
	public class ShopOperatorInput : InputBase<ShopOperatorModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ShopOperatorInput()
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override ShopOperatorModel CreateModel()
		{
			var model = new ShopOperatorModel
			{
				ShopId = this.ShopId,
				OperatorId = this.OperatorId,
				Name = this.Name,
				MenuAccessLevel4 = this.MenuAccessLevel,	// 他のところも更新しないといけないのではないかにゃ？（更新時）
				LoginId = this.LoginId,
				Password = this.Password,
				MailAddr = this.MailAddr,
				ValidFlg = this.ValidFlg ? Constants.FLG_SHOPOPERATOR_VALID_FLG_VALID : Constants.FLG_SHOPOPERATOR_VALID_FLG_INVALID,
				LastChanged = this.LastChanged,
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate(ActionStatus actionStatus)
		{
			var errorMessage = Validator.Validate(
				(actionStatus == ActionStatus.Insert)
					? "ShopOperatorRegister"
					: (actionStatus == ActionStatus.Update)
						? "ShopOperatorModify"
						: "",
				this.DataSource);
			return errorMessage;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		[BindAlias(Constants.REQUEST_KEY_SHOP_ID)]
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_SHOP_ID] = value; }
		}
		/// <summary>オペレータID</summary>
		public string OperatorId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID] = value; }
		}
		/// <summary>オペレータ名</summary>
		public string Name
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_NAME]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_NAME] = value; }
		}
		/// <summary>メニューアクセスレベル</summary>
		public int? MenuAccessLevel
		{
			get { return (int?)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL1]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL1] = value; }
		}
		/// <summary>ログインID</summary>
		public string LoginId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_LOGIN_ID]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_LOGIN_ID] = value; }
		}
		/// <summary>パスワード</summary>
		public string Password
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_PASSWORD]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_PASSWORD] = value; }
		}
		/// <summary>有効フラグ</summary>
		public bool ValidFlg
		{
			get { return (bool)(this.DataSource[Constants.FIELD_SHOPOPERATOR_VALID_FLG] ?? false); }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_VALID_FLG] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_LAST_CHANGED] = value; }
		}
		/// <summary>登録か</summary>
		public bool IsInsert { get; set; }
		/// <summary>メールアドレス</summary>
		public string MailAddr
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_MAIL_ADDR]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MAIL_ADDR] = value; }
		}
		#endregion
	}
}
