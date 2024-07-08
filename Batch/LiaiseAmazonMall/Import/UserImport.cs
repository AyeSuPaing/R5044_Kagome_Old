/*
=========================================================================================================
  Module      : ユーザ情報取込クラス(UserImport.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Sql;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Commerce.Batch.LiaiseAmazonMall.Amazon;

namespace w2.Commerce.Batch.LiaiseAmazonMall.Import
{
	/// <summary>
	/// ユーザ情報取込クラス
	/// </summary>
	public class UserImport : ImportBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="amazonOrder">Amazon注文情報</param>
		/// <param name="mallId">モールID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public UserImport(AmazonOrderModel amazonOrder, string mallId, SqlAccessor accessor)
			: base(amazonOrder, mallId, accessor)
		{
		}
		#endregion

		#region +Import 取込
		/// <summary>
		/// 取込
		/// </summary>
		public override void Import()
		{
			try
			{
				// DB登録
				var service = new UserService();
				if (this.AmazonOrder.IsNewUser)
				{
					var user = CreateImportData();
					service.Insert(user, UpdateHistoryAction.Insert, this.Accessor);
				}
				else
				{
					var user = CreateUpdateData();
					service.Update(user, UpdateHistoryAction.Insert, this.Accessor);
				}

				// 保留注文取込時の仮ユーザを削除する必要がある場合は削除
				if (string.IsNullOrEmpty(this.AmazonOrder.DeleteNecessaryUserId) == false)
				{
					service.Delete(this.AmazonOrder.DeleteNecessaryUserId, Constants.FLG_LASTCHANGED_BATCH, UpdateHistoryAction.DoNotInsert, this.Accessor);
				}
			}
			catch
			{
				throw;
			}
		}
		#endregion

		#region -CreateImportData 取込情報作成
		/// <summary>
		/// 取込情報作成
		/// </summary>
		/// <returns>ユーザモデル</returns>
		private UserModel CreateImportData()
		{
			var model = new UserModel
			{
				UserId = this.AmazonOrder.UserId,
				UserKbn = this.AmazonOrder.UserKbn,
				MallId = this.MallId,
				Name = this.AmazonOrder.Order.IsSetBuyerName() ? this.AmazonOrder.Order.BuyerName : string.Empty,
				MailAddr = this.AmazonOrder.Order.IsSetBuyerEmail() ? this.AmazonOrder.Order.BuyerEmail : string.Empty,
				MemberRankId = this.AmazonOrder.DefaultMemberRankId,
				DateCreated = DateTime.Now,
				DateChanged = DateTime.Now,
				LastChanged = Constants.FLG_LASTCHANGED_BATCH,
				FixedPurchaseMemberFlg = Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF
			};
			return model;
		}
		#endregion

		#region -CreateUpdateData 更新情報作成
		/// <summary>
		/// 更新情報作成
		/// </summary>
		/// <returns>ユーザモデル</returns>
		private UserModel CreateUpdateData()
		{
			var model = new UserService().Get(this.AmazonOrder.UserId);
			model.Name = this.AmazonOrder.Order.IsSetBuyerName() ? this.AmazonOrder.Order.BuyerName : model.Name;
			model.MailAddr = this.AmazonOrder.Order.IsSetBuyerEmail() ? this.AmazonOrder.Order.BuyerEmail : model.MailAddr;
			model.DateChanged = DateTime.Now;
			model.LastChanged = Constants.FLG_LASTCHANGED_BATCH;

			return model;
		}
		#endregion
	}
}
