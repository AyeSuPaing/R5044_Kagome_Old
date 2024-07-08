/*
=========================================================================================================
  Module      : パスワード変更ワーカーサービス(OperatorPasswordChangeWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Cms.Manager.ViewModels.OperatorPasswordChange;
using w2.Domain.ShopOperator;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// パスワード変更ワーカーサービス
	/// </summary>
	public class OperatorPasswordChangeWorkerService : BaseWorkerService
	{
		/// <summary>
		/// パスワード変更ビューモデル作成
		/// </summary>
		/// <returns>ビューモデル</returns>
		internal InputViewModel CreateInputVm()
		{
			return new InputViewModel(this.SessionWrapper.LoginShopId, this.SessionWrapper.LoginOperator.OperatorId);
		}

		/// <summary>
		/// パスワード変更
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="password">パスワード</param>
		/// <param name="operatorName">オペレータ名</param>
		internal void ChangeOperatorPassword(string shopId, string operatorId, string password, string operatorName)
		{
			var service = new ShopOperatorService();
			var shopOperator = service.Get(shopId, operatorId);
			shopOperator.Password = password;
			shopOperator.LastChanged = operatorName;
			service.Update(shopOperator);
		}
	}
}