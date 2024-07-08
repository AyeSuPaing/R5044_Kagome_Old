using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// SBPSReceiverFactory の概要の説明です
/// </summary>
public class SBPSReceiverFactory
{
	/// <summary>
	/// レシーバ作成
	/// </summary>
	/// <param name="request">リクエスト</param>
	/// <returns>レシーバ</returns>
	public static ISBPSReceiver Create(HttpRequest request)
	{
		var requestData = new SBPSApiRequestData(GetRequest(request));

		return Create(requestData);
	}
	/// <summary>
	/// レシーバ作成
	/// </summary>
	/// <param name="requestData">リクエストデータ</param>
	/// <returns>レシーバ</returns>
	private static ISBPSReceiver Create(SBPSApiRequestData requestData)
	{
		switch (requestData.FunctionId)
		{
			// クレジット与信	※廃止予定
			case "ST01-00101-101":
				return new SBPSCreditAuthReceiver(requestData);

			//  クレジット与信（トークン利用）
			case "ST01-00131-101":
				return new SBPSCreditAuthReceiver(requestData);

			// クレジット再与信
			case "ST01-00113-101":
				return new SBPSCreditReauthReceiver(requestData);

			// クレジットコミット
			case "ST02-00101-101":
				return new SBPSCreditCommitReceiver(requestData);

			// クレジット売上処理
			case "ST02-00201-101":
				return new SBPSCreditSaleReceiver(requestData);

			// クレジットキャンセル
			case "ST02-00303-101":
				return new SBPSCreditCancelReceiver(requestData);

			// クレジット顧客登録
			case "MG02-00101-101":
				return new SBPSCreditUserRegistReceiver(requestData);

			// クレジット顧客登録（トークン）
			case "MG02-00131-101":
				return new SBPSCreditUserRegistReceiver(requestData);

			// クレジット顧客参照
			case "MG02-00104-101":
				return new SBPSCreditUserReferReceiver(requestData);

			// コンビニ決済
			case "ST01-00101-701":
				return new SBPSCvsRegistReceiver(requestData);

			// ソフトバンク・ワイモバイルまとめて支払い売上
			case "ST02-00201-405":
				return new SBPSCareerSoftbankKetaiSaleReceiver(requestData);

			// ソフトバンク・ワイモバイルまとめて支払いキャンセル
			case "ST02-00303-405":
				return new SBPSCareerSoftbankKetaiCancelReceiver(requestData);
			
			// ソフトバンク・ワイモバイルまとめて支払い継続課金購入
			case "ST01-00104-405":
				return new SBPSCareerSoftbankKetaiContinuousOrderReceiver(requestData);

			// ドコモケータイ払い売上
			case "ST02-00201-401":
				return new SBPSCareerDocomoKetaiSaleReceiver(requestData);

			// ドコモケータイ払いキャンセル
			case "ST02-00303-401":
				return new SBPSCareerDocomoKetaiCancelReceiver(requestData);

			// ドコモケータイ払い継続課金購入
			case "ST01-00104-401":
				return new SBPSCareerDocomoKetaiContinuousOrderReceiver(requestData);

			// ドコモケータイ払い継続課金解約
			case "ST02-00309-401":
				return new SBPSCareerDocomoKetaiContinuousCancelReceiver(requestData);

			// auかんたん決済売上
			case "ST02-00201-402":
				return new SBPSCareerAuKantanSaleReceiver(requestData);

			// auかんたん決済キャンセル
			case "ST02-00303-402":
				return new SBPSCareerAuKantanCancelReceiver(requestData);

			// auかんたん決済継続課金購入
			case "ST01-00104-402":
				return new SBPSCareerAuKantanContinuousOrderReceiver(requestData);

			// auかんたん決済継続課金解約
			case "ST02-00309-402":
				return new SBPSCareerAuKantanContinuousCancelReceiver(requestData);

			// リクルートかんたん支払い売上
			case "ST02-00202-309":
				return new SBPSRecruitSaleReceiver(requestData);

			// リクルートかんたん支払いキャンセル
			case "ST02-00306-309":
				return new SBPSRecruitCancelReceiver(requestData);

			// PayPalキャンセル
			case "ST02-00303-306":
				return new SBPSPaypalCancelReceiver(requestData);

			// 楽天ペイ売上
			case "ST02-00201-305":
				return new SBPSRakutenIdSaleReceiver(requestData);

			// 楽天ペイキャンセル
			case "ST02-00306-305":
				return new SBPSRakutenIdCancelReceiver(requestData);

			// クレジット顧客登録（永久トークン）
			case "MG12-00107-101":
				return new SBPSCreditUserRegistReceiver(requestData);

			// PayPay sale
			case "ST02-00201-311":
				return new SBPSPayPaySaleReceiver(requestData);

			// PayPay cancel
			case "ST02-00303-311":
				return new SBPSPayPayCancelReceiver(requestData);

			default:
				throw new Exception("未対応の機能ID：" + requestData.FunctionId);
		}
	}

	/// <summary>
	/// リクエスト取得
	/// </summary>
	/// <returns>リクエスト文字列</returns>
	private static string GetRequest(HttpRequest request)
	{
		using (StreamReader sr = new StreamReader(request.InputStream))
		{
			return sr.ReadToEnd();
		}
	}
}