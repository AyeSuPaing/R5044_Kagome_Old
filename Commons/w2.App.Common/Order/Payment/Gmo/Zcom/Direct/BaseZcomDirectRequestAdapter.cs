/*
=========================================================================================================
  Module      : Zcom決済基底アダプタ (BaseZcomDirectRequestAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.GMO.Zcom.Direct
{
	/// <summary>
	/// Zcom決済基底アダプタ
	/// </summary>
	public abstract class BaseZcomDirectRequestAdapter
	{
		/// <summary>ダミーのメールアドレス</summary>
		public const string DUMMY_MAIL_ADDR = "dummynothing@example.com";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected BaseZcomDirectRequestAdapter()
		{
			this.PaymentOrderId = "";
			this.PaymentUserId = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiSetting">Api設定</param>
		protected BaseZcomDirectRequestAdapter(ZcomApiSetting apiSetting)
			: base()
		{
			this.ApiSetting = apiSetting;
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <returns>レスポンスデータ</returns>
		public ZcomDirectResponse Execute()
		{
			var request = CreateRequest();
			return Execute(request);
		}
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="request">リクエストデータ</param>
		/// <returns>レスポンスデータ</returns>
		public ZcomDirectResponse Execute(ZcomDirectRequest request)
		{
			var factory = ExternalApiFacade.Instance.ZcomApiFacadeFactory;
			var facade = factory.CreateFacade(this.ApiSetting);
			var res = facade.DirectPayment(request);
			return res;
		}

		/// <summary>
		/// リクエストデータ生成
		/// </summary>
		/// <returns>リクエストデータ</returns>
		public virtual ZcomDirectRequest CreateRequest()
		{
			var request = new ZcomDirectRequest();

			request.ContractCode = this.GetConstractCode();
			request.Version = this.GetVersion();
			request.CharacterCode = this.GetCharacterCode();
			request.ProcessCode = this.GetProcessCode();
			request.UserId = this.GetUserId();
			request.UserName = this.GetUserName();

			// メルアドは空だとエラーにされるのでダミーのもの入れとく
			var adr = this.GetUserMailAdd();
			request.UserMailAdd = string.IsNullOrEmpty(adr) ? DUMMY_MAIL_ADDR : adr;

			request.LangId = this.GetLangId();
			request.IpAddress = this.GetIpAddress();
			request.UserAgent = this.GetUserAgent();
			request.ItemCode = this.GetItemCode();
			request.ItemName = this.GetItemName();
			request.OrderNumber = this.GetOrderNumber();
			request.StCode = this.GetStCode();
			request.MissionCode = this.GetMissionCode();
			request.CurrencyId = this.GetCurrencyId();

			// 通貨フォーマット整形
			request.ItemPrice = this.GetCurrencyFormatedString(this.GetItemPrice());

			request.CardNumber = this.GetCardNumber();
			request.ExpireYear = this.GetExpireYear();
			request.ExpireMonth = this.GetExpireMonth();
			request.SecurityCode = this.GetSecurityCode();
			request.PanBank = this.GetPanBank();
			request.PanCountry = this.GetPanCountry();
			request.CardHolderName = this.GetCardHolderName();
			request.PaymentMethod = this.GetPaymentMethod();
			request.BackUrl = this.GetBackUrl();
			request.ErrUrl = this.GetErrUrl();
			request.SuccessUrl = this.GetSuccessUrl();
			request.Memo1 = this.GetMemo1();
			request.Memo2 = this.GetMemo2();
			request.AddInfo1 = this.GetAddInfo1();
			request.AddInfo2 = this.GetAddInfo2();
			request.AddInfo3 = this.GetAddInfo3();
			request.AddInfo4 = this.GetAddInfo4();
			request.AddInfo5 = this.GetAddInfo5();

			return request;
		}

		/// <summary>
		/// 通貨整形
		/// </summary>
		/// <param name="val">対象文字</param>
		/// <returns>整形した文字</returns>
		private string GetCurrencyFormatedString(decimal val)
		{
			// 設定を基にフォーマット整形
			return string.Format(Constants.PAYMENT_SETTING_CREDIT_ZCOM_CURRENCYFORMAT, val);
		}

		/// <summary>
		/// 契約コード取得
		/// </summary>
		/// <returns>契約コード</returns>
		protected virtual string GetConstractCode()
		{
			return Constants.PAYMENT_CREDIT_ZCOM_APICONTACTCODE;
		}

		/// <summary>
		/// バージョン取得
		/// </summary>
		/// <returns>バージョン</returns>
		protected virtual string GetVersion()
		{
			// 固定で1
			return "1";
		}

		/// <summary>
		/// 文字コード取得
		/// </summary>
		/// <returns>文字コード</returns>
		protected virtual string GetCharacterCode()
		{
			// 固定でUTF8
			return "UTF-8";
		}

		/// <summary>
		/// 処理コード取得
		/// </summary>
		/// <returns>処理コード</returns>
		protected virtual string GetProcessCode()
		{
			// 常に2
			return "2";
		}

		/// <summary>
		/// ユーザーID取得
		/// </summary>
		/// <returns>ユーザーID</returns>
		protected abstract string GetUserId();

		/// <summary>
		/// ユーザー名取得
		/// </summary>
		/// <returns>ユーザー名</returns>
		protected abstract string GetUserName();

		/// <summary>
		/// メールアドレス取得
		/// </summary>
		/// <returns>メールアドレス</returns>
		protected abstract string GetUserMailAdd();

		/// <summary>
		/// 利用言語取得
		/// </summary>
		/// <returns>利用言語</returns>
		protected virtual string GetLangId()
		{
			return Constants.PAYMENT_CREDIT_ZCOM_APILANGID;
		}

		/// <summary>
		/// IPアドレス取得
		/// </summary>
		/// <returns>IPアドレス</returns>
		protected virtual string GetIpAddress()
		{
			// 空白渡せないのでダミー値
			return "127.0.0.1";
		}

		/// <summary>
		/// ユーザーエージェント取得
		/// </summary>
		/// <returns>ユーザーエージェント</returns>
		protected virtual string GetUserAgent()
		{
			// 空白渡せないのでダミー値
			return "nothing";
		}

		/// <summary>
		/// 商品コード取得
		/// </summary>
		/// <returns>商品コード</returns>
		protected abstract string GetItemCode();

		/// <summary>
		/// 商品名取得
		/// </summary>
		/// <returns>商品名</returns>
		protected abstract string GetItemName();

		/// <summary>
		/// オーダー番号取得
		/// </summary>
		/// <returns>オーダー番号</returns>
		protected abstract string GetOrderNumber();

		/// <summary>
		/// 決済区分取得
		/// </summary>
		/// <returns>決済区分</returns>
		protected virtual string GetStCode()
		{
			switch (Constants.PAYMENT_SETTING_CREDIT_ZCOM_STCODE)
			{
				case Constants.ZcomStCode.NW:
					return "10000-00000-00000-00000-00000-00000-00000";

				case Constants.ZcomStCode.NNW:
					return "00000-00000-10000-00000-00000-00000-00000";

				default:
					return "";
			}
		}

		/// <summary>
		/// 課金区分取得
		/// </summary>
		/// <returns>課金区分</returns>
		protected virtual string GetMissionCode()
		{
			return Constants.PAYMENT_CREDIT_ZCOM_APIMISSIONCODE;
		}

		/// <summary>
		/// 通貨コード取得
		/// </summary>
		/// <returns>通貨コード</returns>
		protected virtual string GetCurrencyId()
		{
			return Constants.PAYMENT_SETTING_CREDIT_ZCOM_CURRENCYCODE;
		}

		/// <summary>
		/// 価格取得
		/// </summary>
		/// <returns>価格</returns>
		protected abstract decimal GetItemPrice();

		/// <summary>
		/// クレジットカード番号取得
		/// </summary>
		/// <returns>クレジットカード番号</returns>
		protected abstract string GetCardNumber();

		/// <summary>
		/// 有効期限年取得
		/// </summary>
		/// <returns>有効期限年</returns>
		protected abstract string GetExpireYear();

		/// <summary>
		/// 有効期限月取得
		/// </summary>
		/// <returns>有効期限月</returns>
		protected abstract string GetExpireMonth();

		/// <summary>
		/// セキュリティコード取得
		/// </summary>
		/// <returns>セキュリティコード</returns>
		protected abstract string GetSecurityCode();

		/// <summary>
		/// 利用カード会社名取得
		/// </summary>
		/// <returns>利用カード会社名</returns>
		protected virtual string GetPanBank()
		{
			// 現在未使用のため空
			return "";
		}

		/// <summary>
		/// 発行カード会社国取得
		/// </summary>
		/// <returns>発行カード会社国</returns>
		protected virtual string GetPanCountry()
		{
			// 未使用のためTW固定
			return "TW";
		}

		/// <summary>
		/// カードホルダー名取得
		/// </summary>
		/// <returns>カードホルダー名</returns>
		protected abstract string GetCardHolderName();

		/// <summary>
		/// 支払い方法取得
		/// </summary>
		/// <returns>支払い方法</returns>
		protected virtual string GetPaymentMethod()
		{
			// 未使用のため空
			return "";
		}

		/// <summary>
		/// 戻りURL取得
		/// </summary>
		/// <returns>戻りURL</returns>
		protected virtual string GetBackUrl()
		{
			// ここでは指定しない
			return "";
		}

		/// <summary>
		/// エラー時URL取得
		/// </summary>
		/// <returns>エラー時URL</returns>
		protected virtual string GetErrUrl()
		{
			// ここでは指定しない
			return "";
		}

		/// <summary>
		/// 決済完了時URL取得
		/// </summary>
		/// <returns>決済完了時URL</returns>
		protected virtual string GetSuccessUrl()
		{
			// ここでは指定しない
			return "";
		}

		/// <summary>
		/// メモ1取得
		/// </summary>
		/// <returns>メモ1</returns>
		protected abstract string GetMemo1();

		/// <summary>
		/// メモ2取得
		/// </summary>
		/// <returns>メモ2</returns>
		protected abstract string GetMemo2();

		/// <summary>
		/// 追加1（仮売り・即時判断フラグ）取得
		/// </summary>
		/// <returns>追加1（仮売り・即時判断フラグ）</returns>
		protected virtual string GetAddInfo1()
		{
			// 仮売りか即売上かは設定から
			return Constants.PAYMENT_CREDIT_ZCOM_APIADDINFO1;
		}

		/// <summary>
		/// 追加2取得
		/// </summary>
		/// <returns>追加2</returns>
		protected virtual string GetAddInfo2()
		{
			// 未使用のため空
			return "";
		}

		/// <summary>
		/// 追加3取得
		/// </summary>
		/// <returns>追加3</returns>
		protected virtual string GetAddInfo3()
		{
			// 未使用のため空
			return "";
		}

		/// <summary>
		/// 追加4取得
		/// </summary>
		/// <returns>追加4</returns>
		protected virtual string GetAddInfo4()
		{
			// 一括しか対応しないので空
			return "";
		}

		/// <summary>
		/// 追加5取得
		/// </summary>
		/// <returns>追加5</returns>
		protected virtual string GetAddInfo5()
		{
			// 未使用のため空
			return "";
		}

		/// <summary>決済注文ID</summary>
		public string PaymentOrderId { get; set; }
		/// <summary>ユーザーID</summary>
		public string PaymentUserId { get; set; }
		/// <summary>/// API設定</summary>
		public ZcomApiSetting ApiSetting { get; set; }
	}
}
