/*
=========================================================================================================
  Module      : LPワーカーサービス(LandingPageWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using w2.App.Common.Design;
using w2.Cms.Manager.Codes.Util;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.LandingPage;
using w2.App.Common.LandingPage.LandingPageDesignData;
using w2.App.Common.Order;
using w2.App.Common.RefreshFileManager;
using w2.App.Common.Util;
using w2.App.Common.Manager.Menu;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.FeatureImage;
using w2.Cms.Manager.ParamModels.LandingPaeg;
using w2.Cms.Manager.ViewModels.LandingPage;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Common.Helper;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.Affiliate;
using w2.Domain.ContentsLog;
using w2.Domain.FeatureImage;
using w2.Domain.LandingPage;
using w2.Domain.Payment;
using w2.Domain.Product;
using w2.Domain.Product.Helper;
using w2.Domain.ProductCategory;
using w2.Domain.SubscriptionBox;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// LPワーカーサービス
	/// </summary>
	public class LandingPageWorkerService : BaseWorkerService
	{
		/// <summary>ランディングページ画像パス</summary>
		public static string m_landingPagePath = @"Contents\LandingPage\";
		/// <summary>一時ランディングページ画像パス</summary>
		public static string m_landingPageTemporaryPath = @"Contents\Temp\";
		/// <summary>ランディングページ画像ルート</summary>
		public string m_landingPageRoot = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, m_landingPagePath);
		/// <summary>一時ランディングページ画像ルート</summary>
		public string m_landingPageTemporaryRoot = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, m_landingPageTemporaryPath);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="landingPageListParam">LP一覧のパラメタ</param>
		/// <returns>ビューモデル</returns>
		public LandingPageListViewModel GetListView(LandingPageListParamModel landingPageListParam)
		{
			var landingPageList = new LandingPageService().SearchByNumberOfPages(
				landingPageListParam.PublicDateKbn,
				landingPageListParam.SearchWord,
				landingPageListParam.SearchPublicStatus,
				landingPageListParam.SearchPublicDesignMode,
				((landingPageListParam.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST) + 1,
				landingPageListParam.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST);

			var result = new LandingPageListViewModel
			{
				ActionStatus = ActionStatus.List,
				Items = landingPageList.Select(this.CreateDetailView).ToArray(),
				ParamModel = landingPageListParam,
				HitCount = landingPageList.Length != 0 ? landingPageList[0].RowCount : 0,
			};
			return result;
		}

		/// <summary>
		/// 詳細ビュー生成
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>詳細ビュー</returns>
		private LandingPageListViewModel.LandingPageListItemDetailViewModel CreateDetailView(LandingPageDesignModel model)
		{
			InitializeImageFolder(model.PageId);

			var dateChanged = DateTimeUtility.ToStringForManager(
				model.DateChanged,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter).Split(new[] { " " }, StringSplitOptions.None);

			var thumbnailUrlPc = WebBrowserCapture.GetImageFilePath(
				Path.Combine(Constants.CMS_LANDING_PAGE_DIR_PATH_PC, string.Format("{0}.aspx", model.PageFileName)),
				device: WebBrowserCapture.Device.Pc,
				delay: 100,
				iSizeH: 800,
				iSizeW: 800,
				bSizeH: 1280,
				bSizeW: 720);
			var thumbnailUrlSp = WebBrowserCapture.GetImageFilePath(
				Path.Combine(Constants.CMS_LANDING_PAGE_DIR_PATH_SP, string.Format("{0}.aspx", model.PageFileName)),
				device: WebBrowserCapture.Device.Sp,
				delay: 100,
				iSizeH: 400,
				iSizeW: 400,
				bSizeH: 400,
				bSizeW: 800);

			var summary = new ContentsLogService().GetContentsSummaryData(Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_LANDINGCART, model.PageId);
			var summaryToday = new ContentsLogService().GetContentsSummaryDataOfToday(Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_LANDINGCART, model.PageId);
			var rtn = new LandingPageListViewModel.LandingPageListItemDetailViewModel
			{
				CvCount = ((summary.Any()) ? summary[0].CvCount : 0) + ((summaryToday.Any()) ? summaryToday[0].CvCount : 0),
				CvPrice = ((summary.Any()) ? summary[0].Price : 0) + ((summaryToday.Any()) ? summaryToday[0].Price : 0),
				DateChanged1 = dateChanged[0],
				DateChanged2 = dateChanged[1],
				PageId = model.PageId,
				ManagementTitle = model.ManagementTitle,
				PageUrl = LpDesignHelper.GetLpPageUrlPc(model.PageFileName),
				PublicStatus = model.PublicStatus,
				DesignMode = model.DesignMode,
				UsePublicRange = ((model.PublicEndDatetime.HasValue) || (model.PublicStartDatetime.HasValue)),
				ViewCount = ((summary.Any()) ? summary[0].PvCount : 0) + ((summaryToday.Any()) ? summaryToday[0].PvCount : 0),
				ThumbnailUrlPc = (File.Exists(Constants.PHYSICALDIRPATH_CMS_MANAGER + thumbnailUrlPc))
					? (Constants.PATH_ROOT_CMS + thumbnailUrlPc).Replace(@"\", "/")
					: Constants.PATH_ROOT_CMS + Constants.IMG_MANAGER_NO_IMAGE,
				ThumbnailUrlSp = (File.Exists(Constants.PHYSICALDIRPATH_CMS_MANAGER + thumbnailUrlSp))
					? (Constants.PATH_ROOT_CMS + thumbnailUrlSp).Replace(@"\", "/")
					: Constants.PATH_ROOT_CMS + Constants.IMG_MANAGER_NO_IMAGE
			};
			rtn.ProductSet = model.ProductSets.Select(
				ms =>
				{
					var ps = new LandingPageListViewModel.LandingPageListProductSet
					{
						Products = ms.Products.Select(
							m =>
							{
								var p = new LandingPageListViewModel.LandingPageListProduct();
								var dv = ProductCommon.GetProductVariationInfo(
									m.ShopId,
									m.ProductId,
									m.VariationId,
									null);
								p.Quantity = m.Quantity;
								if (dv.Count > 0)
								{
									p.ItemImageUrl = this.CreateProductImageUrl(dv[0]);
									p.ProductName = ProductCommon.CreateProductJointName(dv[0]);
								}
								return p;
							}).ToArray(),
						ActionStatus = ActionStatus.List
					};
					return ps;
				}).ToArray();

			return rtn;
		}

		/// <summary>
		/// 商品画像URL取得
		/// </summary>
		/// <param name="drv">商品情報</param>
		/// <returns>URL</returns>
		private string CreateProductImageUrl(DataRowView drv)
		{
			var imgHead = StringUtility.ToEmpty(drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD]);
			var rtn = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC
				+ (string.IsNullOrEmpty(imgHead)
					? string.Format("{0}{1}", Constants.PATH_PRODUCTIMAGES, Constants.PRODUCTIMAGE_NOIMAGE_PC)
					: string.Format("{0}0/{1}{2}", Constants.PATH_PRODUCTIMAGES, imgHead, Constants.PRODUCTIMAGE_FOOTER_M));
			return rtn;
		}

		/// <summary>
		/// LP詳細View取得
		/// </summary>
		/// <param name="id">取得対象のID</param>
		/// <returns>LP詳細View</returns>
		public LandingPageDetailViewModel GetLpDetailViewModel(string id)
		{
			var sv = new LandingPageService();
			var model = sv.GetPageDataWithDesign(id);
			var rtn = new LandingPageDetailViewModel();
			rtn.PageId = model.PageId;
			rtn.PageTitle = model.PageTitle;
			rtn.PageUrl = model.PageFileName;
			rtn.ProductChooseType = model.ProductChooseType;
			rtn.UserRegistrationType = model.UserRegistrationType;
			rtn.LoginFormType = (model.LoginFormType == LandingPageConst.LOGIN_FORM_TYPE_VISIBLE);
			rtn.PublicStatus = model.PublicStatus;
			rtn.RangeStartDate = model.PublicStartDatetime.HasValue ? model.PublicStartDatetime.Value.ToString("yyyy/MM/dd") : "";
			rtn.RangeStartTime = model.PublicStartDatetime.HasValue ? model.PublicStartDatetime.Value.ToString("HH:mm") : "";
			rtn.RangeEndDate = model.PublicEndDatetime.HasValue ? model.PublicEndDatetime.Value.ToString("yyyy/MM/dd") : "";
			rtn.RangeEndTime = model.PublicEndDatetime.HasValue ? model.PublicEndDatetime.Value.ToString("HH:mm") : "";
			var paymentModels = new PaymentService().GetValidAll(this.SessionWrapper.LoginShopId);
			paymentModels = paymentModels
				.Where(paymentmodel => paymentmodel.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
				.Where(paymentmodel => paymentmodel.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE).ToArray();

			var userblePaymentIdList = rtn.GetUseblePaymentIdList(paymentModels);
			rtn.ChoiceUnpermittedPaymentIds = userblePaymentIdList;
			rtn.ChoiceDefaultCheckedPayment = userblePaymentIdList;
			rtn.DesignMode = model.DesignMode;

			var lpTagList = new List<SelectListItem>();
			foreach (var tag in new AffiliateTagSettingService().GetAllIncludeConditionModels())
			{
				if (tag.AffiliateTagConditionList.Any(condition => condition.ConditionValue == "Landing/")
					&& tag.ValidFlg == Constants.FLG_AFFILIATETAGSETTING_VALID_FLG_VALID)
				{
					lpTagList.Add(new SelectListItem()
					{
						Text = tag.AffiliateName,
						Value = tag.AffiliateId.ToString()
					});
				};
			}
			rtn.ChoiceTagSettingList = lpTagList.ToArray();
			rtn.TagSettingList = model.TagSettingList.Split(',');

			var socialLoginTypeList = new List<SelectListItem>
			{
				new SelectListItem()
				{
					Text = LandingPageConst.SocialLoginType.Apple.ToString(),
					Value = LandingPageConst.SocialLoginType.Apple.ToText()
				},
				new SelectListItem()
				{
					Text = LandingPageConst.SocialLoginType.FaceBook.ToString(),
					Value = LandingPageConst.SocialLoginType.FaceBook.ToText()
				},
				new SelectListItem()
				{
					Text = LandingPageConst.SocialLoginType.Line.ToString(),
					Value = LandingPageConst.SocialLoginType.Line.ToText()
				},
				new SelectListItem()
				{
					Text = ValueText.GetValueText(
						LandingPageConst.SNS_NAME_TEXT,
						LandingPageConst.SNS_NAME_TWITTER_TEXT,
						LandingPageConst.SNS_NAME_TWITTER_VALUE),
					Value = LandingPageConst.SocialLoginType.Twitter.ToText()
				},
				new SelectListItem()
				{
					Text = LandingPageConst.SocialLoginType.Yahoo.ToString(),
					Value = LandingPageConst.SocialLoginType.Yahoo.ToText()
				},
                new SelectListItem()
                {
                    Text = LandingPageConst.SocialLoginType.Gplus.ToString(),
                    Value = LandingPageConst.SocialLoginType.Gplus.ToText()
                }
            };

			if (Constants.PAYPAL_LOGINPAYMENT_ENABLED)
			{
				socialLoginTypeList.Add(
					new SelectListItem()
					{
						Text = LandingPageConst.SocialLoginType.PayPal.ToString(),
						Value = LandingPageConst.SocialLoginType.PayPal.ToText()
					});
			}
			if (Constants.RAKUTEN_LOGIN_ENABLED)
			{
				socialLoginTypeList.Add(
					new SelectListItem
					{
						Text = LandingPageConst.SocialLoginType.Rakuten.ToString(),
						Value = LandingPageConst.SocialLoginType.Rakuten.ToText()
					});
			}

			if (Constants.CART_LIST_LP_OPTION && model.IsCartListLp)
			{
				rtn.CanDisplayControlCartListLp = false;
			}
			rtn.ChoiceSocialLoginList = socialLoginTypeList.ToArray();
			rtn.CanEditShortUrl = CommonMenuUtility.HasAuthorityCms(
				this.SessionWrapper.LoginOperator,
				Constants.PATH_ROOT_CMS + "ShortUrl/");
			rtn.SocialLoginUseType = model.SocialLoginUseType;
			rtn.SocialLoginList = model.SocialLoginList.Split(',');
			rtn.EfoCubeUseFlg = (model.EfoCubeUseFlg == LandingPageConst.EFO_CUBE_USE_FLG_ON);
			rtn.OrderConfirmPageSkipFlg = (model.OrderConfirmPageSkipFlg == LandingPageConst.ORDER_CONFIRM_PAGE_SKIP_FLG_ON);
			rtn.MailAddressConfirmFormUseFlg = (model.MailAddressConfirmFormUseFlg == LandingPageConst.MAIL_ADDRESS_CONFIRM_FORM_USE_FLG_ON);
			rtn.NoveltyUseFlg = (model.NoveltyUseFlg == LandingPageConst.NOVELTY_USE_FLG_ON);
			rtn.UnpermittedPaymentIds = model.UnpermittedPaymentIds.Split(',');

			UpdateFileOpenTime(LpDesignHelper.GetLpPageFilePathPc(model.PageFileName));
			UpdateFileOpenTime(LpDesignHelper.GetLpPageFilePathSp(model.PageFileName));
			rtn.MetadataDesc = model.MetadataDesc;
			rtn.ManagementTitle = model.ManagementTitle;
			rtn.DefaultPaymentId = model.DefaultPaymentId;
			rtn.PaymentChooseType = model.PaymentChooseType;
			rtn.PersonalAuthenticationUseFlg = (model.PersonalAuthenticationUseFlg == LandingPageConst.PERSONAL_AUTHENTICATION_USE_FLG_ON);
			return rtn;
		}

		/// <summary>
		/// LPドロップダウンリスト取得
		/// </summary>
		/// <returns>LP詳細View</returns>
		public LandingPageDetailViewModel GetLpDefaultDropDownList()
		{
			var rtn = new LandingPageDetailViewModel();
			var paymentModels = new PaymentService().GetValidAll(this.SessionWrapper.LoginShopId);

			var userblePaymentIdList = rtn.GetUseblePaymentIdList(paymentModels);
			userblePaymentIdList = userblePaymentIdList
				.Where(paymentmodel => paymentmodel.Value != Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
				.Where(paymentmodel => paymentmodel.Value != Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE).ToArray();
			rtn.ChoiceUnpermittedPaymentIds = userblePaymentIdList;
			rtn.ChoiceDefaultCheckedPayment = userblePaymentIdList;

			var lpTagList = new List<SelectListItem>();
			foreach (var tag in new AffiliateTagSettingService().GetAllIncludeConditionModels())
			{
				if (tag.AffiliateTagConditionList.Any(condition => condition.ConditionValue == "Landing/")
					&& tag.ValidFlg == Constants.FLG_AFFILIATETAGSETTING_VALID_FLG_VALID)
				{
					lpTagList.Add(new SelectListItem()
					{
						Text = tag.AffiliateName,
						Value = tag.AffiliateId.ToString(),
					});
				};
			}
			rtn.ChoiceTagSettingList = lpTagList.ToArray();

			var socialLoginTypeList = new List<SelectListItem>
			{
				new SelectListItem()
				{
					Text = LandingPageConst.SocialLoginType.Apple.ToString(),
					Value = LandingPageConst.SocialLoginType.Apple.ToText()
				},
				new SelectListItem()
				{
					Text = LandingPageConst.SocialLoginType.FaceBook.ToString(),
					Value = LandingPageConst.SocialLoginType.FaceBook.ToText(),
				},
				new SelectListItem()
				{
					Text = LandingPageConst.SocialLoginType.Line.ToString(),
					Value = LandingPageConst.SocialLoginType.Line.ToText(),
				},
				new SelectListItem()
				{
					Text = ValueText.GetValueText(
						LandingPageConst.SNS_NAME_TEXT,
						LandingPageConst.SNS_NAME_TWITTER_TEXT,
						LandingPageConst.SNS_NAME_TWITTER_VALUE),
					Value = LandingPageConst.SocialLoginType.Twitter.ToText(),
				},
				new SelectListItem()
				{
					Text = LandingPageConst.SocialLoginType.Yahoo.ToString(),
					Value = LandingPageConst.SocialLoginType.Yahoo.ToText(),
				},
                new SelectListItem()
                {
                    Text = LandingPageConst.SocialLoginType.Gplus.ToString(),
                    Value = LandingPageConst.SocialLoginType.Gplus.ToText()
                }
            };

			if (Constants.PAYPAL_LOGINPAYMENT_ENABLED)
			{
				socialLoginTypeList.Add(
					new SelectListItem()
					{
						Text = LandingPageConst.SocialLoginType.PayPal.ToString(),
						Value = LandingPageConst.SocialLoginType.PayPal.ToText(),
					});
			}
			if (Constants.RAKUTEN_LOGIN_ENABLED)
			{
				socialLoginTypeList.Add(
					new SelectListItem
					{
						Text = LandingPageConst.SocialLoginType.Rakuten.ToString(),
						Value = LandingPageConst.SocialLoginType.Rakuten.ToText(),
					});
			}
			rtn.ChoiceSocialLoginList = socialLoginTypeList.ToArray();
			rtn.CanEditShortUrl = CommonMenuUtility.HasAuthorityCms(
				this.SessionWrapper.LoginOperator,
				Constants.PATH_ROOT_CMS + "ShortUrl/");

			if (rtn.EnabledSubscriptionBox)
			{
				// 頒布会を取得
				rtn.SubscriptionBoxes = new SubscriptionBoxService().GetValidSubscriptionBox()
					.Select(m => new SelectListItem
					{
						Text = m.ManagementName,
						Value = m.CourseId
					}).ToArray();
			}
			return rtn;
		}

		/// <summary>
		/// Lp商品ビュー取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <return>Lp商品ビュー</return>
		public LandingPageProductViewModel GetLpProductViewModel(string pageId)
		{
			var sv = new LandingPageService();
			var model = sv.GetPageProductSets(pageId);

			var rtn = new LandingPageProductViewModel();
			rtn.PageId = pageId;

			rtn.EnabledSubscriptionBox = Constants.SUBSCRIPTION_BOX_OPTION_ENABLED;

			rtn.ProductSets = model.Select(
				ms =>
				{
					var ps = new LandingPageProductViewModel.ProductSet
					{
						Products = ms.Products.Select(
							m =>
							{
								var p = new LandingPageProductViewModel.Product();
								var dv = ProductCommon.GetProductVariationInfo(
									m.ShopId,
									m.ProductId,
									m.VariationId,
									null);
								p.PageId = pageId;
								p.ShopId = m.ShopId;
								p.ProductId = m.ProductId;
								p.VariationId = m.VariationId;
								p.Quantity = m.Quantity.ToString();
								p.VariationSortNumber = m.VariationSortNumber;
								p.BuyType = m.BuyType;
								if (dv.Count > 0)
								{
									p.ProductImage = this.CreateProductImageUrl(dv[0]);
									p.ProductName = ProductCommon.CreateProductJointName(dv[0]);
									p.FixedPurchaseFlg = (dv[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG].ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID);
									p.ShippingId = (dv[0][Constants.FIELD_PRODUCT_SHIPPING_ID]).ToString();
								}
								return p;
							}).ToArray(),
						SetName = ms.SetName,
						ValidFlg = (ms.ValidFlg == LandingPageConst.PRODUCT_SET_VALID_FLG_VALID),
						SubscriptionBoxCourseId = ms.SubscriptionBoxCourseId,
						SubscriptionBoxCourseFlg = (string.IsNullOrEmpty(ms.SubscriptionBoxCourseId) == false)
					};
					return ps;
				}).ToArray();

			return rtn;
		}

		/// <summary>
		/// Lp商品ビュー取得
		/// </summary>
		/// <param name="selectedProducts">選択商品</param>
		/// <returns>Lp商品ビュー</returns>
		public LandingPageProductViewModel GetLpProducts(ProductSearchResultModel[] selectedProducts)
		{
			var rtn = new LandingPageProductViewModel();
			var service = new ProductService();
			var list = new List<LandingPageProductViewModel.Product>();
			rtn.ProductSets = new List<LandingPageProductViewModel.ProductSet> { new LandingPageProductViewModel.ProductSet() };
			foreach (var product in selectedProducts)
			{
				var pId = product.ProductId;
				var vId = product.VariationId;
				var model = service.GetProductVariation(this.SessionWrapper.LoginShopId, pId, vId, "");
				var p = new LandingPageProductViewModel.Product();
				p.ProductImage = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC
					+ (string.IsNullOrEmpty(model.VariationImageHead)
						? string.Format("{0}{1}", Constants.PATH_PRODUCTIMAGES, Constants.PRODUCTIMAGE_NOIMAGE_PC)
						: string.Format(
							"{0}0/{1}{2}",
							Constants.PATH_PRODUCTIMAGES,
							model.VariationImageHead,
							Constants.PRODUCTIMAGE_FOOTER_M));
				p.ShopId = model.ShopId;
				p.ProductId = model.ProductId;
				p.VariationId = model.VariationId;
				p.Quantity = "1";
				p.ProductName = ProductCommon.CreateProductJointName(model.DataSource);
				p.FixedPurchaseFlg = (model.FixedPurchaseFlg != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID);
				p.ShippingId = model.ShippingId;
				p.BuyType = LandingPageConst.BUY_TYPE_NORMAL;

				list.Add(p);
			}
			rtn.ProductSets.First().Products = list.ToArray();
			return rtn;
		}

		/// <summary>
		/// 総件数を取得
		/// </summary>
		/// <param name="paramModel">パラムモデル</param>
		/// <returns>総件数</returns>
		public int GetSearchHitCountOnCms(ProductSearchParamModel paramModel)
		{
			paramModel.BeginRowNumber = (paramModel.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1;
			paramModel.EndRowNumber = paramModel.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST;

			var count = new ProductService().GetSearchVariationHitCountOnCms(paramModel, this.SessionWrapper.LoginShopId);
			return count;
		}

		/// <summary>
		/// 商品検索
		/// </summary>
		/// <param name="paramModel">パラメタモデル</param>
		/// <returns>検索結果</returns>
		public ProductSearchResultModel[] ProductSearch(ProductSearchParamModel paramModel)
		{
			paramModel.BeginRowNumber = (paramModel.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1;
			paramModel.EndRowNumber = paramModel.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST;
			var totalCount = new ProductService().GetSearchVariationHitCountOnCms(paramModel, this.SessionWrapper.LoginShopId);
			var countHtml = paramModel.BeginRowNumber + "-"
				+ ((totalCount > paramModel.EndRowNumber) ? StringUtility.ToNumeric(paramModel.EndRowNumber) : StringUtility.ToNumeric(totalCount))
				+ "/" + StringUtility.ToNumeric(totalCount);

			var productList = new ProductService().SearchVariationOnCms(paramModel, this.SessionWrapper.LoginShopId);
			var service = new ProductCategoryService();
			var vm = productList
				.Where(pm => (pm.SubscriptionBoxFlg != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_ONLY))
				.Select(
				pl => new ProductSearchResultModel
				{
					ProductName = pl.Name,
					VariationName = pl.VariationName1,
					ProductId = pl.ProductId,
					VariationId = pl.VariationId,
					CategoryName = string.IsNullOrEmpty(pl.CategoryId1) == false
						? service.Get(pl.CategoryId1) != null ? service.Get(pl.CategoryId1).Name : string.Empty
						: string.Empty,
					Price = pl.Price.ToPriceString(true),
					SellStartDate = (pl.SellFrom).ToString("yyyy/MM/dd"),
					CountHtml = countHtml
				}).ToArray();
			return vm;
		}

		/// <summary>
		/// デザイナビュー取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>デザイナビュー</returns>
		public LandingPageDesignerViewModel GetDesignerView(string pageId, string designType)
		{
			var sv = new LandingPageService();
			var model = sv.GetPageDataWithDesign(pageId, designType);
			var rtn = new LandingPageDesignerViewModel
			{
				ActionStatus = ActionStatus.Detail,
				Input = new LandingPageInput(model),
				PublicStatus = model.PublicStatus,
				DateChanged = model.DateChanged.ToString("yyyy/MM/dd"),
				DesignType = designType
			};

			rtn.Input.Pagedesign = new PageDesignInput
			{
				PageId = model.PageId,
				BlockSettings = model.Blocks.Select(b => new BlockDesignInput
				{
					BlockClassName = b.BlockClassName,
					BlockIndex = b.BlockIndex.ToString(),
					Elements = b.Elements.Select(e => new BlockElementInput
					{
						ElementIndex = e.ElementIndex.ToString(),
						ElementPlaceHolderName = e.ElementPlaceHolderName,
						Attributes = e.Attributes.Select(a => new BlockKeyValue
						{
							Attribute = a.Attribute,
							Value = a.Value
						}).ToArray()
					}).ToArray()
				}).ToArray()
			};
			return rtn;
		}

		/// <summary>
		/// Lpデータ更新
		/// </summary>
		/// <param name="input">Lp入力値</param>
		/// <returns>
		/// チェックメッセージ
		/// 成功時はempty
		/// </returns>
		public string UpdateLpData(LandingPageInput input)
		{
			// 入力チェック
			var errorMsg = input.CreateErrorJoinMessage(input.Validate(false));
			if (string.IsNullOrEmpty(errorMsg) == false) return errorMsg;

			// ファイル名チェック
			var sv = new LandingPageService();

			// 更新前の情報
			var befModel = sv.GetPageDataWithDesign(input.PageId);
			var pages = sv.GetPageByFileName(input.PageUrl);

			// ファイル名を変更した場合にチェック
			if (befModel.PageFileName != input.PageUrl)
			{
				// 1つ以上の場合、すでに同じものがあるのでNG
				if (pages.Length > 0) return WebMessages.LandingPageFileNameDuplicate;
			}

			// ファイル名に使えないものがあるのでNG
			if (input.PageUrl.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) return WebMessages.LandingPageFileNameInvalidChar;

			var model = input.CreateModel();
			model.LastChanged = base.SessionWrapper.LoginOperatorName;

			// 更新
			if (sv.UpdatePage(model) > 0)
			{
				// キャッシュ更新
				RefreshFileManagerProvider.GetInstance(RefreshFileType.LandingPageDesign).CreateUpdateRefreshFile();
			};

			CopyImagesToMainFolder(model.PageId);

			// 更新時にデザインファイルを再作成する場合のみ
			if (Constants.CMS_LANDING_PAGE_RECREATE_DESIGN_FILE_ON_UPDATE)
			{
				var lpPageFilePathPc = LpDesignHelper.GetLpPageFilePathPc(model.PageFileName);
				var lpPageFilePathSp = LpDesignHelper.GetLpPageFilePathSp(model.PageFileName);
				if (IsFileTimestampNewerThanOpened(lpPageFilePathPc)
					|| IsFileTimestampNewerThanOpened(lpPageFilePathSp))
				{
					return WebMessages.FileOldError;
				}

				if ((befModel.DesignMode != model.DesignMode)
					|| (model.DesignMode == Constants.FLG_LANDINGPAGEDESIGN_DESIGN_MODE_DEFAULT))
				{
					// 古いファイルを削除
					LpDesignHelper.DeletePageFilePc(befModel);
					LpDesignHelper.DeletePageFileSp(befModel);
				}

				// ファイル作成
				this.CreateLandingPageFile(model, befModel:befModel, copySourceId:string.Empty);

				if ((model.DesignMode == Constants.FLG_LANDINGPAGEDESIGN_DESIGN_MODE_CUSTOM)
				&& (befModel.PageFileName != model.PageFileName))
				{
					// 古いファイルを削除
					LpDesignHelper.DeletePageFilePc(befModel);
					LpDesignHelper.DeletePageFileSp(befModel);
				}

				// ページサムネイル
				WebBrowserCapture.Create(
					Constants.PHYSICALDIRPATH_CMS_MANAGER,
					Path.Combine(Constants.CMS_LANDING_PAGE_DIR_PATH_PC, string.Format("{0}.aspx", model.PageFileName)),
					device: WebBrowserCapture.Device.Pc,
					delay: 100,
					iSizeH: 800,
					iSizeW: 800,
					bSizeH: 1280,
					bSizeW: 720);

				// SP使う場合のみサムネイル生成
				if (DesignCommon.UseSmartPhone)
				{
					WebBrowserCapture.Create(
						Constants.PHYSICALDIRPATH_CMS_MANAGER,
						Path.Combine(
							Constants.CMS_LANDING_PAGE_DIR_PATH_SP,
							string.Format("{0}.aspx", model.PageFileName)),
						device: WebBrowserCapture.Device.Sp,
						delay: 100,
						iSizeH: 400,
						iSizeW: 400,
						bSizeH: 400,
						bSizeW: 800);
				}
			}

			return "";
		}

		/// <summary>
		/// 最新ファイル既に開いているか
		/// </summary>
		/// <param name="filePath">チェック対象パス</param>
		/// <returns>最新ファイル既に開いているか</returns>
		private bool IsFileTimestampNewerThanOpened(string filePath)
		{
			var result = ((string.IsNullOrEmpty(filePath) == false)
				&& (this.SessionWrapper.GetLpPageOpenDateTime(filePath) != null)
				&& (File.GetLastWriteTime(filePath) > this.SessionWrapper.GetLpPageOpenDateTime(filePath)));
			return result;
		}

		/// <summary>
		/// Lpデータ削除
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>Message</returns>
		public string DeleteLpData(string pageId)
		{
			// 削除
			var sv = new LandingPageService();
			var model = sv.GetPageDataWithDesign(pageId);

			if (Constants.CART_LIST_LP_OPTION && model.IsCartListLp)
			{
				return WebMessages.LandingPageCanNotDeleteCartListLpError;
			}

			sv.DeletePageData(pageId);
			// ファイル削除
			LpDesignHelper.DeletePageFilePc(model);
			LpDesignHelper.DeletePageFileSp(model);

			return string.Empty;
		}

		/// <summary>
		/// Lpデータ登録
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns>
		/// チェックメッセージ
		/// 成功時はempty
		/// </returns>
		/// <remarks>デザインはデフォルトのものを登録</remarks>
		public string RegisterLpData(LandingPageInput input)
		{
			// 入力チェック
			var errorMessageList = input.Validate(true);
			int count;
			var errorMessageMaxCount = CheckMaxLpCount(out count);
			if (string.IsNullOrEmpty(errorMessageMaxCount) == false)
			{
				errorMessageList.Add(errorMessageMaxCount);
			}
			var errorMsg = input.CreateErrorJoinMessage(errorMessageList);
			if (string.IsNullOrEmpty(errorMsg) == false) return errorMsg;

			// ファイル名チェック
			var sv = new LandingPageService();
			var pages = sv.GetPageByFileName(input.PageUrl);

			// 1つ以上の場合、すでに同じものがあるのでNG
			if (pages.Length > 0) return WebMessages.LandingPageFileNameDuplicate;

			// ファイル名に使えないものがあるのでNG
			if (input.PageUrl.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) return WebMessages.LandingPageFileNameInvalidChar;

			var model = input.CreateModel();
			model.LastChanged = base.SessionWrapper.LoginOperatorName;

			// PageID発行
			model.PageId = string.IsNullOrEmpty(this.SessionWrapper.LandingPageId)
				? NumberingUtility.CreateNewNumber(base.SessionWrapper.LoginShopId, "LandingPageId").ToString().PadLeft(10, '0')
				: this.SessionWrapper.LandingPageId;

			if (string.IsNullOrEmpty(input.CopySourceId))
			{
				// コピー元のIDがなければデフォのデザインセット
				var defdata = LpDesignHelper._GetDefaultBlockJsonData();
				model.Blocks = defdata.First(def => def.DesignType == LandingPageConst.PAGE_DESIGN_TYPE_PC)
					.BlockSettings.Select(
					b => new LandingPageDesignBlockModel()
					{
						PageId = model.PageId,
						BlockClassName = b.BlockClassName,
						BlockIndex = int.Parse(b.BlockIndex),
						DesignType = LandingPageConst.PAGE_DESIGN_TYPE_PC,
						LastChanged = base.SessionWrapper.LoginOperatorName,
						Elements = b.Elements.Select(
							e => new LandingPageDesignElementModel()
							{
								PageId = model.PageId,
								BlockIndex = int.Parse(b.BlockIndex),
								DesignType = LandingPageConst.PAGE_DESIGN_TYPE_PC,
								ElementIndex = int.Parse(e.ElementIndex),
								ElementPlaceHolderName = e.ElementPlaceHolderName,
								LastChanged = base.SessionWrapper.LoginOperatorName,
								Attributes = e.Attributes.Select(
									a => new LandingPageDesignAttributeModel()
									{
										PageId = model.PageId,
										BlockIndex = int.Parse(b.BlockIndex),
										DesignType = LandingPageConst.PAGE_DESIGN_TYPE_PC,
										ElementIndex = int.Parse(e.ElementIndex),
										Attribute = a.Attribute,
										Value = ImagePathExclusionDomain(a.Attribute, a.Value),
										LastChanged = base.SessionWrapper.LoginOperatorName
									}).ToArray()
							}).ToArray()
					}).ToArray();

				// SP使わなくてもここで作っとかないとSP有効後に初期状態のブロックデータ作るタイミングがないのでここで作成しておく
				model.Blocks = model.Blocks.Concat(
					defdata.First(def => def.DesignType == LandingPageConst.PAGE_DESIGN_TYPE_SP).BlockSettings.Select(
						b => new LandingPageDesignBlockModel()
						{
							PageId = model.PageId,
							BlockClassName = b.BlockClassName,
							BlockIndex = int.Parse(b.BlockIndex),
							DesignType = LandingPageConst.PAGE_DESIGN_TYPE_SP,
							LastChanged = base.SessionWrapper.LoginOperatorName,
							Elements = b.Elements.Select(
								e => new LandingPageDesignElementModel()
								{
									PageId = model.PageId,
									BlockIndex = int.Parse(b.BlockIndex),
									DesignType = LandingPageConst.PAGE_DESIGN_TYPE_SP,
									ElementIndex = int.Parse(e.ElementIndex),
									ElementPlaceHolderName = e.ElementPlaceHolderName,
									LastChanged = base.SessionWrapper.LoginOperatorName,
									Attributes = e.Attributes.Select(
										a => new LandingPageDesignAttributeModel()
										{
											PageId = model.PageId,
											BlockIndex = int.Parse(b.BlockIndex),
											DesignType = LandingPageConst.PAGE_DESIGN_TYPE_SP,
											ElementIndex = int.Parse(e.ElementIndex),
											Attribute = a.Attribute,
											Value = ImagePathExclusionDomain(a.Attribute, a.Value),
											LastChanged = base.SessionWrapper.LoginOperatorName
										}).ToArray()
								}).ToArray()
						}).ToArray()).ToArray();
			}
			else
			{
				// コピー元のTempフォルダにあるOGP画像をコピー先のTempフォルダにコピー
				var error = CopyPreviousToCurrentTempImage(input.CopySourceId, model.PageId);
				if (string.IsNullOrEmpty(error) == false) return error;

				// コピー元のIDがある場合はコピー元のデザインをセット
				var sourceDesign = sv.GetPageDataWithDesign(input.CopySourceId);
				model.Blocks = sourceDesign.Blocks.Select(
					(b) =>
					{
						b.PageId = model.PageId;
						b.Elements = b.Elements.Select(
							(e) =>
							{
								e.PageId = model.PageId;
								e.Attributes = e.Attributes.Select(
									(a) =>
									{
										a.PageId = model.PageId;
										return a;
									}).ToArray();
								return e;
							}).ToArray();
						return b;
					}).ToArray();
			}

			// 登録
			sv.Insert(model);
			input.PageId = model.PageId;

			CopyImagesToMainFolder(model.PageId);
			this.SessionWrapper.LandingPageId = string.Empty;

			// ファイル作成
			this.CreateLandingPageFile(model, befModel:null, copySourceId:input.CopySourceId);

			// ページサムネイル
			WebBrowserCapture.Create(
				Constants.PHYSICALDIRPATH_CMS_MANAGER,
				Path.Combine(Constants.CMS_LANDING_PAGE_DIR_PATH_PC, string.Format("{0}.aspx", model.PageFileName)),
				device: WebBrowserCapture.Device.Pc,
				delay: 100,
				iSizeH: 800,
				iSizeW: 800,
				bSizeH: 1280,
				bSizeW: 720);

			// SP使う場合のみサムネイル生成
			if (DesignCommon.UseSmartPhone)
			{
				WebBrowserCapture.Create(
					Constants.PHYSICALDIRPATH_CMS_MANAGER,
					Path.Combine(Constants.CMS_LANDING_PAGE_DIR_PATH_SP, string.Format("{0}.aspx", model.PageFileName)),
					device: WebBrowserCapture.Device.Sp,
					delay: 100,
					iSizeH: 400,
					iSizeW: 400,
					bSizeH: 400,
					bSizeW: 800);
			}
			return "";
		}

		/// <summary>
		/// Lpページデザインファイル生成
		/// </summary>
		/// <param name="aftModel">更新後モデル</param>
		/// <param name="befModel">更新前モデル</param>
		/// <param name="copySourceId">コピー元 ページID</param>
		private void CreateLandingPageFile(LandingPageDesignModel aftModel, LandingPageDesignModel befModel = null, string copySourceId = "")
		{
			var copySourceModel = (string.IsNullOrEmpty(copySourceId) == false)
				? new LandingPageService().Get(copySourceId)
				: null;
			LpDesignHelper.WriteLpPageFilePc(aftModel, befModel, copySourceModel);
			LpDesignHelper.WriteLpPageFileSp(aftModel, befModel, copySourceModel);
		}

		/// <summary>
		/// デザイン更新
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns>
		/// チェックメッセージ
		/// 成功時はempty
		/// </returns>
		public string UpdateLpDesign(PageDesignInput input)
		{
			// 更新
			var sv = new LandingPageService();
			var model = new LandingPageDesignModel()
			{
				PageId = input.PageId
			};

			model.Blocks = input.BlockSettings.Select(b => new LandingPageDesignBlockModel
			{
				PageId = model.PageId,
				BlockClassName = b.BlockClassName,
				BlockIndex = int.Parse(b.BlockIndex),
				DesignType = input.DesignType,
				LastChanged = base.SessionWrapper.LoginOperatorName,
				Elements = b.Elements.Select(e => new LandingPageDesignElementModel()
				{
					PageId = model.PageId,
					BlockIndex = int.Parse(b.BlockIndex),
					DesignType = input.DesignType,
					ElementIndex = int.Parse(e.ElementIndex),
					ElementPlaceHolderName = e.ElementPlaceHolderName,
					LastChanged = base.SessionWrapper.LoginOperatorName,
					Attributes = e.Attributes.Select(a => new LandingPageDesignAttributeModel()
					{
						PageId = model.PageId,
						BlockIndex = int.Parse(b.BlockIndex),
						DesignType = input.DesignType,
						ElementIndex = int.Parse(e.ElementIndex),
						Attribute = a.Attribute,
						Value = ImagePathExclusionDomain(a.Attribute, a.Value),
						LastChanged = base.SessionWrapper.LoginOperatorName
					}).ToArray()
				}).ToArray()
			}).ToArray();
			sv.UpdatePageDesign(model);
			return "";
		}

		/// <summary>
		/// 画像リスト取得
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="keyWord">キーワード</param>
		/// <returns>画像リスト</returns>
		public ViewModels.FeatureImage.GroupListViewModel GetImageList(string groupId, string keyWord)
		{
			var pm = new FeatureImageListSearchParamModel();
			var wSv = new FeatureImageWorkerService();
			pm.GroupId = groupId;
			pm.Keyword = keyWord;
			var vm = wSv.CreateGroupListVm(pm);
			return vm;
		}

		/// <summary>
		/// 画像グループビュー取得
		/// </summary>
		/// <returns>画像グループビュー</returns>
		public LandingPageImageGroupViewModel GetImageGroupListItems()
		{
			var otherGroup = ValueTextForCms.GetValueSelectListItems(
				Constants.TABLE_FEATUREIMAGEGROUP,
				Constants.FIELD_FEATUREIMAGEGROUP_GROUP_NAME);

			var items = otherGroup.Concat(new FeatureImageService().GetAllGroup().Select(
				g => new SelectListItem
				{
					Value = g.GroupId.ToString(),
					Text = g.GroupName
				})).ToArray();

			var rtn = new LandingPageImageGroupViewModel();
			rtn.GroupListItems = items;
			return rtn;
		}

		/// <summary>
		/// ブロックJSON取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>JSON</returns>
		public string GetBlocksJson(string pageId)
		{
			var sv = new LandingPageService();
			var model = sv.GetPageDataWithDesign(pageId);

			var design = new List<PageDesignInput>()
			{
				new PageDesignInput
				{
					PageId = model.PageId,
					DesignType = LandingPageConst.PAGE_DESIGN_TYPE_PC,
					BlockSettings = model.Blocks.Where(b => b.DesignType == LandingPageConst.PAGE_DESIGN_TYPE_PC).Select(b => new BlockDesignInput
					{
						BlockClassName = b.BlockClassName,
						BlockIndex = b.BlockIndex.ToString(),
						Elements = b.Elements.Select(e => new BlockElementInput
						{
							ElementIndex = e.ElementIndex.ToString(),
							ElementPlaceHolderName = e.ElementPlaceHolderName,
							Attributes = e.Attributes.Select(a => new BlockKeyValue
							{
								Attribute = a.Attribute,
								Value = a.Value.Replace(Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC, "@@ rep_pc_site_root @@")
							}).ToArray()
						}).ToArray()
					}).ToArray()
				},
				new PageDesignInput
				{
					PageId = model.PageId,
					DesignType = LandingPageConst.PAGE_DESIGN_TYPE_SP,
					BlockSettings = model.Blocks.Where(b => b.DesignType == LandingPageConst.PAGE_DESIGN_TYPE_SP).Select(b => new BlockDesignInput
					{
						BlockClassName = b.BlockClassName,
						BlockIndex = b.BlockIndex.ToString(),
						Elements = b.Elements.Select(e => new BlockElementInput
						{
							ElementIndex = e.ElementIndex.ToString(),
							ElementPlaceHolderName = e.ElementPlaceHolderName,
							Attributes = e.Attributes.Select(a => new BlockKeyValue
							{
								Attribute = a.Attribute,
								Value = a.Value.Replace(Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC, "@@ rep_pc_site_root @@")
							}).ToArray()
						}).ToArray()
					}).ToArray()
				}
			};

			// JSONのページIDは0にする
			design.ForEach(d => d.PageId = "0");

			return JsonConvert.SerializeObject(design);
		}

		/// <summary>
		/// プレビューファイル生成
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>プレビュー用キー</returns>
		public string CreatePreviewFile(LandingPageInput input, string designType)
		{
			var sv = new LandingPageService();
			var previewModel = sv.GetPageDataWithDesign(input.PageId, designType);
			var inputModel = input.CreateModel();
			previewModel.ProductChooseType = inputModel.ProductChooseType;
			previewModel.LoginFormType = inputModel.LoginFormType;
			previewModel.PageTitle = inputModel.PageTitle;
			previewModel.SocialLoginList = inputModel.SocialLoginList;
			previewModel.SocialLoginUseType = inputModel.SocialLoginUseType;
			previewModel.OrderConfirmPageSkipFlg = inputModel.OrderConfirmPageSkipFlg;
			previewModel.EfoCubeUseFlg = inputModel.EfoCubeUseFlg;
			previewModel.MailAddressConfirmFormUseFlg = inputModel.MailAddressConfirmFormUseFlg;
			previewModel.NoveltyUseFlg = inputModel.NoveltyUseFlg;
			previewModel.UnpermittedPaymentIds = inputModel.UnpermittedPaymentIds;
			previewModel.ProductSets = inputModel.ProductSets;
			previewModel.UserRegistrationType = input.UserRegistrationType;
			previewModel.PaymentChooseType = input.PaymentChooseType;
			previewModel.DefaultPaymentId = input.DefaultPaymentId ?? "";
			previewModel.DesignMode = input.DesignMode;
			previewModel.PersonalAuthenticationUseFlg = inputModel.PersonalAuthenticationUseFlg;
			var key = LpDesignHelper.WritePreviewFile(previewModel, designType);
			return key;
		}

		/// <summary>
		/// プレビューファイル生成
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>プレビューキー</returns>
		public string CreatePreviewFile(PageDesignInput input, string designType)
		{
			var sv = new LandingPageService();
			var model = sv.GetPageDataWithDesign(input.PageId, designType);
			model.Blocks = input.BlockSettings.Select(b => new LandingPageDesignBlockModel
			{
				PageId = model.PageId,
				BlockClassName = b.BlockClassName,
				BlockIndex = int.Parse(b.BlockIndex),
				DesignType = input.DesignType,
				LastChanged = base.SessionWrapper.LoginOperatorName,
				DateChanged = DateTime.Now,
				DateCreated = DateTime.Now,
				Elements = b.Elements.Select(e => new LandingPageDesignElementModel()
				{
					PageId = model.PageId,
					BlockIndex = int.Parse(b.BlockIndex),
					DesignType = input.DesignType,
					ElementIndex = int.Parse(e.ElementIndex),
					ElementPlaceHolderName = e.ElementPlaceHolderName,
					LastChanged = base.SessionWrapper.LoginOperatorName,
					DateChanged = DateTime.Now,
					DateCreated = DateTime.Now,
					Attributes = e.Attributes.Select(a => new LandingPageDesignAttributeModel()
					{
						PageId = model.PageId,
						BlockIndex = int.Parse(b.BlockIndex),
						DesignType = input.DesignType,
						ElementIndex = int.Parse(e.ElementIndex),
						Attribute = a.Attribute,
						Value = a.Value,
						LastChanged = base.SessionWrapper.LoginOperatorName,
						DateChanged = DateTime.Now,
						DateCreated = DateTime.Now,
					}).ToArray()
				}).ToArray()
			}).ToArray();
			var key = LpDesignHelper.WritePreviewFile(model, designType);
			return key;
		}

		/// <summary>
		/// プレビュービュー生成
		/// </summary>
		/// <param name="previewKey">プレビューキー</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>プレビュービュー</returns>
		public PreviewViewModel CreatePreviewVm(string previewKey, string designType)
		{
			var url = LpDesignHelper.GetPreviewUrl(previewKey, designType);
			return new PreviewViewModel() { PreviewUrl = url };
		}

		/// <summary>
		/// 画像フォルダを初期化
		/// </summary>
		/// <param name="pageId">ページID</param>
		public void InitializeImageFolder(string pageId)
		{
			var sourceDir = Path.Combine(m_landingPageRoot, pageId);
			if (Directory.Exists(sourceDir) == false) Directory.CreateDirectory(sourceDir);

			var tempDir = Path.Combine(
				m_landingPageTemporaryRoot,
				this.SessionWrapper.LoginOperator.OperatorId,
				Constants.PATH_TEMP_LANDINGPAGE,
				pageId);
			if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
			Directory.CreateDirectory(tempDir);
			CoordinateWorkerService.CopyDirectory(sourceDir, tempDir);
		}

		/// <summary>
		/// アップロード済画像ビューモデルを作成
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns></returns>
		public UploadedImageViewModel CreateUploadedImageVm(string pageId)
		{
			if (string.IsNullOrEmpty(pageId))
			{
				pageId = string.IsNullOrEmpty(this.SessionWrapper.LandingPageId)
					? NumberingUtility.CreateNewNumber(base.SessionWrapper.LoginShopId, "LandingPageId").ToString().PadLeft(10, '0')
					: this.SessionWrapper.LandingPageId;
				this.SessionWrapper.LandingPageId = pageId;
			}
			var vm = new UploadedImageViewModel(pageId, this.SessionWrapper.LoginOperator.OperatorId);
			return vm;
		}

		/// <summary>
		/// 画像アップロード
		/// </summary>
		/// <param name="image">画像</param>
		/// <param name="pageId">コーディネートID</param>
		public string Upload(HttpPostedFileBase image, string pageId)
		{
			if (image == null) return WebMessages.LandingPageNotFileError;
			if (string.IsNullOrEmpty(pageId))
			{
				pageId = string.IsNullOrEmpty(this.SessionWrapper.LandingPageId)
					? NumberingUtility.CreateNewNumber(base.SessionWrapper.LoginShopId, "LandingPageId").ToString().PadLeft(10, '0')
					: this.SessionWrapper.LandingPageId;
				this.SessionWrapper.LandingPageId = pageId;
			}
			var target = Path.Combine(
				m_landingPageTemporaryRoot,
				this.SessionWrapper.LoginOperator.OperatorId,
				Constants.PATH_TEMP_LANDINGPAGE,
				pageId);

			if (Directory.Exists(target) == false) Directory.CreateDirectory(target);

			var uploadPath = Path.Combine(target, pageId + Constants.CONTENTS_IMAGE_FIRST);

			try
			{
				if (File.Exists(uploadPath)) File.Delete(uploadPath);
				image.SaveAs(uploadPath);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return WebMessages.ContentsManagerFileOperationError;
			}

			return string.Empty;
		}

		/// <summary>
		/// 画像リストからコピー
		/// </summary>
		/// <param name="path">パス</param>
		/// <param name="pageId">ページID</param>
		public string CopyFromImageList(string path, string pageId)
		{
			var originalPath = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, path);
			if (File.Exists(originalPath) == false) return string.Empty;
			if (string.IsNullOrEmpty(pageId))
			{
				pageId = string.IsNullOrEmpty(this.SessionWrapper.LandingPageId)
					? NumberingUtility.CreateNewNumber(base.SessionWrapper.LoginShopId, "LandingPageId").ToString().PadLeft(10, '0')
					: this.SessionWrapper.LandingPageId;
				this.SessionWrapper.LandingPageId = pageId;
			}

			var target = Path.Combine(
				m_landingPageTemporaryRoot,
				this.SessionWrapper.LoginOperator.OperatorId,
				Constants.PATH_TEMP_LANDINGPAGE,
				pageId);
			if (Directory.Exists(target) == false) Directory.CreateDirectory(target);

			var uploadPath = Path.Combine(target, pageId + Constants.CONTENTS_IMAGE_FIRST);

			try
			{
				if (File.Exists(uploadPath)) File.Delete(uploadPath);
				File.Copy(originalPath, uploadPath);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return WebMessages.ContentsManagerFileOperationError;
			}

			return string.Empty;
		}

		/// <summary>
		/// 画像削除
		/// </summary>
		/// <param name="imageNo">イメージNo</param>
		/// <param name="pageId">ページID</param>
		/// <returns>エラーメッセージ</returns>
		public string DeleteImage(int imageNo, string pageId)
		{
			if (string.IsNullOrEmpty(pageId))
			{
				pageId = this.SessionWrapper.LandingPageId;
			}
			var targetDir = Path.Combine(
				m_landingPageTemporaryRoot,
				this.SessionWrapper.LoginOperator.OperatorId,
				Constants.PATH_TEMP_LANDINGPAGE,
				pageId);
			var targetFile = Path.Combine(targetDir, pageId + Constants.CONTENTS_IMAGE_FIRST);
			var errorMessage = string.Empty;
			if (File.Exists(targetFile))
			{
				try
				{
					File.Delete(targetFile);
				}
				catch (Exception ex)
				{
					FileLogger.WriteError(ex);
					InitializeImageFolder(pageId);
					return WebMessages.LandingPageDeleteFileError;
				}
			}
			else
			{
				errorMessage = WebMessages.LandingPageDeleteFileError;
			}

			return errorMessage;
		}

		/// <summary>
		/// 画像をメインフォルダにコピー
		/// </summary>
		/// <param name="pageId">ページID</param>
		public void CopyImagesToMainFolder(string pageId)
		{
			var tempDir = Path.Combine(
				m_landingPageTemporaryRoot,
				this.SessionWrapper.LoginOperator.OperatorId,
				Constants.PATH_TEMP_LANDINGPAGE,
				pageId);
			var sourceDir = Path.Combine(m_landingPageRoot, pageId);
			if (Directory.Exists(tempDir))
			{
				try
				{
					if (Directory.Exists(sourceDir)) Directory.Delete(sourceDir, true);
					CoordinateWorkerService.CopyDirectory(tempDir, sourceDir);
					Directory.Delete(tempDir, true);
				}
				catch (Exception ex)
				{
					FileLogger.WriteError(ex);
					InitializeImageFolder(pageId);
				}
			}
		}

		/// <summary>
		/// LP新規作成状態取得
		/// </summary>
		/// <returns>ビューモデル</returns>
		public LandingPageListViewModel GetLpRegisterState()
		{
			int count;
			var errorMessage = CheckMaxLpCount(out count);

			var rtn = new LandingPageListViewModel
			{
				ItemCount = count,
				CanLpRegister = string.IsNullOrEmpty(errorMessage)
			};

			return rtn;
		}

		/// <summary>
		/// ABテストアイテム件数取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>件数</returns>
		public int GetCountInAbTestItemByPageId(string pageId)
		{
			var result = new LandingPageService().GetCountInAbTestItemByPageId(pageId);
			return result;
		}

		/// <summary>
		/// LPの最大作成数をチェック
		/// </summary>
		/// <param name="count">出力：LP作成数</param>
		/// <returns>エラーメッセージ</returns>
		private string CheckMaxLpCount(out int count)
		{
			count = new LandingPageService().GetCount();
			var errorMessage =
				((Constants.LPBUILDER_MAXCOUNT.HasValue) && (count >= Constants.LPBUILDER_MAXCOUNT))
					? WebMessages.LandingPageRegisterOverMaxCount.Replace(
						"@@ 1 @@",
						Constants.LPBUILDER_MAXCOUNT.ToString())
					: string.Empty;

			return errorMessage;
		}

		/// <summary>
		/// コピー前のページIDの画像をコピー後のページIDに移す。ただし、以下の動きもする
		/// ・コピー後のページIDにすでに画像がある場合、何もしない
		/// ・コピー前のページIDの画像が何もない場合、何もしない
		/// </summary>
		/// <param name="previousPageId">コピー前のページID</param>
		/// <param name="currentPageId">コピー後のページID</param>
		/// <returns>成功:空文字, 失敗:エラー文</returns>
		private string CopyPreviousToCurrentTempImage(string previousPageId, string currentPageId)
		{
			// コピー元にフォルダーやファイルがないならコピーせず終了
			var previousFolderPath = CreateTempImageFolder(previousPageId);
			if (Directory.Exists(previousFolderPath) == false) return string.Empty;
			var uplodedPath = CreateTempImagePath(previousPageId, previousFolderPath);
			if (File.Exists(uplodedPath) == false) return string.Empty;

			// コピー先にファイルがなければ新規フォルダを作成し続行
			var currnetFolderPath = CreateTempImageFolder(currentPageId);
			var uploadPath = CreateTempImagePath(currentPageId, currnetFolderPath);
			if (File.Exists(uploadPath)) return string.Empty;
			if (Directory.Exists(currnetFolderPath) == false) Directory.CreateDirectory(currnetFolderPath);

			try
			{
				File.Copy(uplodedPath, uploadPath);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return WebMessages.LandingPageCopyOgpImageError;
			}

			return string.Empty;
		}

		/// <summary>
		/// 一時フォルダ内のフォルダのパスの生成
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>フォルダのパス</returns>
		private string CreateTempImageFolder(string pageId)
		{
			return Path.Combine(
				m_landingPageTemporaryRoot,
				this.SessionWrapper.LoginOperator.OperatorId,
				Constants.PATH_TEMP_LANDINGPAGE,
				pageId);
		}

		/// <summary>
		/// 一時フォルダ内の画像パスの生成
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="folderPath">フォルダパス</param>
		/// <returns>画像のパス</returns>
		private string CreateTempImagePath(string pageId, string folderPath)
		{
			return Path.Combine(folderPath, pageId + Constants.CONTENTS_IMAGE_FIRST);
		}

		/// <summary>
		/// 画像パスからドメインを除外
		/// </summary>
		/// <param name="attributeName">要素名</param>
		/// <param name="value">要素内容</param>
		/// <returns>除隊した画像パス</returns>
		private string ImagePathExclusionDomain(string attributeName, string value)
		{
			if ((attributeName != Constants.FLG_LANDINGPAGEDESIGNATTRIBUTE_ATTRIBUTE_SRC)
				&& (attributeName != Constants.FLG_LANDINGPAGEDESIGNATTRIBUTE_ATTRIBUTE_BACKGROUND_IMAGE)) return value;

			var result = value.Replace(
				Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC,
				string.Empty);
			return result;
		}
	}
}
