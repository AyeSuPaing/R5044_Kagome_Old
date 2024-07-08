/*
=========================================================================================================
  Module      : つくーるAPI連携：ユーザークレジットカード情報登録 (UserCreditCardImport.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Util;
using w2.Common.Sql;
using w2.Domain.UserCreditCard;
using w2.Commerce.Batch.ExternalOrderImport.Entity;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.ExternalOrderImport.Import.UreruAd
{
	/// <summary>
	/// つくーるAPI連携：ユーザークレジットカード情報登録
	/// </summary>
	public class UserCreditCardImport : UreruAdImportBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseData">レスポンスデータ</param>
		/// <param name="accessor">SQLアクセサ</param>
		public UserCreditCardImport(UreruAdResponseDataItem responseData, SqlAccessor accessor)
			: base(responseData, accessor)
		{
		}

		/// <summary>
		/// 更新・登録
		/// </summary>
		public override void Import()
		{
			if (ValueText.GetValueText(
				Constants.TABLE_ORDER, 
				Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, 
				this.ResponseData.PaymentMethod) != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) return;

			var service = new UserCreditCardService();
			var userCreditCard = CreateImportData();
			this.ResponseData.CreditBranchNo = service.Insert(userCreditCard, UpdateHistoryAction.Insert, this.Accessor);
		}

		/// <summary>
		/// インポートデータ生成
		/// </summary>
		/// <returns>ユーザークレジットカード情報</returns>
		private UserCreditCardModel CreateImportData()
		{
			var userCreditCard = new UserCreditCardModel
			{
				UserId = this.ResponseData.User.UserId,
				CooperationId = this.ResponseData.GetCooperationId(),
				CardDispName = Constants.URERU_AD_IMPORT_USER_CREDIT_CARD_CARD_DISP_NAME,
				LastFourDigit = Constants.URERU_AD_IMPORT_USER_CREDIT_CARD_LAST_FOUR_DIGIT,
				ExpirationMonth = Constants.URERU_AD_IMPORT_USER_CREDIT_CARD_EXPIRATION_MONTH,
				ExpirationYear = Constants.URERU_AD_IMPORT_USER_CREDIT_CARD_EXPIRATION_YEAR,
				AuthorName = Constants.URERU_AD_IMPORT_USER_CREDIT_CARD_AUTHOR_NAME,
				LastChanged = Constants.FLG_LASTCHANGED_BATCH,
				CompanyCode = string.Empty,
				CooperationId2 = this.ResponseData.GetCooperationId2()
			};
			return userCreditCard;
		}
	}
}
