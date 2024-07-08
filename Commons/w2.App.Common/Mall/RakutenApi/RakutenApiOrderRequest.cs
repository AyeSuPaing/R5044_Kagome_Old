/*
=========================================================================================================
  Module      : 楽天ペイ受注情報取得API リクエスト (RakutenApiOrderRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace w2.App.Common.Mall.RakutenApi
{
	/// <summary>
	/// 楽天ペイ受注API getOrderAPIへのリクエストデータ
	/// </summary>
	public class RakutenApiOrderRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RakutenApiOrderRequest()
		{
			this.ApiUrl = "";
			this.ServiceSecret = "";
			this.LicenseKey = "";
			this.ContentType = "";
			this.OrderProgressList = new List<string>();
			this.SubStatusIdList = new List<string>();
			this.DateType = "";
			this.StartDateTime = "";
			this.EndDateTime = "";
			this.OrderTypeList = new List<string>();
			this.SettlementMethod = "";
			this.DeliveryName = "";
			this.ShippingDateBlankFlag = "";
			this.ShippingNumberBlankFlag = "";
			this.SearchKeywordType = "";
			this.SearchKeyword = "";
			this.MailSendType = "";
			this.OrdererMailAddress = "";
			this.PhoneNumberType = "";
			this.PhoneNumber = "";
			this.ReserveNumber = "";
			this.PurchaseSiteType = "";
			this.AsurakuFlag = "";
			this.CouponUseFlag = "";
			this.DrugFlag = "";
			this.OverseasFlag = "";
		}

		/// <summary>APIのURL</summary>
		public string ApiUrl { get; set; }
		/// <summary>サービスシークレット</summary>
		public string ServiceSecret { get; set; }
		/// <summary>ライセンスキー</summary>
		public string LicenseKey { get; set; }
		/// <summary>認証用文字列</summary>
		public string AuthKey
		{
			get
			{
				return "ESA "
					+ Convert.ToBase64String(
						Encoding.UTF8.GetBytes(
							string.Format("{0}:{1}", this.ServiceSecret, this.LicenseKey)));
			}
		}
		/// <summary>コンテンツタイプ</summary>
		public string ContentType { get; set; }
		/// <summary>ステータスリスト</summary>
		/// <remarks>
		/// 100: 注文確認待ち
		/// 200: 楽天処理中
		/// 300: 発送待ち
		/// 400: 変更確定待ち
		/// 500: 発送済
		/// 600: 支払手続き中
		/// 700: 支払手続き済
		/// 800: キャンセル確定待ち
		/// </remarks>
		public List<string> OrderProgressList { get; set; }
		/// <summary>サブステータスIDリスト</summary>
		public List<string> SubStatusIdList { get; set; }
		/// <summary>期間検索種別</summary>
		/// <remarks>
		/// 1: 注文日
		/// 2: 注文確認日
		/// 3: 注文確定日
		/// 4: 発送日
		/// 5: 発送完了報告日
		/// 6: 決済確定日
		/// </remarks>
		public string DateType { get; set; }
		/// <summary>期間検索開始日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		public string StartDateTime { get; set; }
		/// <summary>期間検索終了日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		public string EndDateTime { get; set; }
		/// <summary>販売種別リスト</summary>
		/// <remarks>
		/// 1: 通常購入
		/// 4: 定期購入
		/// 5: 頒布会
		/// 6: 予約商品
		/// </remarks>
		public List<string> OrderTypeList { get; set; }
		/// <summary>支払方法名</summary>
		/// <remarks>
		/// 1: クレジットカード
		/// 2: 代金引換
		/// 3: 後払い
		/// 4: ショッピングクレジット／ローン
		/// 5: オートローン
		/// 6: リース
		/// 7: 請求書払い
		/// 9: 銀行振込
		/// 12: Apple Pay
		/// 13: セブンイレブン（前払）
		/// 14: ローソン、郵便局ATM等（前払）
		/// </remarks>
		public string SettlementMethod { get; set; }
		/// <summary>配送方法</summary>
		public string DeliveryName { get; set; }
		/// <summary>発送日未指定有無フラグ</summary>
		/// <remarks>
		/// 0: 発送日の指定の有無によらず取得
		/// 1: 発送日が未指定のものだけを取得
		/// </remarks>
		public string ShippingDateBlankFlag { get; set; }
		/// <summary>お荷物伝票番号未指定有無フラグ</summary>
		/// <remarks>
		/// 0: お荷物伝票番号の指定の有無によらず取得
		/// 1: お荷物伝票番号が未指定のものだけを取得
		/// </remarks>
		public string ShippingNumberBlankFlag { get; set; }
		/// <summary>検索キーワード種別</summary>
		/// <remarks>
		/// 0: なし
		/// 1: 商品名
		/// 2: 商品番号
		/// 3: ひとことメモ
		/// 4: 注文者お名前
		/// 5: 注文者お名前フリガナ
		/// 6: 送付先お名前
		/// </remarks>
		public string SearchKeywordType { get; set; }
		/// <summary>検索キーワード</summary>
		/// <remarks>
		/// 機種依存文字などの不正文字以外
		/// 全角、半角にかかわらず32文字以下
		/// </remarks>
		public string SearchKeyword { get; set; }
		/// <summary>注文メールアドレス種別</summary>
		/// <remarks>
		/// 0: PC/モバイル
		/// 1: PC
		/// 2: モバイル
		/// </remarks>
		public string MailSendType { get; set; }
		/// <summary>注文者メールアドレス</summary>
		public string OrdererMailAddress { get; set; }
		/// <summary>電話番号種別</summary>
		/// <remarks>
		/// 1: 注文者
		/// 2: 送付先
		/// </remarks>
		public string PhoneNumberType { get; set; }
		/// <summary>電話番号</summary>
		public string PhoneNumber { get; set; }
		/// <summary>申込番号</summary>
		public string ReserveNumber { get; set; }
		/// <summary>購入サイトリスト</summary>
		/// <remarks>
		/// 0: すべて
		/// 1: PCで注文
		/// 2: モバイルで注文
		/// 3: スマートフォンで注文
		/// 4: タブレットで注文
		/// </remarks>
		public string PurchaseSiteType { get; set; }
		/// <summary>あす楽希望フラグ</summary>
		/// <remarks>
		/// 0: あす楽希望の有無によらず取得
		/// 1: あす楽希望のものだけを取得
		/// </remarks>
		public string AsurakuFlag { get; set; }
		/// <summary>クーポン利用有無フラグ</summary>
		/// <remarks>
		/// 0: クーポン利用の有無によらず取得
		/// 1: クーポン利用のものだけを取得
		/// </remarks>
		public string CouponUseFlag { get; set; }
		/// <summary>医薬品受注フラグ</summary>
		/// <remarks>
		/// 0: 医薬品の有無によらず取得
		/// 1: 医薬品を含む注文だけを取得
		/// </remarks>
		public string DrugFlag { get; set; }
		/// <summary>海外かご注文フラグ</summary>
		/// <remarks>
		/// 0: 海外カゴ注文の有無によらず取得
		/// 1: 海外カゴ注文のものだけを取得
		/// </remarks>
		public string OverseasFlag { get; set; }
	}
}
