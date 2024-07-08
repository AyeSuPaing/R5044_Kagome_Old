/*
=========================================================================================================
  Module      : GMOクレジット決済モジュール(PaymentGmoCredit.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Register;
using w2.App.Common.ShopMessage;
using w2.Common.Sql;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.GMO
{
	///*********************************************************************************************
	/// <summary>
	/// GMOクレジット決済モジュール
	/// </summary>
	///*********************************************************************************************
	public class PaymentGmoCredit : PaymentGmo, IPaymentGmoCredit
	{
		#region 定数

		//------------------------------------------------------
		// URL
		//------------------------------------------------------
		/// <summary>取引登録</summary>
		const string URL_ENTRYTRAN = "/payment/EntryTran.idPass";
		/// <summary>決済実行</summary>
		const string URL_EXECTRAN = "/payment/ExecTran.idPass";
		/// <summary>3DS1.0認証後決済実行</summary>
		const string URL_SECURETRAN = "/payment/SecureTran.idPass";
		/// <summary>3DS2.0認証後決済実行</summary>
		const string URL_SECURETRAN2 = "/payment/SecureTran2.idPass";
		/// <summary>仮売上⇒実売上実行 / 取り消し</summary>
		const string URL_EXECALTERTRAN = "/payment/AlterTran.idPass";
		/// <summary>会員登録</summary>
		const string URL_SAVEMENBER = "/payment/SaveMember.idPass";
		/// <summary>会員参照</summary>
		const string URL_SEARCHMEMBER = "/payment/SearchMember.idPass";
		/// <summary>カード登録・更新</summary>
		const string URL_SAVECARD = "/payment/SaveCard.idPass";
		/// <summary>カード参照</summary>
		const string URL_SEARCHCARD = "/payment/SearchCard.idPass";
		/// <summary>取引状態参照</summary>
		const string URL_SEARCHTRADE = "/payment/SearchTrade.idPass";
		/// <summary>取引金額変更</summary>
		const string URL_CHANGETRAN = "/payment/ChangeTran.idPass";
		/// <summary>決済後カード登録</summary>
		const string URL_TRADEDCARD = "/payment/TradedCard.idPass";

		//------------------------------------------------------
		// 処理区分
		//------------------------------------------------------
		/// <summary>有効性チェック</summary>
		const string KBN_JOB_CD_CHECK = "CHECK";
		/// <summary>即時売上</summary>
		const string KBN_JOB_CD_CAPTURE = "CAPTURE";
		/// <summary>仮売上</summary>
		const string KBN_JOB_CD_AUTH = "AUTH";
		/// <summary>簡易オーソリ（未対応）</summary>
		//const string JOB_CD_SAUTH = "SAUTH";
		/// <summary>仮売上⇒実売上実行</summary>
		const string KBN_JOB_CD_SALES = "SALES";
		/// <summary>決済取り消し</summary>
		const string KBN_JOB_CD_VOID = "VOID";
		/// <summary>返品</summary>
		const string KBN_JOB_CD_RETURN = "RETURN";
		/// <summary>月跨り返品</summary>
		const string KBN_JOB_CD_RETURNX = "RETURNX";

		//------------------------------------------------------
		// 支払方法
		//------------------------------------------------------
		/// <summary>一括</summary>
		const string KBN_PAYMENTMETHOD_SINGLE = "1";
		/// <summary>分割</summary>
		const string KBN_PAYMENTMETHOD_INSTALLMENT = "2";
		/// <summary>リボ</summary>
		const string KBN_PAYMENTMETHOD_REVOLVING = "5";

		//------------------------------------------------------
		// カード登録連番モード
		//------------------------------------------------------
		/// <summary>物理モード</summary>
		const string KBN_SEQMODE_PHYSICAL = "1";

		//------------------------------------------------------
		// 削除フラグ
		//------------------------------------------------------
		/// <summary>削除</summary>
		const string KBN_DELETEFLAG_DEL = "1";
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentGmoCredit()
			: this(
				Constants.PAYMENT_SETTING_GMO_AUTH_SERVER_URL,
				Constants.PAYMENT_SETTING_GMO_SITE_ID,
				Constants.PAYMENT_SETTING_GMO_SITE_PASS,
				Constants.PAYMENT_SETTING_GMO_SHOP_ID,
				Constants.PAYMENT_SETTING_GMO_SHOP_PASS,
				Constants.PAYMENT_SETTING_GMO_SHOP_NAME,
				Constants.PAYMENT_SETTING_GMO_TDS2_TYPE,
				(Constants.GmoCreditCardPaymentMethod)Constants.PAYMENT_SETTING_GMO_PAYMENTMETHOD)
		{
			// 何もしない //
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="serverUrl">決済サーバURL</param>
		/// <param name="siteId">サイトID</param>
		/// <param name="sitePass">サイトパスワード</param>
		/// <param name="shopId">ショップID</param>
		/// <param name="shopPass">ショップパスワード</param>
		/// <param name="shopName">店舗名</param>
		/// <param name="tds2Type">3DS2.0未対応時取り扱い</param>
		/// <param name="jobCd">決済方法</param>
		public PaymentGmoCredit(
			string serverUrl,
			string siteId,
			string sitePass,
			string shopId,
			string shopPass,
			string shopName,
			string tds2Type,
			Constants.GmoCreditCardPaymentMethod jobCd)
		{
			// 文字エンコードの指定
			var eAuthorizationEncoding = Encoding.GetEncoding("EUC-JP");
			this.ServerUrl = serverUrl;
			this.SiteId = siteId;
			this.SitePass = sitePass;
			this.ShopId = shopId;
			this.ShopPass = shopPass;
			this.ShopName = Convert.ToBase64String(eAuthorizationEncoding.GetBytes(shopName));
			this.Tds2Type = tds2Type;
			this.JobCd = jobCd.ToString().ToUpper();
		}

		/// <summary>
		/// 取引登録実行
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="priceTotal">合計金額</param>
		/// <param name="execTypes">注文実行種別(未指定時は管理画面)</param>
		/// <returns>True:成功、False:失敗</returns>
		public bool EntryTran(
			string gmoOrderId,
			decimal priceTotal,
			OrderRegisterBase.ExecTypes execTypes = OrderRegisterBase.ExecTypes.CommerceManager)
		{
			// POST送信パラメタ作成
			var parameters = new NameValueCollection()
			{
				{PARAM_SHOPID, this.ShopId},
				{PARAM_SHOPPASS, this.ShopPass},
				{PARAM_ORDERID, gmoOrderId},
				{PARAM_JOBCD, this.JobCd}
			};

			// 3Dセキュアを使用する場合
			if (Constants.PAYMENT_SETTING_GMO_3DSECURE
				&& (execTypes == OrderRegisterBase.ExecTypes.Pc))
			{
				parameters.Add(PARAM_TDFLAG, "2");
				parameters.Add(PARAM_TDTENANTNAME, this.ShopName);
				parameters.Add(PARAM_TDS2TYPE, this.Tds2Type);
			}

			// 有効性チェック以外の場合
			if (this.JobCd != KBN_JOB_CD_CHECK)
			{
				parameters.Add(PARAM_AMOUNT, priceTotal.ToPriceString());
			}

			// 取引登録実行
			if (SendParams(URL_ENTRYTRAN, parameters, "", gmoOrderId))
			{
				// 成功
				// 取引ID、取引パスワード取得（決済取引IDに” ”区切りで格納する）
				this.AccessId = (string)this.Result.Parameters[PARAM_ACCESSID];
				this.AccessPass = (string)this.Result.Parameters[PARAM_ACCESSPASS];

				return true;
			}
			else
			{
				// 失敗
				return false;
			}
		}

		/// <summary>
		/// 決済実行（カード入力）
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="creditCardNo">カード番号</param>
		/// <param name="creditCardExpire">有効期限</param>
		/// <param name="creditCardInstallments">支払方法</param>
		/// <param name="securityCode">セキュリティコード</param>
		/// <returns>True:成功、False:失敗</returns>
		[Obsolete("古い形式です。Tokenを利用した決済実行を利用するようにしてください。")]
		public bool ExecTran(
			string gmoOrderId,
			string creditCardNo,
			string creditCardExpire,
			string creditCardInstallments,
			string securityCode)
		{
			// 支払回数コンバート
			var installments = ConvertCardInstallments(creditCardInstallments);

			// POST送信パラメタ作成
			var parameters = new NameValueCollection()
			{
				{PARAM_ACCESSID, this.AccessId},
				{PARAM_ACCESSPASS, this.AccessPass},
				{PARAM_ORDERID, gmoOrderId}
			};
			// 有効性チェック以外の場合
			if (this.JobCd != KBN_JOB_CD_CHECK)
			{
				parameters.Add(PARAM_METHOD, installments.Key);
				// 分割払いの場合
				if (installments.Key == KBN_PAYMENTMETHOD_INSTALLMENT)
				{
					parameters.Add(PARAM_PAYTIMES, installments.Value);
				}
			}
			parameters.Add(PARAM_CARDNO, creditCardNo);
			parameters.Add(PARAM_EXPIRE, creditCardExpire);
			// セキュリティコード有効？
			if (Constants.PAYMENT_SETTING_GMO_SECURITYCODE)
			{
				parameters.Add(PARAM_SECURITYCODE, securityCode);
			}

			// 決済実行（カード入力）
			return SendParams(URL_EXECTRAN, parameters, "", gmoOrderId);
		}

		/// <summary>
		/// 決済実行（登録カード利用）
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="gmoMemberId">GMO会員ID</param>
		/// <param name="creditCardInstallments">支払方法</param>
		/// <param name="securityCode">セキュリティコード</param>
		/// <param name="returnUrl">戻りURL</param>
		/// <returns>True:成功、False:失敗</returns>
		public bool ExecTranUseCard(
			string gmoOrderId,
			string cardTranId,
			string gmoMemberId,
			string creditCardInstallments,
			string securityCode,
			string returnUrl = null)
		{
			// 支払回数コンバート
			var installments = ConvertCardInstallments(creditCardInstallments);

			// POST送信パラメタ作成
			var accessId = cardTranId.Split(' ')[0];
			var accessPass = cardTranId.Split(' ')[1];
			var parameters = new NameValueCollection()
			{
				{PARAM_ACCESSID, accessId},
				{PARAM_ACCESSPASS, accessPass},
				{PARAM_ORDERID, gmoOrderId},
				{PARAM_SITEID, this.SiteId},
				{PARAM_SITEPASS, this.SitePass},
				{PARAM_MEMBERID, gmoMemberId},
				{PARAM_SEQMODE, KBN_SEQMODE_PHYSICAL},
				{PARAM_CARDSEQ, "0"}
			};
			// 有効性チェック以外の場合
			if (this.JobCd != KBN_JOB_CD_CHECK)
			{
				parameters.Add(PARAM_METHOD, installments.Key);
				// 分割払いの場合
				if (installments.Key == KBN_PAYMENTMETHOD_INSTALLMENT)
				{
					parameters.Add(PARAM_PAYTIMES, installments.Value);
				}
			}
			// セキュリティコード有効？
			if (Constants.PAYMENT_SETTING_GMO_SECURITYCODE)
			{
				parameters.Add(PARAM_SECURITYCODE, securityCode);
			}

			if (returnUrl != null)
			{
				parameters.Add(PARAM_RET_URL, returnUrl);
			}

			// 決済実行（登録カード利用）
			return SendParams(URL_EXECTRAN, parameters, gmoMemberId, gmoOrderId);
		}

		/// <summary>
		/// 決済実行（トークン利用）
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="creditCardInstallments">支払方法</param>
		/// <param name="token">カードトークン</param>
		/// <param name="returnUrl">戻りURL</param>
		/// <returns>True:成功、False:失敗</returns>
		public bool ExecTran(
			string gmoOrderId,
			string creditCardInstallments,
			string token,
			string returnUrl = null)
		{
			// 支払回数コンバート
			var installments = ConvertCardInstallments(creditCardInstallments);

			// POST送信パラメタ作成
			var parameters = new NameValueCollection()
			{
				{PARAM_ACCESSID, this.AccessId},
				{PARAM_ACCESSPASS, this.AccessPass},
				{PARAM_ORDERID, gmoOrderId}
			};
			// 有効性チェック以外の場合
			if (this.JobCd != KBN_JOB_CD_CHECK)
			{
				parameters.Add(PARAM_METHOD, installments.Key);
				// 分割払いの場合
				if (installments.Key == KBN_PAYMENTMETHOD_INSTALLMENT)
				{
					parameters.Add(PARAM_PAYTIMES, installments.Value);
				}
			}
			parameters.Add(PARAM_TOKEN, token);
			if (returnUrl != null)
			{
				parameters.Add(PARAM_RET_URL, returnUrl);
			}

			// 決済実行（カード入力）
			var result = SendParams(URL_EXECTRAN, parameters, "", gmoOrderId);
			return result;
		}

		/// <summary>
		/// 決済実行（3Dセキュア1.0利用）
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="paRes">本人認証サービス結果</param>
		/// <param name="md">取引ID</param>
		/// <returns>True:成功、False:失敗</returns>
		public bool SecureTran(
			string gmoOrderId,
			string paRes,
			string md)
		{
			// POST送信パラメタ作成
			var parameters = new NameValueCollection
			{
				{ PARAM_PA_RES, paRes },
				{ PARAM_MD, md }
			};
			// 決済実行（カード入力）
			var result = SendParams(URL_SECURETRAN, parameters, gmoOrderId);
			return result;
		}

		/// <summary>
		/// 決済実行（3Dセキュア2.0利用）
		/// </summary>
		/// <param name="cardTranId">決済取引ID</param>
		/// <returns>True:成功、False:失敗</returns>
		public bool SecureTran2(string cardTranId)
		{
			// POST送信パラメタ作成
			var accessId = cardTranId.Split(' ')[0];
			var accessPass = cardTranId.Split(' ')[1];
			var parameters = new NameValueCollection
			{
				{PARAM_ACCESSID, accessId},
				{PARAM_ACCESSPASS, accessPass},
			};
			// 決済実行（カード入力）
			var result = SendParams(URL_SECURETRAN2, parameters);
			return result;
		}

		/// <summary>
		/// 仮売上⇒実売上実行
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="priceTotal">合計金額</param>
		/// <returns>True:成功、False:失敗</returns>
		public bool Sales(
			string gmoOrderId,
			string cardTranId,
			decimal priceTotal)
		{
			return ExecAlterTran(gmoOrderId, cardTranId, priceTotal, KBN_JOB_CD_SALES);
		}

		/// <summary>
		/// 決済取り消し実行
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <returns>True:成功、False:失敗</returns>
		private bool Void(
			string gmoOrderId,
			string cardTranId)
		{
			return ExecAlterTran(gmoOrderId, cardTranId, 0, KBN_JOB_CD_VOID);
		}

		/// <summary>
		/// 返品実行
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <returns>True:成功、False:失敗</returns>
		private bool Return(
			string gmoOrderId,
			string cardTranId)
		{
			return ExecAlterTran(gmoOrderId, cardTranId, 0, KBN_JOB_CD_RETURN);
		}

		/// <summary>
		/// 月跨り返品実行
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <returns>True:成功、False:失敗</returns>
		private bool ReturnX(
			string gmoOrderId,
			string cardTranId)
		{
			return ExecAlterTran(gmoOrderId, cardTranId, 0, KBN_JOB_CD_RETURNX);
		}

		/// <summary>
		///  取引変更実行
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="priceTotal">合計金額</param>
		/// <param name="jobCd">処理区分</param>
		/// <returns>True:成功、False:失敗</returns>
		private bool ExecAlterTran(
			string gmoOrderId,
			string cardTranId,
			decimal priceTotal,
			string jobCd)
		{
			// POST送信パラメタ作成
			var accessId = cardTranId.Split(' ')[0];
			var accessPass = cardTranId.Split(' ')[1];
			var parameters = new NameValueCollection()
			{
				{PARAM_SHOPID, this.ShopId},
				{PARAM_SHOPPASS, this.ShopPass},
				{PARAM_ACCESSID, accessId},
				{PARAM_ACCESSPASS, accessPass},
				{PARAM_JOBCD, jobCd}
			};
			// 仮売上⇒実売上実行の場合は、利用金額をセット
			if (jobCd == KBN_JOB_CD_SALES)
			{
				parameters.Add(PARAM_AMOUNT, priceTotal.ToPriceString());
			}

			// 取引変更実行
			return SendParams(URL_EXECALTERTRAN, parameters, "", gmoOrderId);
		}

		/// <summary>
		/// 会員登録実行
		/// </summary>
		/// <param name="gmoMemberId">GMO会員ID</param>
		/// <param name="userName">ユーザ名</param>
		/// <returns>True:成功、False:失敗</returns>
		private bool SaveMember(string gmoMemberId, string userName)
		{
			// POST送信パラメタ作成
			var parameters = new NameValueCollection()
			{
				{PARAM_SITEID, this.SiteId},
				{PARAM_SITEPASS, this.SitePass},
				{PARAM_MEMBERID, gmoMemberId},
				{PARAM_MEMBERNAME, userName}
			};

			// 会員登録実行
			return SendParams(URL_SAVEMENBER, parameters, gmoMemberId);
		}

		/// <summary>
		/// 会員参照実行
		/// </summary>
		/// <param name="gmoMemberId">GMO会員ID</param>
		/// <returns>True:成功、False:失敗</returns>
		public bool SearchMember(string gmoMemberId)
		{
			// POST送信パラメタ作成
			var parameters = new NameValueCollection()
			{
				{PARAM_SITEID, this.SiteId},
				{PARAM_SITEPASS, this.SitePass},
				{PARAM_MEMBERID, gmoMemberId}
			};

			// 会員参照実行
			return SendParams(URL_SEARCHMEMBER, parameters, gmoMemberId);
		}

		/// <summary>
		/// カード登録・更新実行
		/// </summary>
		/// <param name="gmoMemberId">GMO会員ID</param>
		/// <param name="token">トークン文字列</param>
		/// <param name="authorName">名義人</param>
		/// <returns>True:成功、False:失敗</returns>
		private bool SaveCard(
			string gmoMemberId,
			string token,
			string authorName)
		{
			// POST送信パラメタ作成
			var parameters = new NameValueCollection()
			{
				{PARAM_SITEID, this.SiteId},
				{PARAM_SITEPASS, this.SitePass},
				{PARAM_MEMBERID, gmoMemberId},
				{PARAM_SEQMODE, KBN_SEQMODE_PHYSICAL},
				{PARAM_HOLDERNAME, authorName},
				{PARAM_TOKEN, token},
			};

			// カード登録・更新実行
			var result = SendParams(URL_SAVECARD, parameters, gmoMemberId);
			return result;
		}

		/// <summary>
		/// 決済後カード登録実行
		/// </summary>
		/// <param name="gmoMemberId">GMO会員ID</param>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="authorName">カード名義人</param>
		/// <returns>True:成功、False:失敗</returns>
		private bool TradedCard(
			string gmoMemberId,
			string gmoOrderId,
			string authorName)
		{
			var parameters = new NameValueCollection()
			{
				{PARAM_SHOPID, this.ShopId},
				{PARAM_SHOPPASS, this.ShopPass},
				{PARAM_ORDERID, gmoOrderId},
				{PARAM_SITEID, this.SiteId},
				{PARAM_SITEPASS, this.SitePass},
				{PARAM_MEMBERID, gmoMemberId},
				{PARAM_SEQMODE, KBN_SEQMODE_PHYSICAL},
				//{PARAM_DELETEFLAG, "0"},	// 継続課金対象としない
				{PARAM_HOLDERNAME, authorName}
			};
			var result = SendParams(URL_TRADEDCARD, parameters, gmoMemberId, gmoOrderId);
			return result;
		}

		/// <summary>
		/// カード参照実行
		/// </summary>
		/// <param name="gmoMemberId">GMO会員ID</param>
		/// <returns>True:成功、False:失敗</returns>
		public bool SearchCard(string gmoMemberId)
		{
			// POST送信パラメタ作成
			var parameters = new NameValueCollection()
			{
				{PARAM_SITEID, this.SiteId},
				{PARAM_SITEPASS, this.SitePass},
				{PARAM_MEMBERID, gmoMemberId},
				{PARAM_SEQMODE, KBN_SEQMODE_PHYSICAL},
				{PARAM_CARDSEQ, "0"}
			};

			// カード参照実行
			if (SendParams(URL_SEARCHCARD, parameters, gmoMemberId))
			{
				// カードが削除されている場合はfalseを返す
				if ((string)this.Result.Parameters[PARAM_DELETEFLAG] == KBN_DELETEFLAG_DEL)
				{
					return false;
				}

				// 成功
				return true;
			}
			else
			{
				// 失敗
				return false;
			}
		}

		/// <summary>
		/// 取引参照実行
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <returns>True:成功、False:失敗</returns>
		public bool SearchTrade(string gmoOrderId)
		{
			// POST送信パラメタ作成
			var parameters = new NameValueCollection()
			{
				{PARAM_SHOPID, this.ShopId},
				{PARAM_SHOPPASS, this.ShopPass},
				{PARAM_ORDERID, gmoOrderId}
			};

			// 取引参照実行
			return SendParams(URL_SEARCHTRADE, parameters, "", gmoOrderId);
		}

		/// <summary>
		/// 金額変更があったかどうか？
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="priceTotal">合計金額</param>
		/// <returns>True:成功、False:失敗</returns>
		public bool IsPriceChange(string gmoOrderId, string cardTranId, decimal priceTotal)
		{
			// 取引参照実行
			if (SearchTrade(gmoOrderId))
			{
				// 金額変更があった？
				if (this.Result.Parameters[PARAM_AMOUNT].ToPriceString() != priceTotal.ToPriceString())
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 金額変更実行
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="priceTotal">合計金額</param>
		/// <returns>True:成功、False:失敗</returns>
		public bool ChangeTran(string gmoOrderId, string cardTranId, decimal priceTotal)
		{
			// POST送信パラメタ作成
			var accessId = cardTranId.Split(' ')[0];
			var accessPass = cardTranId.Split(' ')[1];
			var parameters = new NameValueCollection()
			{
				{PARAM_SHOPID, this.ShopId},
				{PARAM_SHOPPASS, this.ShopPass},
				{PARAM_ACCESSID, accessId},
				{PARAM_ACCESSPASS, accessPass},
				{PARAM_JOBCD, this.JobCd},
			};
			// 有効性チェック
			if (this.JobCd != KBN_JOB_CD_CHECK)
			{
				parameters.Add(PARAM_AMOUNT, priceTotal.ToPriceString());
			}

			// 金額変更実行
			return SendParams(URL_CHANGETRAN, parameters, "", gmoOrderId);
		}

		/// <summary>
		/// 会員登録（※） ⇒ カード登録実行
		/// ※会員が存在しなければ登録
		/// </summary>
		/// <param name="gmoMemberId">GMO会員ID</param>
		/// <param name="userName">ユーザ名</param>
		/// <param name="token">トークン文字列</param>
		/// <param name="authorName">名義人</param>
		/// <returns>True:成功、False:失敗</returns>
		public bool SaveMemberAndCard(
			string gmoMemberId,
			string userName,
			string token,
			string authorName)
		{
			var result = true;
			if (SearchMember(gmoMemberId) == false)
			{
				// 会員登録実行
				result = SaveMember(gmoMemberId, userName);
			}
			if (result)
			{
				// カード登録実行
				result = SaveCard(gmoMemberId, token, authorName);
			}
			return result;
		}

		/// <summary>
		/// 会員登録（※） ⇒ 決済後カード登録実行
		/// ※会員が存在しなければ登録
		/// </summary>
		/// <param name="gmoMemberId">GMO会員ID</param>
		/// <param name="userName">ユーザ名</param>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="authorName">名義人</param>
		/// <returns>True:成功、False:失敗</returns>
		public bool SaveMemberAndTraedCard(
			string gmoMemberId,
			string userName,
			string gmoOrderId,
			string authorName)
		{
			var result = true;
			if (SearchMember(gmoMemberId) == false)
			{
				// 会員登録実行
				result = SaveMember(gmoMemberId, userName);
			}
			if (result)
			{
				// カード登録実行
				result = TradedCard(gmoMemberId, gmoOrderId, authorName);
			}
			return result;
		}

		/// <summary>
		/// パラメータをPOST送信
		/// </summary>
		/// <param name="url">送信URL</param>
		/// <param name="parameters">送信パラメータ</param>
		/// <param name="gmoMemberId">GMO会員ID</param>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <remarks>結果はResultプロパティに格納されます</remarks>
		private bool SendParams(
			string url,
			NameValueCollection parameters,
			string gmoMemberId = "",
			string gmoOrderId = "")
		{
			// POST送信用パラメタ文字列を連結
			var endoding = Encoding.GetEncoding("Shift_JIS");
			var postParameters = new List<string>();
			var postLog = new List<string>();
			foreach (string key in parameters)
			{
				postParameters.Add(string.Concat(key, "=", HttpUtility.UrlEncode(parameters[key], endoding)));
				if (IsWriteLog(key) == false) continue;
				postLog.Add(string.Concat(key, "=", parameters[key]));
			}

			// POST送信ログ出力
			var log = new StringBuilder();
			log.Append("\t").Append("[POST]");
			log.Append("\t").Append("url=").Append(url);
			log.Append("\t").Append("gmoMemberId=").Append(gmoMemberId);
			log.Append("\t").Append("gmoOrderId=").Append(gmoOrderId);
			log.Append("\t").Append("post=").Append(string.Join(":", postLog.ToArray()));

			// POST送信用パラメタをバイト列へ変換
			var data = Encoding.ASCII.GetBytes(string.Join("&", postParameters.ToArray()));

			// データをPOST送信
			var request = WebRequest.Create(this.ServerUrl + url);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = data.Length;
			using (var requestStream = request.GetRequestStream())
			{
				requestStream.Write(data, 0, data.Length);
			}

			PaymentFileLogger.WritePaymentLog(
				null,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.Gmo,
				PaymentFileLogger.PaymentProcessingType.Unknown,
				log.ToString(),
				new Dictionary<string, string>
				{
					{ "gmoOrderId", gmoOrderId },
					{ "gmoMemberId", gmoMemberId }
				});

			// レスポンス受信
			string responseString = null;
			using (var response = request.GetResponse())
			using (var responseStream = response.GetResponseStream())
			using (var streamReader = new StreamReader(responseStream, endoding))
			{
				responseString = streamReader.ReadToEnd();
			}

			// レスポンス文字列から結果取得
			this.Result = new ResponseResult(responseString);

			if ((url == URL_EXECTRAN) && Constants.PAYMENT_SETTING_GMO_3DSECURE)
			{
				// レスポンスデータが項目2個前提
				// RedirectUrlがURLエンコードされておらず、URLにパラメータもついている
				// 単純に「&」「=」で区切ると意図しない分割がされてしまうためSplitを「2」としている
				var responseSplit = responseString.Split(new[] { '&' }, 2)
					.Select(param => param.Split(new[] { '=' }, 2))
					.ToDictionary(param => param[0], param => param[1]);

				this.Acs = responseSplit.ContainsKey(PARAM_ACS) ? responseSplit[PARAM_ACS] : string.Empty;
				if (this.IsTds2)
				{
					this.RedirectUrl = responseSplit.ContainsKey(PARAM_REDIRECT_URL) ? responseSplit[PARAM_REDIRECT_URL] : string.Empty;
				}
				else if (this.IsTds1)
				{
					// 3Dセキュア1.0の場合は項目2個ではないため、もう一度分割する
					responseSplit = responseString.Split('&')
						.Select(param => param.Split('='))
						.ToDictionary(param => param[0], param => param[1]);

					this.AcsUrl = responseSplit.ContainsKey(PARAM_ACS_URL) ? responseSplit[PARAM_ACS_URL] : string.Empty;
					this.PaReq = responseSplit.ContainsKey(PARAM_PA_REQ) ? responseSplit[PARAM_PA_REQ] : string.Empty;
					this.Md = responseSplit.ContainsKey(PARAM_MD) ? responseSplit[PARAM_MD] : string.Empty;
				}
			}

			// ログ出力
			log = new StringBuilder();
			log.Append("\t[").Append(this.Result.IsSuccess ? "OK" : "NG").Append("]");
			var responseLog = new List<string>();
			foreach (string key in this.Result.Parameters)
			{
				if (IsWriteLog(key) == false) continue;
				responseLog.Add(string.Concat(key, "=", this.Result.Parameters[key]));
			}
			log.Append("\t").Append("result=").Append(string.Join(":", responseLog.ToArray()));

			// エラーの場合
			this.ErrorMessages = "";
			if (this.Result.IsSuccess == false)
			{
				log.Append("\t").Append("errorMessages=").Append(this.Result.ErrorMessages);
				this.ErrorMessages = this.Result.ErrorMessages;
				// エラー詳細コードを返却している(先頭が42)場合、エラー種別コードを取得
				this.ErrorTypeCode = (this.ErrorMessages.StartsWith("42")) ? this.ErrorMessages.Substring(2, 3) : string.Empty;
			}

			PaymentFileLogger.WritePaymentLog(
				this.Result.IsSuccess,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.Gmo,
				PaymentFileLogger.PaymentProcessingType.Unknown,
				log.ToString(),
				new Dictionary<string, string>
				{
					{ "gmoOrderId", gmoOrderId },
					{ "gmoMemberId", gmoMemberId }
				});

			return this.Result.IsSuccess;
		}

		/// <summary>
		/// 支払回数コンバート
		/// </summary>
		/// <param name="creditCardInstallments">カード支払回数</param>
		/// <returns>支払回数</returns>
		private KeyValuePair<string, string> ConvertCardInstallments(string creditCardInstallments)
		{
			switch (creditCardInstallments)
			{
				// 一括
				case "01":
					return new KeyValuePair<string, string>(KBN_PAYMENTMETHOD_SINGLE, "");

				// リボ
				case "99":
					return new KeyValuePair<string, string>(KBN_PAYMENTMETHOD_REVOLVING, "");

				// その他分割
				default:
					return new KeyValuePair<string, string>(KBN_PAYMENTMETHOD_INSTALLMENT, creditCardInstallments);
			}
		}

		/// <summary>
		/// ログ出力対象か？
		/// </summary>
		/// <param name="parameter">パラメータ</param>
		/// <returns>ログ出力対象か？（true:出力する、false:出力しない）</returns>
		private bool IsWriteLog(string parameter)
		{
			// 出力対象外のパラメータの場合
			// ※設定で持っている保持している値、カード番号、氏名
			switch (parameter)
			{
				case PARAM_SITEID:
				case PARAM_SITEPASS:
				case PARAM_SHOPID:
				case PARAM_SHOPPASS:
				case PARAM_ACCESSID:
				case PARAM_ACCESSPASS:
				case PARAM_CARDNO:
				case PARAM_EXPIRE:
				case PARAM_HOLDERNAME:
				case PARAM_SECURITYCODE:
				case PARAM_MEMBERNAME:
				case PARAM_CHECKSTRING:
				case PARAM_PA_RES:
					return false;
			}

			return true;
		}

		/// <summary>
		/// キャンセル処理(与信、売上確定のの日付に応じたキャンセルを行う)
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="cardTranId">決済カート取引ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns></returns>
		public bool Cancel(string paymentOrderId, string cardTranId, string orderId, SqlAccessor accessor = null)
		{
			var result = false;
			var today = DateTime.Today;
			var thisMonthFirstDay = new DateTime(today.Year, today.Month, 1);

			var order = new OrderService().Get(orderId, accessor);
			var authDate = order.ExternalPaymentAuthDate.HasValue
				? order.ExternalPaymentAuthDate.Value.Date
				: order.ExternalPaymentAuthDate;
			// 売上確定日がないため入金日で判定
			var paymentDate = order.OrderPaymentDate.HasValue
				? order.OrderPaymentDate.Value.Date
				: order.OrderPaymentDate;
			// 売上確定日が当日もしくは与信日が当日の場合、キャンセル処理
			if ((paymentDate == today) || (authDate == today))
			{
				result = Void(paymentOrderId, cardTranId);

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					result,
					Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentFileLogger.PaymentType.Gmo,
					PaymentFileLogger.PaymentProcessingType.Cancel,
					result ? "" : LogCreator.CreateErrorMessage("", this.ErrorMessages),
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId},
						{Constants.FIELD_ORDER_ORDER_ID, orderId},
						{Constants.FIELD_ORDER_CARD_TRAN_ID, cardTranId}
					});
				return result;
			}
			// 売上確定日が先月以前の場合、月跨り返品処理
			else if (paymentDate < thisMonthFirstDay)
			{
				result = ReturnX(paymentOrderId, cardTranId);
				return result;
			}
			// 売上確定日が昨日以前もしくは与信日が昨日以前の場合、返品処理
			else if ((paymentDate < today) || (authDate < today))
			{
				result = Return(paymentOrderId, cardTranId);

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					result,
					Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentFileLogger.PaymentType.Gmo,
					PaymentFileLogger.PaymentProcessingType.Return,
					result ? "" : LogCreator.CreateErrorMessage("", this.ErrorMessages),
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId},
						{Constants.FIELD_ORDER_ORDER_ID, orderId},
						{Constants.FIELD_ORDER_CARD_TRAN_ID, cardTranId}
					});
				return result;
			}
			// どれにも当てはまらない場合(与信日が異常値(nullもしくは未来日))、処理をスキップ
			else
			{
				this.ErrorMessages = "外部決済与信日が不正です。";

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentFileLogger.PaymentType.Gmo,
					PaymentFileLogger.PaymentProcessingType.Cancel,
					LogCreator.CreateErrorMessage("", this.ErrorMessages),
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId},
						{Constants.FIELD_ORDER_ORDER_ID, orderId},
						{Constants.FIELD_ORDER_CARD_TRAN_ID, cardTranId}
					});
				return false;
			}
		}
	}
}
