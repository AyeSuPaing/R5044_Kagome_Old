/*
=========================================================================================================
  Module      : パスワード変更入力クラス(OperatorPasswordChangeInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.App.Common;
using w2.Cms.Manager.Codes.Common;
using w2.Domain.ShopOperator;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// パスワード変更入力クラス
	/// </summary>
	public class OperatorPasswordChangeInput
	{
		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		internal string Validate()
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SHOPOPERATOR_PASSWORD + "_old", this.PasswordOld },
				{ Constants.FIELD_SHOPOPERATOR_PASSWORD, this.Password },
				{ Constants.FIELD_SHOPOPERATOR_PASSWORD + "_conf", this.PasswordConfirm },
			};

			var errorMessage = Validator.Validate("ShopOperatorPasswordChange", input);

			var shopOperator = new ShopOperatorService().Get(this.ShopId, this.OperatorId);
			errorMessage = (shopOperator.Password != this.PasswordOld)
				? WebMessages.ShopOperatorNoOperatorError
				: errorMessage;
			return errorMessage;
		}

		/// <summary>現在のパスワード</summary>
		public string PasswordOld { get; set; }
		/// <summary>新しいパスワード</summary>
		public string Password { get; set; }
		/// <summary>新しいパスワード(確認)</summary>
		public string PasswordConfirm { get; set; }
		/// <summary>店舗ID</summary>
		public string ShopId { get; set; }
		/// <summary>オペレータID</summary>
		public string OperatorId { get; set; }
	}
}