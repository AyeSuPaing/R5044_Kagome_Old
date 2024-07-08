/*
=========================================================================================================
  Module      : LP 詳細ビューモデル(LandingPageDetailViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using System.Web.Mvc;
using w2.App.Common;
using w2.Domain.Payment;

namespace w2.Cms.Manager.ViewModels.LandingPage
{
	/// <summary>
	/// LP 詳細ビューモデル
	/// </summary>
	[Serializable]
	public class LandingPageDetailViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LandingPageDetailViewModel()
		{
			this.PageId = "";
			this.ChoiceUnpermittedPaymentIds = new[] { new SelectListItem() };
			this.CanDisplayControlCartListLp = true;
			this.CanEditShortUrl = true;
		}

		/// <summary>
		/// 決済モデルリスト取得
		/// </summary>
		/// <param name="paymentModels">決済モデルリスト</param>
		public SelectListItem[] GetUseblePaymentIdList(PaymentModel[] paymentModels)
		{
			return paymentModels
				.OrderBy(p => p.DisplayOrder)
				.ThenBy(p => p.PaymentId)
				.Select(
					p => new SelectListItem()
					{
						Text = p.PaymentName,
						Value = p.PaymentId
					}).ToArray();
		}

		/// <summary>ページID</summary>
		public string PageId { get; set; }
		/// <summary>タイトル</summary>
		public string PageTitle { get; set; }
		/// <summary>URL</summary>
		public string PageUrl { get; set; }
		/// <summary>商品選択タイプ</summary>
		public string ProductChooseType { get; set; }
		/// <summary>ユーザー登録タイプ</summary>
		public string UserRegistrationType { get; set; }
		/// <summary>ログインフォームタイプ</summary>
		public bool LoginFormType { get; set; }
		/// <summary>公開状態</summary>
		public string PublicStatus { get; set; }
		/// <summary>公開開始日</summary>
		public string RangeStartDate { get; set; }
		/// <summary>公開開始時間</summary>
		public string RangeStartTime { get; set; }
		/// <summary>公開終了日</summary>
		public string RangeEndDate { get; set; }
		/// <summary>公開終了時間</summary>
		public string RangeEndTime { get; set; }
		/// <summary>SEOディスクリプション</summary>
		public string MetadataDesc { get; set; }
		/// <summary>管理用タイトル</summary>
		public string ManagementTitle { get; set; }
		/// <summary>ソーシャルログイン利用の選択方法</summary>
		public string SocialLoginUseType { get; set; }
		/// <summary>利用するソーシャルログインリスト</summary>
		public string[] SocialLoginList { get; set; }
		/// <summary>利用するタグ設定リスト</summary>
		public string[] TagSettingList { get; set; }
		/// <summary>EFO CUBE利用フラグ</summary>
		public bool EfoCubeUseFlg { get; set; }
		/// <summary>確認画面スキップフラグ</summary>
		public bool OrderConfirmPageSkipFlg { get; set; }
		/// <summary>確認メールアドレスフォーム利用フラグ</summary>
		public bool MailAddressConfirmFormUseFlg { get; set; }
		/// <summary>決済除外リスト</summary>
		public string[] UnpermittedPaymentIds { get; set; }
		/// <summary>ドロップダウンリスト 決済除外リスト</summary>
		public SelectListItem[] ChoiceUnpermittedPaymentIds { get; set; }
		/// <summary>ドロップダウンリスト ソーシャルログインリスト</summary>
		public SelectListItem[] ChoiceSocialLoginList { get; set; }
		/// <summary>ドロップダウンリスト タグ設定リスト</summary>
		public SelectListItem[] ChoiceTagSettingList { get; set; }
		/// <summary>決済種別選択タイプ</summary>
		public string PaymentChooseType { get; set; }
		/// <summary>デフォルト決済種別</summary>
		public string DefaultPaymentId { get; set; }
		/// <summary>ドロップダウンリスト デフォルト決済種別</summary>
		public SelectListItem[] ChoiceDefaultCheckedPayment{ get; set; }
		/// <summary>ノベルティ利用フラグ</summary>
		public bool NoveltyUseFlg { get; set; }
		/// <summary>LPカートリストページのコントロールが表示可能か</summary>
		public bool CanDisplayControlCartListLp { get; set; }
		/// <summary>ショートURL機能利用可能か</summary>
		public bool CanEditShortUrl { get; set; }
		/// <summary>デザインモード</summary>
		public string DesignMode { get; set; }
		/// <summary>Personal authentication use flag</summary>
		public bool PersonalAuthenticationUseFlg { get; set; }
		/// <summary>List of Subscription Box</summary>
		public SelectListItem[] SubscriptionBoxes { get; set; }
		/// <summary>Enable subscription box option in configuration</summary>
		public bool EnabledSubscriptionBox
		{
			get { return Constants.SUBSCRIPTION_BOX_OPTION_ENABLED; }
		}
	}
}