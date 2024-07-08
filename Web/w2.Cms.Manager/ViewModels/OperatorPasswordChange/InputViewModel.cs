/*
=========================================================================================================
  Module      : パスワード変更入力ビューモデル(InputViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Cms.Manager.Input;

namespace w2.Cms.Manager.ViewModels.OperatorPasswordChange
{
	/// <summary>
	/// パスワード変更入力ビューモデル
	/// </summary>
	public class InputViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="operatorId">オペレータID</param>
		public InputViewModel(string shopId, string operatorId)
		{
			this.Input = new OperatorPasswordChangeInput
			{
				ShopId = shopId,
				OperatorId = operatorId
			};
		}

		/// <summary>パスワード変更入力</summary>
		public OperatorPasswordChangeInput Input { get; set; }
	}
}