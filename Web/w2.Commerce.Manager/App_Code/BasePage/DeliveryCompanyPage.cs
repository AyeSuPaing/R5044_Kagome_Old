/*
=========================================================================================================
  Module      : 配送会社情報基底ページ(DeliveryCompanyPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Text;
using System.Web;
using w2.Common.Web;
using w2.Domain.DeliveryCompany;
using w2.Domain.DeliveryLeadTime;

/// <summary>
/// 配送会社情報基底ページ
/// </summary>
public class DeliveryCompanyPage : BasePage
{
	/// <summary>
	/// 配送会社一覧ページURL
	/// </summary>
	/// <param name="deliveryCompanyId">配送会社ID</param>
	/// <param name="pageManager">ページ</param>
	/// <param name="actionStatus">アクション</param>
	/// <returns>Url</returns>
	protected string CreateDeliveryCompanyUrl(string deliveryCompanyId, string pageManager, string actionStatus)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + pageManager)
			.AddParam(Constants.REQUEST_KEY_DELIVERY_COMPANY_ID, deliveryCompanyId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, actionStatus);

		return url.CreateUrl();
	}

	/// <summary>
	/// 出荷連携配送会社を設定する決済区分かどうか(クレジットカード)
	/// </summary>
	/// <returns>結果</returns>
	protected bool IsDeliveryCompanyTypeCreditcardByPaymentKbn()
	{
		return (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc);
	}

	/// <summary>
	/// 出荷連携配送会社を設定する決済区分かどうか(後払い)
	/// </summary>
	/// <returns>結果</returns>
	protected bool IsDeliveryCompanyTypePostPaymentByPaymentKbn()
	{
		return ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.YamatoKa)
			|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Gmo)
			|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atodene)
			|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk)
			|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom)
			|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score));
	}

	/// <summary>配送会社サービス</summary>
	protected DeliveryCompanyService DeliveryCompanyService
	{
		get { return new DeliveryCompanyService(); }
	}
	/// <summary>配送会社ID</summary>
	protected string DeliveryCompanyId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_DELIVERY_COMPANY_ID]); }
	}
	/// <summary>Delivery lead time service</summary>
	protected DeliveryLeadTimeService DeliveryLeadTimeService
	{
		get { return new DeliveryLeadTimeService(); }
	}
}