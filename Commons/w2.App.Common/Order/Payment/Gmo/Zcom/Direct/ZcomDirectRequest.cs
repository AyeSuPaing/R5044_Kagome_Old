/*
=========================================================================================================
  Module      : Zcom決済リクエストデータ (ZcomDirectRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.GMO.Zcom.Direct
{
	/// <summary>
	/// Zcom決済リクエストデータ
	/// </summary>
	public class ZcomDirectRequest : BaseZcomRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ZcomDirectRequest()
			: base()
		{
			base.m_data.Add("contract_code", "");
			base.m_data.Add("version", "");
			base.m_data.Add("character_code", "");
			base.m_data.Add("process_code", "");
			base.m_data.Add("user_id", "");
			base.m_data.Add("user_name", "");
			base.m_data.Add("user_mail_add", "");
			base.m_data.Add("lang_id", "");
			base.m_data.Add("ip_address", "");
			base.m_data.Add("user_agent", "");
			base.m_data.Add("item_code", "");
			base.m_data.Add("item_name", "");
			base.m_data.Add("order_number", "");
			base.m_data.Add("st_code", "");
			base.m_data.Add("mission_code", "");
			base.m_data.Add("currency_id", "");
			base.m_data.Add("item_price", "");
			base.m_data.Add("card_number", "");
			base.m_data.Add("expire_year", "");
			base.m_data.Add("expire_month", "");
			base.m_data.Add("security_code", "");
			base.m_data.Add("pan_bank", "");
			base.m_data.Add("pan_country", "");
			base.m_data.Add("card_holder_name", "");
			base.m_data.Add("payment_method", "");
			base.m_data.Add("back_url", "");
			base.m_data.Add("err_url", "");
			base.m_data.Add("success_url", "");
			base.m_data.Add("memo1", "");
			base.m_data.Add("memo2", "");
			base.m_data.Add("add_info1", "");
			base.m_data.Add("add_info2", "");
			base.m_data.Add("add_info3", "");
			base.m_data.Add("add_info4", "");
			base.m_data.Add("add_info5", "");
		}

		/// <summary>契約コード</summary>
		public string ContractCode
		{
			get { return base.m_data["contract_code"]; }
			set { base.m_data["contract_code"] = value; }
		}
		/// <summary>バージョン</summary>
		public string Version
		{
			get { return base.m_data["version"]; }
			set { base.m_data["version"] = value; }
		}
		/// <summary>文字コード</summary>
		public string CharacterCode
		{
			get { return base.m_data["character_code"]; }
			set { base.m_data["character_code"] = value; }
		}
		/// <summary>処理区分</summary>
		public string ProcessCode
		{
			get { return base.m_data["process_code"]; }
			set { base.m_data["process_code"] = value; }
		}
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return base.m_data["user_id"]; }
			set { base.m_data["user_id"] = value; }
		}
		/// <summary>ユーザ氏名</summary>
		public string UserName
		{
			get { return base.m_data["user_name"]; }
			set { base.m_data["user_name"] = value; }
		}
		/// <summary>メールアドレス</summary>
		public string UserMailAdd
		{
			get { return base.m_data["user_mail_add"]; }
			set { base.m_data["user_mail_add"] = value; }
		}
		/// <summary>利用言語</summary>
		public string LangId
		{
			get { return base.m_data["lang_id"]; }
			set { base.m_data["lang_id"] = value; }
		}
		/// <summary>IPアドレス</summary>
		public string IpAddress
		{
			get { return base.m_data["ip_address"]; }
			set { base.m_data["ip_address"] = value; }
		}
		/// <summary>ユーザーエージェント</summary>
		public string UserAgent
		{
			get { return base.m_data["user_agent"]; }
			set { base.m_data["user_agent"] = value; }
		}
		/// <summary>商品コード（複数ある場合は代表で一つ）</summary>
		public string ItemCode
		{
			get { return base.m_data["item_code"]; }
			set { base.m_data["item_code"] = value; }
		}
		/// <summary>商品名（複数ある場合は代表で一つ）64Byte切り捨て</summary>
		public string ItemName
		{
			get { return base.m_data["item_name"]; }
			set { base.m_data["item_name"] = value; }
		}
		/// <summary>オーダー番号（決済注文ID）</summary>
		public string OrderNumber
		{
			get { return base.m_data["order_number"]; }
			set { base.m_data["order_number"] = value; }
		}
		/// <summary>決済区分</summary>
		public string StCode
		{
			get { return base.m_data["st_code"]; }
			set { base.m_data["st_code"] = value; }
		}
		/// <summary>課金区分</summary>
		public string MissionCode
		{
			get { return base.m_data["mission_code"]; }
			set { base.m_data["mission_code"] = value; }
		}
		/// <summary>通貨コード</summary>
		public string CurrencyId
		{
			get { return base.m_data["currency_id"]; }
			set { base.m_data["currency_id"] = value; }
		}
		/// <summary>価格</summary>
		public string ItemPrice
		{
			get { return base.m_data["item_price"]; }
			set { base.m_data["item_price"] = value; }
		}
		/// <summary>クレジットカード番号</summary>
		public string CardNumber
		{
			get { return base.m_data["card_number"]; }
			set { base.m_data["card_number"] = value; }
		}
		/// <summary>有効期限：年（4桁）</summary>
		public string ExpireYear
		{
			get { return base.m_data["expire_year"]; }
			set { base.m_data["expire_year"] = value; }
		}
		/// <summary>有効期限：月（1～2桁 0埋めなし）</summary>
		public string ExpireMonth
		{
			get { return base.m_data["expire_month"]; }
			set { base.m_data["expire_month"] = value; }
		}
		/// <summary>セキュリティコード</summary>
		public string SecurityCode
		{
			get { return base.m_data["security_code"]; }
			set { base.m_data["security_code"] = value; }
		}
		/// <summary>利用カード会社名</summary>
		public string PanBank
		{
			get { return base.m_data["pan_bank"]; }
			set { base.m_data["pan_bank"] = value; }
		}
		/// <summary>利用カード会社国</summary>
		public string PanCountry
		{
			get { return base.m_data["pan_country"]; }
			set { base.m_data["pan_country"] = value; }
		}
		/// <summary>カードホルダー名</summary>
		public string CardHolderName
		{
			get { return base.m_data["card_holder_name"]; }
			set { base.m_data["card_holder_name"] = value; }
		}
		/// <summary>支払い方法</summary>
		public string PaymentMethod
		{
			get { return base.m_data["payment_method"]; }
			set { base.m_data["payment_method"] = value; }
		}
		/// <summary>戻りURL</summary>
		public string BackUrl
		{
			get { return base.m_data["back_url"]; }
			set { base.m_data["back_url"] = value; }
		}
		/// <summary>エラー時URL</summary>
		public string ErrUrl
		{
			get { return base.m_data["err_url"]; }
			set { base.m_data["err_url"] = value; }
		}
		/// <summary>決済完了時URL</summary>
		public string SuccessUrl
		{
			get { return base.m_data["success_url"]; }
			set { base.m_data["success_url"] = value; }
		}
		/// <summary>メモ1</summary>
		public string Memo1
		{
			get { return base.m_data["memo1"]; }
			set { base.m_data["memo1"] = value; }
		}
		/// <summary>メモ2</summary>
		public string Memo2
		{
			get { return base.m_data["memo2"]; }
			set { base.m_data["memo2"] = value; }
		}
		/// <summary>仮売り即時判断フラグ</summary>
		public string AddInfo1
		{
			get { return base.m_data["add_info1"]; }
			set { base.m_data["add_info1"] = value; }
		}
		/// <summary>追加2</summary>
		public string AddInfo2
		{
			get { return base.m_data["add_info2"]; }
			set { base.m_data["add_info2"] = value; }
		}
		/// <summary>追加3</summary>
		public string AddInfo3
		{
			get { return base.m_data["add_info3"]; }
			set { base.m_data["add_info3"] = value; }
		}
		/// <summary>分割払い</summary>
		public string AddInfo4
		{
			get { return base.m_data["add_info4"]; }
			set { base.m_data["add_info4"] = value; }
		}
		/// <summary>追加5</summary>
		public string AddInfo5
		{
			get { return base.m_data["add_info5"]; }
			set { base.m_data["add_info5"] = value; }
		}
	}
}
