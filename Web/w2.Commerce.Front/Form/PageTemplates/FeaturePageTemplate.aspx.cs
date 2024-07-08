/*
=========================================================================================================
  Module      : 特集ページ (FeaturePageTemplate.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.ContentsLog;
using w2.Domain.Favorite;
using w2.Domain.FeaturePage;
using w2.Domain.Product;
using w2.Domain.ProductGroup;
using w2.Domain.SetPromotion;

public partial class Form_PageTemplates_FeaturePageTemplate : ProductListPage
{
	#region ラップ済みコントロール宣言
	WrappedRepeater WrFeatureProductGroup { get { return GetWrappedControl<WrappedRepeater>("rFeatureProductGroup"); } }
	WrappedHiddenField WhfCategoryIdIncludeParamName { get { return GetWrappedControl<WrappedHiddenField>("hfCategoryIdIncludeParamName"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			if (this.SelectVariationList == null) this.SelectVariationList = new Dictionary<string, string>();
			GetFeaturePageData();
			SetProductData();
		}
	}

	/// <summary>
	/// 商品情報を取得
	/// </summary>
	/// <param name="productGroupItem"></param>
	/// <returns></returns>
	protected object GetProductData(FeaturePageProductGroupItem productGroupItem)
	{
		var fileName = Path.GetFileName(this.Request.Url.AbsolutePath);
		var previewFlg = fileName.Contains(".Preview.aspx");

		fileName = previewFlg
			? fileName.Replace(".Preview.aspx", string.Empty)
			: fileName;

		int currentFeaturePageNumber;
		if (int.TryParse(this.Request[Constants.REQUEST_KEY_PAGE_NO], out currentFeaturePageNumber) == false)
		{
			currentFeaturePageNumber = 1;
		}

		var model = DataCacheControllerFacade.GetFeaturePageCacheController()
			.CacheData
			.FirstOrDefault(
				item => string.Equals(
					item.FileName,
					fileName,
					StringComparison.CurrentCultureIgnoreCase));
		if (model == null) return null;

		var productContents = model.Contents
			.FirstOrDefault(
				item =>
					((item.ContentsType == Constants.FLG_FEATUREPAGECONTENTS_TYPE_PRODUCT_LIST)
						&& (item.ContentsKbn == Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_PC))
						&& (item.ProductGroupId == productGroupItem.ProductGroupId));
		if (productContents == null) return null;

		GetParameters();
		if ((model.PageType == Constants.FLG_FEATUREPAGE_GROUP)
			&& (productContents.Pagination == Constants.FLG_FEATUREPAGECONTENTS_PAGINATION_FLG_ON))
		{
			this.BgnNum = (currentFeaturePageNumber - 1) * productContents.DispNum + 1;
			this.EndNum = currentFeaturePageNumber * productContents.DispNum;
		}
		else
		{
			this.BgnNum = 1;
			this.EndNum = productContents.DispNum;
		}

		this.ProductGroupId = productGroupItem.ProductGroupId;
		this.SortKbn = "99";
		var info = GetProductListInfosFromCacheOrDb();

		var product = productGroupItem.Product;
		var filterForProductId = string.Format(
			"{0} = '{1}'",
			Constants.FIELD_PRODUCT_PRODUCT_ID,
			product.ProductId);

		// 商品IDで絞り込みを行う
		info.Products.RowFilter = filterForProductId;
		info.ProductVariations.RowFilter = filterForProductId;

		// バリエーションを使用していない場合、商品情報を返す
		if (product.HasProductVariation == false)
		{
			return info.Products[0];
		}

		// 該当商品のバリエーションが選択済みであればフィルターを行う
		if (this.SelectVariationList.ContainsKey(product.ProductId))
		{
			info.ProductVariations.RowFilter = string.Format(
				"{0} = '{1}'",
				Constants.FIELD_PRODUCT_VARIATION_ID,
				this.SelectVariationList[product.ProductId]);
		}
		this.ProductId = product.ProductId;
		this.ShopId = product.ShopId;	
		return info.ProductVariations[0];
	}

	/// <summary>
	/// 商品データをセット
	/// </summary>
	private void SetProductData()
	{
		var fileName = Path.GetFileName(this.Request.Url.AbsolutePath);
		var previewFlg = fileName.Contains(".Preview.aspx");

		// サイトがPCまたはレスポンシブかどうか
		var deviceType = (this.IsPc || SmartPhoneUtility.SmartPhoneSiteSettings.All(setting => setting.SmartPhonePageEnabled == false));

		if (previewFlg)
		{
			// プレビュー表示時にユーザーエージェントを考慮しないでPCフラグをセット
			foreach (var spSetting in SmartPhoneUtility.SmartPhoneSiteSettings)
			{
				if (string.IsNullOrEmpty(spSetting.RootPath)) continue;
				deviceType = (this.Request.Url.AbsolutePath.Contains(spSetting.RootPath.Substring(1)) == false);
			}

			fileName = fileName.Replace(".Preview.aspx", string.Empty);
		}

		int currentFeaturePageNumber;
		if (int.TryParse(this.Request[Constants.REQUEST_KEY_PAGE_NO], out currentFeaturePageNumber) == false)
		{
			currentFeaturePageNumber = 1;
		}

		var model = DataCacheControllerFacade.GetFeaturePageCacheController()
			.CacheData
			.FirstOrDefault(
				item => string.Equals(
					item.FileName,
					fileName,
					StringComparison.CurrentCultureIgnoreCase));

		if ((model == null) || (model.Contents.Length == 0))
		{
			SetPreviewData();
			return;
		}

		if (previewFlg == false)
		{
			var accessUser = GetReleaseRangeAccessUser();
			var result = new ReleaseRangeFeaturePage(model).Check(accessUser);
			ReleaseRangeRedirect(result);
		}

		this.FeaturePageContents = model.Contents
			.Where(
				item =>
					((item.ContentsType == Constants.FLG_FEATUREPAGECONTENTS_TYPE_PRODUCT_LIST)
						&& (item.ContentsKbn == Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_PC)))
			.ToList();

		GetParameters();

		var groupService = new ProductGroupService();
		this.FeaturePageProductGroup = new List<FeaturePageProductGroup>();

		foreach (var product in this.FeaturePageContents)
		{
			var groupModel = groupService.Get(product.ProductGroupId);

			if (groupModel == null) continue;

			if ((model.PageType == Constants.FLG_FEATUREPAGE_GROUP)
				&& (product.Pagination == Constants.FLG_FEATUREPAGECONTENTS_PAGINATION_FLG_ON))
			{
				this.PagerHtml = WebPager.CreateFeaturePagePager(
					groupModel.Items.Length,
					currentFeaturePageNumber,
					this.Request.FilePath,
					product.DispNum);

				this.BgnNum = (currentFeaturePageNumber - 1) * product.DispNum + 1;
				this.EndNum = currentFeaturePageNumber * product.DispNum;
			}
			else
			{
				this.BgnNum = 1;
				this.EndNum = product.DispNum;
			}

			this.ProductGroupId = groupModel.ProductGroupId;
			this.SortKbn = "99";
			var info = GetProductListInfosFromCacheOrDb();

			var productList = info.Products.Cast<DataRowView>().Select(drv => new ProductModel(drv)).ToList();
			var productGroupItems = new List<FeaturePageProductGroupItem>();
			foreach (var item in productList)
			{
				var variationinfo = info.ProductVariations;
				variationinfo.RowFilter = string.Format(
					"{0} = '{1}'",
					Constants.FIELD_PRODUCT_PRODUCT_ID,
					item.ProductId);

				var variationList = GetVariationAddCartList(variationinfo);
				if (this.SelectVariationList.ContainsKey(item.ProductId))
				{
					variationList = variationList
						.Where(
							variation =>
								(string)variation[Constants.FIELD_PRODUCT_VARIATION_ID] == this.SelectVariationList[item.ProductId])
						.ToList();
				}

				var groupItem = new FeaturePageProductGroupItem
				{
					ProductGroupId = groupModel.ProductGroupId,
					Product = item,
					ProductInfo = variationList[0],
				};
				productGroupItems.Add(groupItem);
			}

			var seeMore = ((model.PageType == Constants.FLG_FEATUREPAGE_MULTI_GROUP)
					&& (groupModel.Items.Length > product.DispNum))
				? new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_PRODUCT_LIST)
					.AddParam(Constants.REQUEST_KEY_PRODUCT_GROUP_ID, groupModel.ProductGroupId)
					.AddParam(Constants.REQUEST_KEY_SORT_KBN, Constants.KBN_SORT_PRODUCT_LIST_PRODUCT_GROUP_ITEM_NO_ASC)
					.CreateUrl()
				: string.Empty;

			var group = new FeaturePageProductGroup
			{
				Title = product.ProductListTitle,
				ViewMore = seeMore,
				ProductGroupItems = productGroupItems
			};
			this.FeaturePageProductGroup.Add(group);
		}

		lTitle.Text = model.GetTitle(deviceType);
		this.Sort = model.GetSort(deviceType);
		this.AltText = model.GetAltText(deviceType);

		if (this.ProductMasterList != null)
		{
			SetFavoriteDataForDisplay(this.ProductMasterList);
		}
		this.IsUserFixedPurchaseAble = CheckFixedPurchaseLimitedUserLevel(this.ShopId, this.ProductId) == false;
		// コンテンツログ出力
		ContentsLogUtility.InsertPageViewContentsLog(Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_FEATURE, model.FeaturePageId.ToString(), this.IsSmartPhone);

		this.WrFeatureProductGroup.DataSource = this.FeaturePageProductGroup;
		this.WrFeatureProductGroup.DataBind();
	}

	/// <summary>
	/// 特集ページ情報取得
	/// </summary>
	private void GetFeaturePageData()
	{
		// リクエストURLからファイル名取得
		var currentRequest = System.Web.HttpContext.Current.Request;
		var fileName = Path.GetFileName(currentRequest.Url.LocalPath);

		//Preview.aspxを末尾から除外
		var suffixToRemove = ".Preview.aspx";
		if (fileName.EndsWith(suffixToRemove, StringComparison.OrdinalIgnoreCase))
		{
			fileName = fileName.Substring(0, fileName.Length - suffixToRemove.Length);
		}

		// ファイル名から特集ページ情報を取得
		var executableFileName = Path.GetFileName(fileName);
		this.FeaturePageModel = new FeaturePageService().GetByFileName(executableFileName);

		//URLを編集して更新した場合、該当ファイルが存在しない都合上カテゴリIDがNULLになるため、初期値を代入する。
		if (this.FeaturePageModel.CategoryId == null) {
			this.PublishDateFrom = null;
			this.PublishDateTo = null;
			this.ChangedDate = null;
			this.FeaturePageModel.CategoryId = string.Empty;
			this.IsPublishDateTo = false;
		}
		else
		{
			this.PublishDateFrom = this.FeaturePageModel.ConditionPublishDateFrom;
			this.PublishDateTo = this.FeaturePageModel.ConditionPublishDateTo;
			this.ChangedDate = this.FeaturePageModel.DateChanged;
			this.IsPublishDateTo = (this.PublishDateTo != null);
		}
		//特集ページカテゴリIDをヒドゥンフィールドに格納
		WhfCategoryIdIncludeParamName.Value = string.Format(
			"{0}={1}",
			Constants.REQUEST_KEY_FEATURE_PAGE_CATEGORY_ID,
			this.FeaturePageModel.CategoryId.Substring(0, Math.Min(3, this.FeaturePageModel.CategoryId.Length)));

	}

	/// <summary>
	/// プレビュー表示用データ設定
	/// </summary>
	private void SetPreviewData()
	{
		var previewType = this.Request[Constants.REQUEST_KEY_FEATURE_PAGE_TYPE];
		var sampleGroupName = ValueText.GetValueText(Constants.TABLE_FEATUREPAGE, "sample_text", "group_title");
		var groupList = new List<FeaturePageProductGroup>();

		switch (previewType)
		{
			default:
				var group = new FeaturePageProductGroup
				{
					Title = sampleGroupName,
					ViewMore = string.Empty,
				};

				groupList.Add(group);
				break;

			case Constants.FLG_FEATUREPAGE_MULTI_GROUP:
				var group1 = new FeaturePageProductGroup
				{
					Title = sampleGroupName + "1",
					ViewMore = string.Empty,
				};
				var group2 = new FeaturePageProductGroup
				{
					Title = sampleGroupName + "2",
					ViewMore = string.Empty,
				};

				groupList.Add(group1);
				groupList.Add(group2);
				break;

			case Constants.FLG_FEATUREPAGE_SINGLE:
				var single = new FeaturePageProductGroup
				{
					Title = sampleGroupName,
					ViewMore = string.Empty,
				};

				groupList.Add(single);
				break;
		}

		this.WrFeatureProductGroup.DataSource = groupList;
		this.WrFeatureProductGroup.DataBind();
	}

	/// <summary>
	/// サンプル商品一覧作成
	/// </summary>
	/// <param name="sampleProductName">サンプル商品名</param>
	/// <returns>サンプル商品一覧</returns>
	private List<ProductModel> CreateSampleProductList(string sampleProductName)
	{
		var productList = new List<ProductModel>();
		var product = new ProductModel
		{
			Name = sampleProductName + "1",
			DisplayPrice = 1080,
			ImageHead = string.Empty,
		};

		var product2 = product.Clone();
		product2.Name = sampleProductName + "2";
		var product3 = product.Clone();
		product3.Name = sampleProductName + "3";
		var product4 = product.Clone();
		product4.Name = sampleProductName + "4";
		var product5 = product.Clone();
		product5.Name = sampleProductName + "5";

		productList.Add(product);
		productList.Add(product2);
		productList.Add(product3);
		productList.Add(product4);
		productList.Add(product5);

		return productList.ToList();
	}

	/// <summary>
	/// 商品一覧情報を取得（キャッシュorDBから）
	/// </summary>
	/// <returns>商品一覧情報</returns>
	private ProductListInfos GetProductListInfosFromCacheOrDb()
	{
		var cacheKey = "FeaturePageProductList " + string.Join(
			",",
			this.RequestParameter.Keys.Where(key => key != "current_date")
				.Select(key => key + "=" + this.RequestParameter[key]).ToArray());
		cacheKey = string.Format(
			"{0},MemberRankId={1},UserFixedPurchaseMemberFlg={2},GroupId={3}",
			cacheKey,
			this.MemberRankId,
			this.UserFixedPurchaseMemberFlg,
			this.ProductGroupId);

		// キャッシュまたはDBから取得
		var expire = Constants.PRODUCTLIST_CACHE_EXPIRE_MINUTES;
		var data = DataCacheManager.GetInstance().GetData(cacheKey, expire, CreateProductListInfosFromDb);
		return data;
	}

	/// <summary>
	/// DBから商品一覧情報作成
	/// </summary>
	/// <returns>商品一覧情報</returns>
	private ProductListInfos CreateProductListInfosFromDb()
	{
		// プロパティにセットする
		var products = GetProductsList();
		var productVariations = GetVariationList(products, this.MemberRankId);

		var productInfos = new ProductListInfos(
			products,
			productVariations,
			null,
			null);

		// 翻訳情報＋言語コードとかを設定してキャッシュさせる
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			var productTranslation = GetProductTranslationData(products);
			var productVariationTranslation = GetProductVariationTranslationData(productVariations);

			productInfos.SetProductListTranslationData(
				productTranslation,
				productVariationTranslation);
		}
		return productInfos;
	}

	/// <summary>
	/// 商品一覧取得SQLパラメタ作成
	/// </summary>
	/// <returns>SQLパラメタ</returns>
	public override Hashtable CreateGetProductListSqlParam()
	{
		// パラメタ作成
		var input = new Hashtable
		{
			{ Constants.FIELD_PRODUCT_SHOP_ID, this.ShopId },
			{ "current_date", DateTime.Now },
			{ "sort_kbn", 99 },
			{ "bgn_row_num", this.BgnNum },
			{ "end_row_num", this.EndNum },
			{ Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, this.MemberRankId },
			{ Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG, this.UserFixedPurchaseMemberFlg },
			{ "stock_existence_disp_kbn", "0" },
			{ "fixed_purchase_filter", "0" },
			{ "product_group_id", this.ProductGroupId }
		};

		if (Constants.GLOBAL_OPTION_ENABLE == false) return input;

		input.Add("language_code", RegionManager.GetInstance().Region.LanguageCode);
		input.Add("language_locale_id", RegionManager.GetInstance().Region.LanguageLocaleId);
		return input;
	}

	/// <summary>
	/// 商品グループアイテムリピータイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rFeatureProductGroupItem_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var productGroupItem = (FeaturePageProductGroupItem)e.Item.DataItem;
		if (productGroupItem.Product.HasProductVariation == false) return;

		int currentFeaturePageNumber;
		if (int.TryParse(this.Request[Constants.REQUEST_KEY_PAGE_NO], out currentFeaturePageNumber) == false)
		{
			currentFeaturePageNumber = 1;
		}

		var fileName = Path.GetFileName(this.Request.Url.AbsolutePath);
		var model = DataCacheControllerFacade.GetFeaturePageCacheController()
			.CacheData
			.FirstOrDefault(
				item => string.Equals(
					item.FileName,
					fileName,
					StringComparison.CurrentCultureIgnoreCase));
		if (model == null) return;

		var productContents = model.Contents
			.FirstOrDefault(
				item =>
					((item.ContentsType == Constants.FLG_FEATUREPAGECONTENTS_TYPE_PRODUCT_LIST)
						&& (item.ContentsKbn == Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_PC))
						&& (item.ProductGroupId == productGroupItem.ProductGroupId));
		if (productContents == null) return;

		if ((model.PageType == Constants.FLG_FEATUREPAGE_GROUP)
			&& (productContents.Pagination == Constants.FLG_FEATUREPAGECONTENTS_PAGINATION_FLG_ON))
		{
			this.BgnNum = (currentFeaturePageNumber - 1) * productContents.DispNum + 1;
			this.EndNum = currentFeaturePageNumber * productContents.DispNum;
		}
		else
		{
			this.BgnNum = 1;
			this.EndNum = productContents.DispNum;
		}
		this.ProductGroupId = productGroupItem.ProductGroupId;
		this.SortKbn = "99";
		var info = GetProductListInfosFromCacheOrDb();

		info.ProductVariations.RowFilter = string.Format(
			"{0} = '{1}'",
			Constants.FIELD_PRODUCT_PRODUCT_ID,
			productGroupItem.Product.ProductId);

		var variationList = GetVariationAddCartList(info.ProductVariations);
		var ddlVariation = new ListItemCollection();
		foreach (var variation in variationList)
		{
			ddlVariation.Add(
				new ListItem(
					CreateVariationName(
						(string)GetKeyValue(variation, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1),
						(string)GetKeyValue(variation, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2),
						(string)GetKeyValue(variation, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3)),
					(string)GetKeyValue(variation, Constants.FIELD_PRODUCT_VARIATION_ID)));
		}
		var wddlVariationList = GetWrappedControl<WrappedDropDownList>(e.Item, "ddlVariationList");
		wddlVariationList.DataSource = ddlVariation;
		wddlVariationList.DataBind();

		var productId = productGroupItem.Product.ProductId;
		if (this.SelectVariationList.ContainsKey(productId))
		{
			wddlVariationList.SelectedValue = this.SelectVariationList[productId];
		}
	}

	/// <summary>
	/// 商品バリエーションドロップダウン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlVariationList_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var ddlVariationList = (DropDownList)sender;
		var whfProductId = GetWrappedControl<WrappedHiddenField>(ddlVariationList.Parent, "hfProductId");
		var productId = whfProductId.Value;

		if (this.SelectVariationList.ContainsKey(productId))
		{
			this.SelectVariationList[productId] = ddlVariationList.SelectedValue;
		}
		else
		{
			this.SelectVariationList.Add(whfProductId.Value, ddlVariationList.SelectedValue);
		}

		SetProductData();
		ddlVariationList.Focus();
	}

	/// <summary>
	/// 商品リピータコマンド
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void ProductMasterList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		// カート投入バリエーション一覧で選択されたバリエーションIDをプロパティにセット
		this.VariationId = ((HiddenField)e.Item.FindControl("hfVariationId")).Value;

		// 商品一覧で選択された商品IDをプロパティにセット
		this.ProductId = ((HiddenField)e.Item.FindControl("hfProductId")).Value;

		// 注文数を1固定、複数バリエーション選択カート投入
		var cartAddProductCount = "1";

		ContentsLogModel contentsLog;
		var feature = new FeaturePageService().GetByFileName(Path.GetFileName(this.Request.Path));
		if (feature != null)
		{
			// コンテンツログ作成
			contentsLog = new ContentsLogModel
			{
				ContentsType = Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_FEATURE,
				ContentsId = feature.FeaturePageId.ToString(),
			};
		}
		else
		{
			contentsLog = null;
		}

		this.IsDisplayPopupAddFavorite = true;

		// カート投入処理
		switch (e.CommandName)
		{

			// カート投入処理
			case "CartAdd":
				AddCart(Constants.AddCartKbn.Normal, cartAddProductCount, Constants.KBN_REDIRECT_AFTER_ADDPRODUCT_CARTLIST, contentsLog);
				break;

			case "CartAddFixedPurchase":
				AddCart(Constants.AddCartKbn.FixedPurchase, cartAddProductCount, Constants.KBN_REDIRECT_AFTER_ADDPRODUCT_CARTLIST, contentsLog);
				break;

			case "CartAddGift":
				AddCart(Constants.AddCartKbn.GiftOrder, cartAddProductCount, Constants.KBN_REDIRECT_AFTER_ADDPRODUCT_CARTLIST, contentsLog);
				break;

			case "FavoriteAdd":
				AddToFavorite(this.ShopId, this.ProductId, this.CategoryId, Constants.VARIATION_FAVORITE_CORRESPONDENCE ? this.VariationId : string.Empty);
				break;

			// 通知登録関連処理
			case "ArrivalMail":
				// 入荷通知メール一覧画面へ
				this.Response.Redirect(
					CreateRegistUserProductArrivalMailUrl(
						this.ShopId,
						this.ProductId,
						this.VariationId,
						((HiddenField)e.Item.FindControl("hfArrivalMailKbn")).Value,
						this.RawUrl));
				break;

			case "SmartArrivalMail":
				ViewArrivalMailForm(e);
				break;
		}
	}

	/// <summary>
	/// お気に入りデータ設定
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>お気に入り登録者数</returns>
	protected int SetFavoriteDataForDisplay(string productId, string variationId)
	{
		var favorite = new FavoriteModel
		{
			ShopId = this.ShopId,
			ProductId = productId,
			VariationId = Constants.VARIATION_FAVORITE_CORRESPONDENCE ? variationId : string.Empty,
		};
		var result = Constants.VARIATION_FAVORITE_CORRESPONDENCE
			? new FavoriteService().GetFavoriteCountByProduct(favorite)
			: new FavoriteService().GetFavoriteByProduct(favorite);
		return result;
	}

	/// <summary>
	/// 対象の商品バリエーションを含むセットプロモーションを取得
	/// </summary>
	/// <param name="variationInfo">商品バリエーション情報</param>
	/// <returns>該当商品バリエーションを含むセットプロモーション</returns>
	protected SetPromotionModel[] GetSetPromotionByVariation(Dictionary<string, object> variationInfo)
	{
		var setPromotionList = GetSetPromotionByVariation(
			(string)variationInfo[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID],
			(string)variationInfo[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID],
			this.ProductVariationList);
		return setPromotionList;
	}

	/// <summary>ページャーHTML</summary>
	protected string PagerHtml
	{
		get { return (string)this.ViewState["PagerHtml"]; }
		private set { this.ViewState["PagerHtml"] = value; }
	}
	/// <summary>並び順</summary>
	protected string[] Sort { get; private set; }
	/// <summary>ヘッダーバナー代替テキスト</summary>
	protected string AltText { get; private set; }
	/// <summary>特集ページモデル</summary>
	protected FeaturePageModel FeaturePageModel { get; set; }
	/// <summary>公開開始日</summary>
	protected DateTime? PublishDateFrom { get; set; }
	/// <summary>公開終了日</summary>
	protected DateTime? PublishDateTo { get; set; }
	/// <summary>記事更新日</summary>
	protected DateTime? ChangedDate { get; set; }
	/// <summary>公開終了日が設定されているか？</summary>
	protected bool IsPublishDateTo { get; set; }
	/// <summary>商品マスターリスト</summary>
	private DataView ProductMasterList { get; set; }
	/// <summary>商品バリエーションマスタリスト</summary>
	private DataView ProductVariationList { get; set; }
	/// <summary>商品表示開始位置</summary>
	private int BgnNum { get; set; }
	/// <summary>商品表示終了位置</summary>
	private int EndNum { get; set; }
	/// <summary>特集ページコンテンツモデルリスト</summary>
	private List<FeaturePageContentsModel> FeaturePageContents { get; set; }
	/// <summary>特集ページ商品一覧リスト</summary>
	private List<FeaturePageProductGroup> FeaturePageProductGroup
	{
		get { return (List<FeaturePageProductGroup>)this.ViewState["FeaturePageProductGroup"]; }
		set { this.ViewState["FeaturePageProductGroup"] = value; }
	}
	/// <summary>選択されたバリエーションリスト</summary>
	private Dictionary<string, string> SelectVariationList
	{
		get { return (Dictionary<string, string>)this.ViewState["SelectVariationLlist"]; }
		set { this.ViewState["SelectVariationLlist"] = value; }
	}
}

/// <summary>
/// 特集ページ商品一覧
/// </summary>
[Serializable]
public class FeaturePageProductGroup
{
	/// <summary>商品一覧タイトル</summary>
	public string Title { get; set; }
	/// <summary>もっと見るリンク</summary>
	public string ViewMore { get; set; }
	/// <summary>特集ページ商品情報</summary>
	public List<FeaturePageProductGroupItem> ProductGroupItems { get; set; }
	/// <summary>商品情報</summary>
	public ProductModel ProductList { get; set; }
}

/// <summary>
/// 特集ページ商品情報
/// </summary>
[Serializable]
public class FeaturePageProductGroupItem
{
	/// <summary>商品グループID</summary>
	public string ProductGroupId { get; set; }
	/// <summary>商品マスタモデル</summary>
	public ProductModel Product { get; set; }
	/// <summary>商品情報</summary>
	public Dictionary<string, object> ProductInfo { get; set; }
}
