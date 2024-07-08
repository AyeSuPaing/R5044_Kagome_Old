/*
=========================================================================================================
  Module      : GMOクレジットカード登録レスポンスクラス(PaymentGmoCreditCardRegisterResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace w2.App.Common.Order.Payment.GMO.Receiver
{
	/// <summary>
	/// GMOクレジットカード登録レスポンスクラス
	/// </summary>
	public class PaymentGmoCreditCardRegisterResponse
	{
		/// <summary>処理区分：登録</summary>
		public const string PROCESS_TYPE_INSERT = "I";
		/// <summary>処理区分：更新</summary>
		public const string PROCESS_TYPE_UPDATE = "U";
		/// <summary>処理区分：削除</summary>
		public const string PROCESS_TYPE_DELETE = "D";
		/// <summary>処理区分：決済後カード登録</summary>
		public const string PROCESS_TYPE_TRANSACTION = "T";
		/// <summary>決済方法：クレジット</summary>
		public const string PAY_TYPE_CREDIT = "0";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="context">コンテキスト</param>
		public PaymentGmoCreditCardRegisterResponse(HttpContext context)
		{
			this.Post = context.Request.Form;
		}

		/// <summary>
		/// 登録（更新）可能か
		/// </summary>
		/// <returns>true:可能 false:不可</returns>
		public bool IsRegistable()
		{
			var result = this.CardSeq == "0";
			return result;
		}

		/// <summary>サイトID</summary>
		public string SiteId { get { return this.Post["SiteID"]; } }
		/// <summary>会員ID</summary>
		public string MemberId { get { return this.Post["MemberID"]; } }
		/// <summary>登録連番(物理)</summary>
		public string CardSeq { get { return this.Post["CardSeq"]; } }
		/// <summary>登録連番(論理)</summary>
		public string CardSeqLogical { get { return this.Post["CardSeqLogical"]; } }
		/// <summary>処理日時</summary>
		public string ProcessDate { get { return this.Post["ProcessDate"]; } }
		/// <summary>処理区分</summary>
		public string ProcessType { get { return this.Post["ProcessType"]; } }
		/// <summary>カード番号(マスク済)</summary>
		public string CardNo { get { return this.Post["CardNo"]; } }
		/// <summary>有効期限(マスク済)</summary>
		public string Expire { get { return this.Post["Expire"]; } }
		/// <summary>仕向先会社コード</summary>
		public string Forward { get { return this.Post["Forward"]; } }
		/// <summary>デフォルトフラグ</summary>
		public string DefaultFlag { get { return this.Post["DefaultFlag"]; } }
		/// <summary>決済方法</summary>
		public string PayType { get { return this.Post["PayType"]; } }
		/// <summary>POST</summary>
		public NameValueCollection Post { get; private set; }
	}
}
