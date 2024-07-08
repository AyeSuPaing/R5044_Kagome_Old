/*
=========================================================================================================
  Module      : GMOアトカラ 与信審査結果取得 APIクラス(PaymentGmoAtokaraGetAuthorizationResultApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Linq;
using w2.App.Common.Order.Payment.GMOAtokara.Utils;

namespace w2.App.Common.Order.Payment.GMOAtokara
{
	/// <summary>
	/// GMOアトカラ 与信審査結果取得 APIクラス
	/// </summary>
	public class PaymentGmoAtokaraGetAuthorizationResultApi : PaymentGmoAtokaraBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentGmoAtokaraGetAuthorizationResultApi()
			: this(PaymentGmoAtokaraSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">設定</param>
		public PaymentGmoAtokaraGetAuthorizationResultApi(
			PaymentGmoAtokaraSetting settings)
			: base(Constants.PAYMENT_GMOATOKARA_DEFERRED_URL_GETAUTHRESULT, settings, new PaymentGmoAtokaraGetAuthorizationResultResponseData())
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="gmoTransactionId">GMO取引ID</param>
		/// <returns>実行結果</returns>
		public bool Exec(string gmoTransactionId)
		{
			return Exec(
				gmoTransactionId,
				PaymentGmoAtokaraConstants.TRANSACTIONTYPE_CLOSING);
		}
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="gmoTransactionId">GMO取引ID</param>
		/// <param name="transactionType">取引種別</param>
		/// <returns>実行結果</returns>
		private bool Exec(
			string gmoTransactionId,
			string transactionType)
		{
			var requestXml = CreateRequestXml(
				gmoTransactionId,
				transactionType);

			return Post(requestXml);
		}

		/// <summary>
		/// リクエストXML作成
		/// </summary>
		/// <param name="gmoTransactionId">GMO取引ID</param>
		/// <param name="transactionType">取引種別</param>
		/// <returns>リクエストXML</returns>
		private XDocument CreateRequestXml(
			string gmoTransactionId,
			string transactionType)
		{
			var document = new XDocument(new XDeclaration("1.0", "UTF-8", ""));
			document.Add(
				new XElement("request",
					CreateShopInfoNode(),
					new XElement("transaction",
						new XElement("gmoTransactionId", gmoTransactionId),
						new XElement("transactionType", transactionType)
					)
				));

			return document;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentGmoAtokaraGetAuthorizationResultResponseData ResponseData { get { return (PaymentGmoAtokaraGetAuthorizationResultResponseData)this.ResponseDataInner; } }
	}
}
