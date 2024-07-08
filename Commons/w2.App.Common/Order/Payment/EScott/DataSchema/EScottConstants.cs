/*
=========================================================================================================
  Module      : e-SCOTT データパラメータ名クラス(EScottConstants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order.Payment.EScott.DataSchema
{
	/// <summary>
	/// データパラメータ名クラス
	/// </summary>
	internal static class EScottConstants
	{
		/// <summary>マーチャントID</summary>
		public const string MERCHANT_ID = "MerchantId";
		/// <summary>マーチャントパスワード</summary>
		public const string MERCHANT_PASS = "MerchantPass";
		/// <summary>処理連番</summary>
		public const string TRANSACTION_ID = "TransactionId";
		/// <summary>取引日付</summary>
		public const string TRANSACTION_DATE = "TransactionDate";
		/// <summary>処理区分</summary>
		public const string OPERATE_ID = "OperateId";
		/// <summary>貴社自由領域1</summary>
		public const string MERCHANT_FREE1 = "MerchantFree1";
		/// <summary>貴社自由領域2</summary>
		public const string MERCHANT_FREE2 = "MerchantFree2";
		/// <summary>貴社自由領域3</summary>
		public const string MERCHANT_FREE3 = "MerchantFree3 ";
		/// <summary>プロセスID</summary>
		public const string PROCESS_ID = "ProcessId";
		/// <summary>プロセスパスワード</summary>
		public const string PROCESS_PASS = "ProcessPass";
		/// <summary>処理結果コード</summary>
		public const string RESPONSE_CD = "ResponseCd";
		/// <summary>店舗コード</summary>
		public const string TENANT_ID = "TenantId";
		/// <summary>会員ID</summary>
		public const string KAIIN_ID = "KaiinId";
		/// <summary>カードNo</summary>
		public const string CARD_NO = "CardNo";
		/// <summary>有効期限</summary>
		public const string CARD_EXP = "CardExp";
		/// <summary>支払回数</summary>
		public const string PAY_TYPE = "PayType";
		/// <summary>利用金額</summary>
		public const string AMOUNT = "Amount";
		/// <summary>セキュリティコード</summary>
		public const string SEC_CD = "SecCd";
		/// <summary>かな姓</summary>
		public const string KANA_SEI = "KanaSei";
		/// <summary>かな名</summary>
		public const string KANA_MEI = "KanaMei";
		/// <summary>電話番号</summary>
		public const string TEL_NO = "TelNo";
		/// <summary>性別</summary>
		public const string SEX = "Sex";
		/// <summary>誕生日(月日)</summary>
		public const string BIRTH_DAY = "BirthDay";
		/// <summary>誕生日(年)</summary>
		public const string BIRTH_YEAR = "BirthYear";
		/// <summary>郵便番号</summary>
		public const string POSTAL = "Postal";
		/// <summary>銀行口座番号上四桁</summary>
		public const string ACNT_NO1 = "AcntNo1";
		/// <summary>銀行口座番号下四桁</summary>
		public const string ACNT_NO2 = "AcntNo2";
		/// <summary>カード名義</summary>
		public const string CARD_MEI = "CardMei";
		/// <summary>3D メッセージバージョン番号</summary>
		public const string MESSAGE_VERSION_NO_3D = "MessageVersionNo3D";
		/// <summary>3DトランザクションID</summary>
		public const string TRANSACTION_ID_3D = "TransactionId3D";
		/// <summary>エンコード済みXID</summary>
		public const string ENCODE_X_ID_3D = "EncodeXId3D";
		/// <summary>3Dセキュアのトランザクションステータス</summary>
		public const string TRANSACTION_STATUS_3D = "TransactionStatus3D";
		/// <summary>CAVVアルゴリズム</summary>
		public const string CAVV_ALGORITHM_3D = "CAVVAlgorithm3D";
		/// <summary>CAVV</summary>
		public const string CAVV3D = "CAVV3D";
		/// <summary>3Dセキュア認証結果</summary>
		public const string ECI3D = "ECI3D";
		/// <summary>カード番号(3D)</summary>
		public const string PAN_CARD_NO_3D = "PANCardNo3D";
		/// <summary>カード会社コード</summary>
		public const string COMPANY_CD = "CompanyCd";
		/// <summary>承認番号</summary>
		public const string APPROVE_NO = "ApproveNo";
		/// <summary>認証結果1(セキュリティコードの認証成否)</summary>
		public const string MC_SEC_CD = "McSecCd";
		/// <summary>認証結果2(かな姓)</summary>
		public const string MC_KANA_SEI = "McKanaSei";
		/// <summary>認証結果3(かな名)</summary>
		public const string MC_KANA_MEI = "McKanaMei";
		/// <summary>認証結果4(誕生日(月日))</summary>
		public const string MC_BIRTH_DAY = "McBirthDay";
		/// <summary>認証結果5(電話番号)</summary>
		public const string MC_TEL_NO = "McTelNo";
		/// <summary>認証結果6(性別)</summary>
		public const string MC_SEX = "McSex";
		/// <summary>認証結果7(誕生日(年))</summary>
		public const string MC_BIRTH_YEAR = "McBirthYear";
		/// <summary>認証結果8(郵便番号)</summary>
		public const string MC_POSTAL = "McPostal";
		/// <summary>認証結果9(銀行口座番号上四桁)</summary>
		public const string MC_ACNT_NO1 = "McAcntNo1";
		/// <summary>認証結果9(銀行口座番号下四桁)</summary>
		public const string MC_ACNT_NO2 = "McAcntNo2";
		/// <summary>認証結果9(カード名義)</summary>
		public const string MC_CARD_MEI = "McCardMei";
		/// <summary>トークン</summary>
		public const string TOKEN = "Token";
		/// <summary>POST用URL</summary>
		public const string POST_URL = "PostUrl";
		/// <summary>処理連番</summary>
		public const string PROC_NO = "ProcNo";
		/// <summary>処理連番</summary>
		public const string AUTH_KAIIN_ADD_FLG = "AuthKaiinAddFlg";
		/// <summary>会員ID自動採番利用フラグ</summary>
		public const string KAIIN_ID_AUTO_RIYO_FLG = "KaiinIdAutoRiyoFlg";
		/// <summary>会員パスワード</summary>
		public const string KAIIN_PASS = "KaiinPass";
		/// <summary>売り上げ計上日</summary>
		public const string SALES_DATE = "SalesDate";

		/// <summary>会員ID自動採番利用フラグ:オフ</summary>
		public const string FLG_KAIIN_ID_AUTO_RIYO_FLG_OFF = "0";

		/// <summary>オペレータID : 与信</summary>
		public const string OPERATOR_ID_MASTER_1AUTH = "1Auth";
		/// <summary>オペレータID : 与信と売上計上</summary>
		public const string OPERATOR_ID_MASTER_1GATHERING = "1Gathering";
		/// <summary>オペレータID : カード確認</summary>
		public const string OPERATOR_ID_MASTER_1CHECK = "1Check";
		/// <summary>オペレータID : 会員追加</summary>
		public const string OPERATOR_ID_MEMBER_4MEMADD = "4MemAdd";
		/// <summary>オペレータID : 会員削除</summary>
		public const string OPERATOR_ID_MEMBER_4MEMDEL = "4MemDel";
		/// <summary>オペレータID : 会員無効</summary>
		public const string OPERATOR_ID_MEMBER_4MEMINVAL = "4MemInval";
		/// <summary>オペレータID : 会員参照(マスキング)</summary>
		public const string OPERATOR_ID_MEMBER_4MEMREF = "4MemRefM";
		/// <summary>オペレータID : 売り上げ計上</summary>
		public const string OPERATOR_ID_PROCESS_1CAPTURE = "1Capture";
		/// <summary>オペレータID : 金額変更</summary>
		public const string OPERATOR_ID_PROCESS_1CHANGE = "1Change";
		/// <summary>オペレータID : 与信削除</summary>
		public const string OPERATOR_ID_PROCESS_1DELETE = "1Delete";
		/// <summary>オペレータID : 再与信</summary>
		public const string OPERATOR_ID_PROCESS_1REAUTH = "1ReAuth";

		// セキュリティコードの定数
		/// <summary>マッチ</summary>
		public const string MS_SEC_CD_MATCH = "0";
		/// <summary>アンマッチ</summary>
		public const string MS_SEC_CD_UNMATCH = "1";
		/// <summary>代行中</summary>
		public const string MS_SEC_CD_ACTING = "5";
		/// <summary>セット無し</summary>
		public const string MS_SEC_CD_NOTSET = "8";
		/// <summary>照会不可</summary>
		public const string MS_SEC_CD_INQUIRYNOTPOSSIBLE = "9";

		/// <summary>成功時の応答</summary>
		public const string REQUEST_APPROVED = "OK";
	}
}
