/*
=========================================================================================================
  Module      : メッセージマネージャー(MessageManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;

namespace w2.Common.Util
{
	///**************************************************************************************
	/// <summary>
	/// 画面に出力する基本メッセージを管理する
	/// </summary>
	/// <remarks>
	/// GetMessagesでエラーメッセージ取得。
	/// GetMessagesを呼び出すごとにXMLファイルの更新日付をチェックし、
	/// 変更があれば再読み込みを行う。
	/// </remarks>
	///**************************************************************************************
	public class MessageManager
	{
		//=========================================================================================
		// 共通系
		//=========================================================================================
		//------------------------------------------------------
		// システムエラー
		//------------------------------------------------------
		/// <summary>エラーメッセージ定数：システムエラー</summary>
		public const string ERRMSG_SYSTEM_ERROR = "ERRMSG_SYSTEM_ERROR";
		/// <summary>エラーメッセージ定数：404エラー</summary>
		public const string ERRMSG_404_ERROR = "ERRMSG_404_ERROR";

		//------------------------------------------------------
		// システム入力エラー(不正文字入力など) 
		//------------------------------------------------------
		/// <summary>エラーメッセージ定数：不正文字入力エラー</summary>
		public const string ERRMSG_SYSTEM_VALIDATION_ERROR = "ERRMSG_SYSTEM_VALIDATION_ERROR";
		/// <summary>外部リンクURL形式エラー/// </summary>
		public const string ERRMSG_MANAGER_URL_FORMAT_ERROR = "ERRMSG_MANAGER_URL_FORMAT_ERROR";
		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		/// <summary>エラーメッセージ定数：必須チェックエラー</summary>
		public const string INPUTCHECK_NECESSARY = "INPUTCHECK_NECESSARY";
		/// <summary>エラーメッセージ定数：文字数チェックエラー</summary>
		public const string INPUTCHECK_LENGTH = "INPUTCHECK_LENGTH";
		/// <summary>エラーメッセージ定数：最大文字数チェックエラー</summary>
		public const string INPUTCHECK_LENGTH_MAX = "INPUTCHECK_LENGTH_MAX";
		/// <summary>エラーメッセージ定数：最小文字数チェックエラー</summary>
		public const string INPUTCHECK_LENGTH_MIN = "INPUTCHECK_LENGTH_MIN";
		/// <summary>エラーメッセージ定数：バイト数チェックエラー</summary>
		public const string INPUTCHECK_BYTE_LENGTH = "INPUTCHECK_BYTE_LENGTH";
		/// <summary>エラーメッセージ定数：最大バイト数チェックエラー</summary>
		public const string INPUTCHECK_BYTE_LENGTH_MAX = "INPUTCHECK_BYTE_LENGTH_MAX";
		/// <summary>エラーメッセージ定数：最小バイト数チェックエラー</summary>
		public const string INPUTCHECK_BYTE_LENGTH_MIN = "INPUTCHECK_BYTE_LENGTH_MIN";
		/// <summary>エラーメッセージ定数：最大数値チェックエラー</summary>
		public const string INPUTCHECK_NUMBER_MAX = "INPUTCHECK_NUMBER_MAX";
		/// <summary>エラーメッセージ定数：最小数値チェックエラー</summary>
		public const string INPUTCHECK_NUMBER_MIN = "INPUTCHECK_NUMBER_MIN";
		/// <summary>エラーメッセージ定数：全角チェックエラー</summary>
		public const string INPUTCHECK_FULLWIDTH = "INPUTCHECK_FULLWIDTH";
		/// <summary>エラーメッセージ定数：全角ひらがなチェックエラー</summary>
		public const string INPUTCHECK_FULLWIDTH_HIRAGANA = "INPUTCHECK_FULLWIDTH_HIRAGANA";
		/// <summary>エラーメッセージ定数：全角カタカナチェックエラー</summary>
		public const string INPUTCHECK_FULLWIDTH_KATAKANA = "INPUTCHECK_FULLWIDTH_KATAKANA";
		/// <summary>エラーメッセージ定数：半角チェックエラー</summary>
		public const string INPUTCHECK_HALFWIDTH = "INPUTCHECK_HALFWIDTH";
		/// <summary>エラーメッセージ定数：半角英数チェックエラー</summary>
		public const string INPUTCHECK_HALFWIDTH_ALPHNUM = "INPUTCHECK_HALFWIDTH_ALPHNUM";
		/// <summary>エラーメッセージ定数：半角英数記号チェックエラー</summary>
		public const string INPUTCHECK_HALFWIDTH_ALPHNUMSYMBOL = "INPUTCHECK_HALFWIDTH_ALPHNUMSYMBOL";
		/// <summary>エラーメッセージ定数：半角数値チェックエラー</summary>
		public const string INPUTCHECK_HALFWIDTH_NUMBER = "INPUTCHECK_HALFWIDTH_NUMBER";
		/// <summary>エラーメッセージ定数：半角数字チェックエラー</summary>
		public const string INPUTCHECK_HALFWIDTH_DECIMAL = "INPUTCHECK_HALFWIDTH_DECIMAL";
		/// <summary>エラーメッセージ定数：半角日付チェックエラー</summary>
		public const string INPUTCHECK_HALFWIDTH_DATE = "INPUTCHECK_HALFWIDTH_DATE";
		/// <summary>エラーメッセージ定数：半角数値チェックエラー</summary>
		public const string INPUTCHECK_HALFWIDTH_NUMERIC = "INPUTCHECK_HALFWIDTH_NUMERIC";
		/// <summary>エラーメッセージ定数：日付チェックエラー</summary>
		public const string INPUTCHECK_DATE = "INPUTCHECK_DATE";
		/// <summary>エラーメッセージ定数：未来日付チェックエラー</summary>
		public const string INPUTCHECK_DATE_FUTURE = "INPUTCHECK_DATE_FUTURE";
		/// <summary>エラーメッセージ定数：過去日付チェックエラー</summary>
		public const string INPUTCHECK_DATE_PAST = "INPUTCHECK_DATE_PAST";
		/// <summary>エラーメッセージ定数：メールアドレスチェックエラー</summary>
		public const string INPUTCHECK_MAILADDRESS = "INPUTCHECK_MAILADDRESS";
		/// <summary>エラーメッセージ定数：都道府県チェックエラー</summary>
		public const string INPUTCHECK_PREFECTURE = "INPUTCHECK_PREFECTURE";
		/// <summary>エラーメッセージ定数：商品税率カテゴリチェックエラー</summary>
		public const string INPUTCHECK_TAX_RATE = "INPUTCHECK_TAX_RATE";
		/// <summary>エラーメッセージ定数：禁止文字チェックエラー</summary>
		public const string INPUTCHECK_PROHIBITED_CHAR = "INPUTCHECK_PROHIBITED_CHAR";
		/// <summary>エラーメッセージ定数：文字コード外チェックエラー</summary>
		public const string INPUTCHECK_OUTOFCHARCODE = "INPUTCHECK_OUTOFCHARCODE";
		/// <summary>エラーメッセージ定数：正規表現チェックエラー</summary>
		public const string INPUTCHECK_REGEXP = "INPUTCHECK_REGEXP";
		/// <summary>エラーメッセージ定数：正規表現2チェックエラー</summary>
		public const string INPUTCHECK_REGEXP2 = "INPUTCHECK_REGEXP2";
		/// <summary>エラーメッセージ定数：正規表現（除外）チェックエラー</summary>
		public const string INPUTCHECK_EXCEPT_REGEXP = "INPUTCHECK_EXCEPT_REGEXP";
		/// <summary>エラーメッセージ定数：確認入力チェックエラー</summary>
		public const string INPUTCHECK_CONFIRM = "INPUTCHECK_CONFIRM";
		/// <summary>エラーメッセージ定数：同値チェックエラー</summary>
		public const string INPUTCHECK_EQUIVALENCE = "INPUTCHECK_EQUIVALENCE";
		/// <summary>エラーメッセージ定数：異値チェックエラー</summary>
		public const string INPUTCHECK_DIFFERENT_VALUE = "INPUTCHECK_DIFFERENT_VALUE";
		/// <summary>エラーメッセージ定数：重複チェックエラー</summary>
		public const string INPUTCHECK_DUPLICATION = "INPUTCHECK_DUPLICATION";
		/// <summary>エラーメッセージ定数：重複チェックエラー</summary>
		public const string INPUTCHECK_DUPLICATION_DATERANGE = "INPUTCHECK_DUPLICATION_DATERANGE";
		/// <summary>エラーメッセージ定数：開始日付は終了日付より小さいチェックエラー</summary>
		public const string INPUTCHECK_DATERANGE = "INPUTCHECK_DATERANGE";
		/// <summary>エラーメッセージ定数：通貨チェックエラー</summary>
		public const string INPUTCHECK_CURRENCY = "INPUTCHECK_CURRENCY";
		/// <summary>エラーメッセージ定数：メール送信禁止文字列チェックエラー</summary>
		public const string INPUTCHECK_MAIL_TRANSMISSION_DISABLED_STRING = "INPUTCHECK_MAIL_TRANSMISSION_DISABLED_STRING";
		/// <summary>エラーメッセージ定数：時間チェックエラー</summary>
		public const string INPUTCHECK_DROPDOWN_TIME = "INPUTCHECK_DROPDOWN_TIME";
		/// <summary>エラーメッセージ定数：上限金額チェックエラー</summary>
		public const string INPUTCHECK_PRICE_MAX = "INPUTCHECK_PRICE_MAX";
		/// <summary>オプション価格のエラーメッセージ定数：半角数値チェックエラー</summary>
		public const string INPUTCHECK_HALFWIDTH_NUMBER_OPTION_PRICE = "INPUTCHECK_HALFWIDTH_NUMBER_OPTION_PRICE";
		/// <summary>エラーメッセージ定数：使用禁止文字エラー</summary>
		public const string INPUTCHECK_PROHIBITED_CHARACTERS = "INPUTCHECK_PROHIBITED_CHARACTERS";

		/// <summary>エラーファイル最終更新日</summary>
		private static DateTime m_dtFileLastUpdate = new DateTime(0);

		/// <summary>エラーメッセージ格納ディクショナリ</summary>
		private static Dictionary<string, string> m_dicMessages = new Dictionary<string, string>();

		/// <summary>ReaderWriterLockSlimオブジェクト</summary>
		private static System.Threading.ReaderWriterLockSlim m_lock = new System.Threading.ReaderWriterLockSlim();

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		/// <param name="replaces">置換パラメータ</param>
		/// <returns>エラーメッセージ</returns>
		public static string GetMessages(string messageKey, params string[] replaces)
		{
			var message = MessageProvider.GetMessages(messageKey, replaces);
			if (string.IsNullOrEmpty(message)) message = string.Empty;
			return message;
		}

		/// <summary>
		/// メッセージプロバイダ
		/// </summary>
		public static IMessageProvider MessageProvider
		{
			get { return MessageManager.m_messageProvider; }
			set { MessageManager.m_messageProvider = value; }
		}
		/// <summary>メッセージプロバイダ</summary>
		private static IMessageProvider m_messageProvider = new MessageProviderXml();
	}
}
