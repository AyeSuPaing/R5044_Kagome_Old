/*
=========================================================================================================
  Module      : GmoKb Get Frame Guarantee Status Command (GmoKbGetFrameGuaranteeStatusCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.FrameGuarantee;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Database.Common;
using w2.Domain.UserBusinessOwner;

namespace w2.Commerce.Batch.GmoKbGetFrameGuaranteeStatus
{
	/// <summary>
	/// 枠保証のステータス取得
	/// </summary>
	public class GmoKbGetFrameGuaranteeStatusCommand
	{
		/// <summary>
		/// 実行
		/// </summary>
		public void Exec()
		{
			var listFrameGuarantees = GetFrameGuaranteeNeedUpdate();
			if (listFrameGuarantees != null)
			{
				foreach (var item in listFrameGuarantees)
				{
					try
					{
						var request = new GmoRequestFrameGuaranteeGetStatus(item);
						var result = new GmoTransactionApi().FrameGuaranteeGetStatus(request);
						if (result.IsResultOk)
						{
							var status = result.Examination.Status.ToString();
							new UserBusinessOwnerService().UpdateCreditStatus(item.UserId, status);
						}
						else if (result.IsResultNg)
						{
							AppLogger.WriteInfo(result.Errors.Error[0].ErrorMessage);
						}
					}
					catch (Exception ex)
					{
						AppLogger.WriteInfo(ex);
					}
				}
			}
		}

		/// <summary>
		/// 審査中のGMOユーザーリスト取得
		/// </summary>
		/// <returns>ユーザーリスト</returns>
		private UserBusinessOwnerModel[] GetFrameGuaranteeNeedUpdate()
		{
			var listUserBusinessOwners = new UserBusinessOwnerService().GetFrameGuaranteeNeedUpdate();
			return listUserBusinessOwners;
		}
	}
}
