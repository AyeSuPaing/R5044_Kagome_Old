/*
=========================================================================================================
  Module      : コーディネート詳細画面(CoordinateDetail.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Coordinate;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Util.Security;
using w2.Common.Web;
using w2.Domain.ContentsLog;
using w2.Domain.Coordinate;
using w2.Domain.Product;
using w2.Domain.Staff;
using w2.Domain.User;

public partial class Form_Coordinate_CoordinateDetail : CoordinatePage
{
	/// <summary>リピートプラスONEリダイレクト必須判定</summary>
	public override bool RepeatPlusOneNeedsRedirect { get { return Constants.REPEATPLUSONE_OPTION_ENABLED; } }
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Http; } }	// Httpアクセス

	#region ラップ済コントロール宣言
	WrappedLinkButton WrLikeButton { get { return GetWrappedControl<WrappedLinkButton>("lbLike"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!this.IsPostBack)
		{
			// コーディネート情報をセット
			SetCoordinatePage();

			// コーディネート商品リストをセット
			SetCoordinateProductList();

			// コンテンツログ出力
			ContentsLogUtility.InsertPageViewContentsLog(Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_COORDINATE, this.CoordinateId, this.IsSmartPhone);

			// 画面全体をデータバインド
			this.DataBind();
		}
	}

	/// <summary>
	/// コーディネート情報をセット
	/// </summary>
	protected void SetCoordinatePage()
	{
		// プレビューモードチェック
		if (this.IsPreview)
		{
			// プレビューの場合、キャッシュを残さない
			this.Response.AddHeader("Cache-Control", "private, no-store, no-cache, must-revalidate");
			this.Response.AddHeader("Pragma", "no-cache");
			this.Response.Cache.SetAllowResponseInBrowserHistory(false);
		}

		var service = new CoordinateService();
		this.Coordinate = (this.IsPreview)
			? CoordinateCommon.GetPreview(this.ShopId, this.CoordinateId)
			: service.GetWithChilds(this.CoordinateId, this.ShopId);

		if (this.Coordinate == null)
		{
			// 商品が見つからない場合はエラーページへ
			this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_COORDINATE_UNDISP);
			this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
		else
		{
			// 公開状態でなく、プレビューでなければエラーページへ
			if ((IsPublic(this.Coordinate) == false) && (this.IsPreview == false))
			{
				this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_COORDINATE_UNDISP);
				this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				// 翻訳情報設定
				this.Coordinate = NameTranslationCommon.SetCoordinateTranslationDataWithChild(this.Coordinate);
				this.Coordinate.CategoryList = NameTranslationCommon.SetCoordinateCategoryTranslationData(this.Coordinate.CategoryList.ToArray()).ToList();
			}

			this.SelectedProductList = this.Coordinate.ProductList;
			this.CoordinateImages = new List<string>();

			// 画像処理
			var targetPath = (this.IsPreview)
				? Path.Combine(
					Constants.PHYSICALDIRPATH_FRONT_PC,
					Constants.PATH_TEMP,
					this.OperatorId,
					Constants.PATH_TEMP_COORDINATE,
					this.CoordinateId)
				: Path.Combine(
					Constants.PHYSICALDIRPATH_FRONT_PC,
					Constants.PATH_COORDINATE,
					this.CoordinateId);

			if (Directory.Exists(targetPath))
			{
				var target = Directory.EnumerateFileSystemEntries(targetPath);
				var enumerable = target as string[] ?? target.ToArray();
				for (var i = 1; i <= enumerable.Length; i++)
				{
					this.CoordinateImages.Add(CreateCoordinateImageUrl(this.CoordinateId, i, this.OperatorId));
				}
				if (enumerable.Any() == false) this.CoordinateImages.Add(CreateCoordinateImageUrl(this.CoordinateId, 1, this.OperatorId));
			}
			else
			{
				this.CoordinateImages.Add(CreateCoordinateImageUrl(this.CoordinateId, 1, this.OperatorId));
			}

			// スタッフ画像処理
			this.StaffImagePath = GetStaffImagePath(this.Coordinate.StaffId);

			// いいね数
			var userService = new UserService();
			this.LikeCount = userService.GetUserActivityCountForManager(Constants.FLG_USERACTIVITY_MASTER_KBN_COORDINATE_LIKE, this.Coordinate.CoordinateId);

			// 同じスタッフと同じ店舗のコーディネートリスト
			this.AllCoordinateList = service.GetAll().ToList();

			this.CoordinateListOfSameStaff =
				this.AllCoordinateList.Where(
					coordinate => (coordinate.StaffId == this.Coordinate.StaffId)
						&& (coordinate.CoordinateId != this.Coordinate.CoordinateId)
						&& (IsPublic(coordinate))).Take(this.MaxDispCountOfSameStaff).ToList();
			if (this.CoordinateListOfSameStaff.Count == 0) StaffList.Visible = false;

			this.CoordinateListOfSameRealShop =
				this.AllCoordinateList.Where(
					coordinate => (coordinate.RealShopId == this.Coordinate.RealShopId)
						&& (coordinate.CoordinateId != this.Coordinate.CoordinateId)
						&& (IsPublic(coordinate))).Take(this.MaxDispCountOfSameRealShop).ToList();
			if (this.CoordinateListOfSameRealShop.Count == 0) RealShopList.Visible = false;

			// 同じ商品のコーディネートリスト
			this.CoordinateListOfSameProduct = new List<CoordinateModel>();
			foreach (var product in this.Coordinate.ProductList)
			{
				var coordinateList = service.GetCoordinateListByProductId(product.ProductId, this.ShopId);
				if (coordinateList != null)
				{
					foreach (var coordinate in coordinateList.Where(
						coordinate => coordinate.CoordinateId != this.Coordinate.CoordinateId
						&& (IsPublic(coordinate))))
					{
						var tempList = this.CoordinateListOfSameProduct.Where(
							c => c.CoordinateId == coordinate.CoordinateId).ToList();
						if (tempList.Count == 0) this.CoordinateListOfSameProduct.Add(coordinate);
					}
				}
			}
			this.CoordinateListOfSameProduct =
				this.CoordinateListOfSameProduct.Take(this.MaxDispCountOfSameProduct).ToList();
			if (this.CoordinateListOfSameProduct.Count == 0) ProductList.Visible = false;

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				this.CoordinateListOfSameStaff =
					NameTranslationCommon.SetCoordinateTranslationDataWithChild(this.CoordinateListOfSameStaff.ToArray()).ToList();
				this.CoordinateListOfSameRealShop =
					NameTranslationCommon.SetCoordinateTranslationDataWithChild(this.CoordinateListOfSameRealShop.ToArray()).ToList();
				this.CoordinateListOfSameProduct =
					NameTranslationCommon.SetCoordinateTranslationDataWithChild(this.CoordinateListOfSameProduct.ToArray()).ToList();
			}
		}
	}

	/// <summary>
	/// 同じスタッフのコーディネートを追加
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void AddMaxDispCountOfSameStaff_Click(object sender, System.EventArgs e)
	{
		this.MaxDispCountOfSameStaff += this.AddDispCountOfSameStaff;
		this.CoordinateListOfSameStaff = this.AllCoordinateList
			.Where(
				coordinate => (coordinate.StaffId == this.Coordinate.StaffId)
					&& (coordinate.CoordinateId != this.Coordinate.CoordinateId)
					&& IsPublic(coordinate)).Take(this.MaxDispCountOfSameStaff)
			.ToList();
		DataBindByClass(this.Page, "StaffList");
	}

	/// <summary>
	/// 同じ店舗のコーディネートを追加
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void AddMaxDispCountOfSameRealShop_Click(object sender, System.EventArgs e)
	{
		this.MaxDispCountOfSameRealShop += this.AddDispCountOfSameRealShop;
		this.CoordinateListOfSameRealShop = this.AllCoordinateList
			.Where(
				coordinate => (coordinate.RealShopId == this.Coordinate.RealShopId)
					&& (coordinate.CoordinateId != this.Coordinate.CoordinateId)
					&& IsPublic(coordinate)).Take(this.MaxDispCountOfSameRealShop)
			.ToList();
		DataBindByClass(this.Page, "RealShopList");
	}

	/// <summary>
	/// 同じ商品のコーディネートを追加
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void AddMaxDispCountOfSameProduct_Click(object sender, System.EventArgs e)
	{
		this.MaxDispCountOfSameProduct += this.AddDispCountOfSameProduct;
		// 同じ商品のコーディネートリスト
		this.CoordinateListOfSameProduct = new List<CoordinateModel>();
		foreach (var product in this.Coordinate.ProductList)
		{
			var coordinateList = new CoordinateService().GetCoordinateListByProductId(product.ProductId, this.ShopId)
				.Where(
					coordinate => coordinate.CoordinateId != this.Coordinate.CoordinateId
					&& IsPublic(coordinate));
			foreach (var coordinate in coordinateList)
			{
				var tempList = this.CoordinateListOfSameProduct.Where(
					c => c.CoordinateId == coordinate.CoordinateId).ToList();
				if (tempList.Count == 0) this.CoordinateListOfSameProduct.Add(coordinate);
			}
		}
		this.CoordinateListOfSameProduct =
			this.CoordinateListOfSameProduct.Take(this.MaxDispCountOfSameProduct).ToList();

		this.CoordinateListOfSameProduct = this.CoordinateListOfSameProduct.Take(this.MaxDispCountOfSameProduct).ToList();
		DataBindByClass(this.Page, "ProductList");
	}

	/// <summary>
	/// コーディネート商品リストを設定
	/// </summary>
	/// <param name="changeProduct">変更した商品</param>
	protected void SetCoordinateProductList(ProductModel changeProduct = null)
	{
		this.CoordinateProductList = new List<DataRowView>();
		this.ErrorMessage = null;
		this.AlertMessage = null;

		foreach (var product in this.SelectedProductList)
		{
			var drv = CheckProduct(product, changeProduct);
			if (drv != null) this.CoordinateProductList.Add(drv);
		}
	}

	/// <summary>
	/// 商品をチェック
	/// </summary>
	/// <param name="selectedProduct">選択されている商品</param>
	/// <param name="changeProduct">変更された商品</param>
	/// <returns>商品（異常がある場合はNullで表示しない）</returns>
	protected DataRowView CheckProduct(ProductModel selectedProduct, ProductModel changeProduct = null)
	{
		var productId = selectedProduct.ProductId;
		var variationId = selectedProduct.VariationId;
		var productInfo = ProductCommon.GetProductVariationInfo(this.ShopId, productId, variationId, this.MemberRankId);

		// 商品が見つからない場合はNullを返す
		if (productInfo.Count == 0) return null;

		var product = productInfo[0];

		// 表示期間チェック（非表示の場合Nullを返す）
		if ((int)product["disp_flg"] == 0) return null;

		// 閲覧・購入可能会員ランクチェック
		var orderMemberRankdId = MemberRankOptionUtility.GetMemberRankId(this.LoginUserId);
		var displayMemberRank = (string)product[Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK];
		if (MemberRankOptionUtility.CheckMemberRankPermission(orderMemberRankdId, displayMemberRank) == false) return null;

		// 定期購入のみの商品かチェック
		if (((string)product[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY)) return null;

		if (this.IsPreview == false)
		{
			//// 商品表示履歴追加
			// 商品履歴オブジェクトが生成されている場合
			if (this.Session[Constants.SESSION_KEY_PRODUCTHISTORY_LIST] == null)
			{
				this.Session[Constants.SESSION_KEY_PRODUCTHISTORY_LIST] = new ProductHistoryObject();
			}
			ProductHistoryObject ph = (ProductHistoryObject)this.Session[Constants.SESSION_KEY_PRODUCTHISTORY_LIST];
			ph.Add(product);
		}

		// 商品がドロップダウンリストで選択されている場合
		if (IsProductSelected(selectedProduct))
		{
			// 購入会員ランク・販売期間・在庫有無チェック
			var errorMessage = CheckBuyableMemberRank(
				product,
				OrderCommon.CheckProductStockBuyable(product, 1));
			// バリエーション選択状況に応じて「商品名(＋バリエーション名)」を置換
			var productName = ProductCommon.CreateProductJointName(product);

			// 購入可能判定
			if ((string.IsNullOrEmpty(errorMessage) == false))
			{
				this.ErrorMessage += errorMessage.Replace("@@ 1 @@", productName);
				this.ErrorMessage += "\r\n";
			}
		}

		// Alert message limited payment
		if (string.IsNullOrEmpty(product[Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS].ToString()) == false)
		{
			var limitedPaymentList = OrderCommon.GetLimitedPayments(
					StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_SHOP_ID]),
					product[Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS].ToString().Split(','))
				.Select(payment => payment.PaymentName).ToArray();

			this.AlertMessage += (limitedPaymentList.Any() == false)
				? String.Empty
				: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCTDETAIL_LIMITED_PAYMENT_DISPLAYED)
					.Replace("@@ 1 @@", String.Join(", ", limitedPaymentList));
			this.AlertMessage += "\r\n";
		}

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// 翻訳情報設定
			var productTranslationSettings = NameTranslationCommon.GetProductAndVariationTranslationSettingsByProductId(
				(string)product[Constants.FIELD_PRODUCT_PRODUCT_ID],
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId);

			var stockMessageTranslationSettings = (product[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF] != DBNull.Value)
				? NameTranslationCommon.GetProductStockMessageTranslationSettings(
					(string)product[Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID],
					RegionManager.GetInstance().Region.LanguageCode,
					RegionManager.GetInstance().Region.LanguageLocaleId)
				: null;
			product = (DataRowView)NameTranslationCommon.TranslateProductAndVariationData(product, productTranslationSettings);
			product = NameTranslationCommon.SetProductStockMessageTranslationData(product, stockMessageTranslationSettings);
		}

		return product;
	}

	/// <summary>
	/// リピーターにセレクトチェンジイベントを付与
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected virtual void RepeaterItemCreated(object sender, RepeaterItemEventArgs e)
	{
		DropDownList selectedVariationNameList1 = (DropDownList)e.Item.FindControl("ddlVariationSelect1");
		selectedVariationNameList1.SelectedIndexChanged += ddlVariationId_SelectedIndexChanged;
		DropDownList selectedVariationNameList2 = (DropDownList)e.Item.FindControl("ddlVariationSelect2");
		selectedVariationNameList2.SelectedIndexChanged += ddlVariationId_SelectedIndexChanged;
	}

	/// <summary>
	/// 商品バリエーション選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlVariationId_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 選択リストを変更
		var changeProduct = ChangeSelectedProductList((DropDownList)sender);
		// コーディネート商品リストを作成
		SetCoordinateProductList(changeProduct);
		// バリエーション更新対象をデータバインド
		DataBindByClass(this.Page, "ChangesByVariation");
	}

	/// <summary>
	/// 選択されている商品を変更
	/// </summary>
	/// <param name="ddl">ドロップダウンリスト</param>
	/// <returns>変更された商品</returns>
	protected ProductModel ChangeSelectedProductList(DropDownList ddl)
	{
		var productId = ddl.SelectedItem.Value.Split(',')[0];
		var selectedProduct = this.SelectedProductList.Where(p => p.ProductId == productId).ToList()[0];
		var productInfo = ProductCommon.GetProductInfo(this.ShopId, productId, this.MemberRankId, this.UserFixedPurchaseMemberFlg);

		if (selectedProduct.HasProductVariation)
		{
			switch (ddl.ID)
			{
				case "ddlVariationSelect1":
					selectedProduct.VariationName1 = ddl.SelectedItem.Text;
					selectedProduct.SelectedIndex1 = ddl.SelectedIndex;
					break;

				case "ddlVariationSelect2":
					selectedProduct.VariationName2 = ddl.SelectedItem.Text;
					selectedProduct.SelectedIndex2 = ddl.SelectedIndex;
					break;
			}

			var variationProducts = productInfo.Cast<DataRowView>().Select(drv => new ProductModel(drv))
				.Where(p => (string.IsNullOrEmpty(p.VariationName2) == false)
					? (p.VariationName1 == selectedProduct.VariationName1)
					&& (p.VariationName2 == selectedProduct.VariationName2)
					: (p.VariationName1 == selectedProduct.VariationName1)).ToArray();
			if (variationProducts.Any()) selectedProduct.VariationId = variationProducts[0].VariationId;
		}
		else
		{
			selectedProduct.VariationName1 = ddl.SelectedItem.Text;
			selectedProduct.SelectedIndex1 = ddl.SelectedIndex;
			selectedProduct.VariationId = productId;
		}

		this.SelectedProductList.Where(p => p.ProductId == productId).ToList()[0] = selectedProduct;
		return selectedProduct;
	}

	/// <summary>
	/// 商品が1つでも選択されているか
	/// </summary>
	/// <returns>商品が選択されているか</returns>
	protected bool IsOneSelected()
	{
		foreach (var product in this.SelectedProductList)
		{
			if (IsProductSelected(product)) return true;
		}
		return false;
	}

	/// <summary>
	/// 商品が選択されているか
	/// </summary>
	/// <param name="product">商品</param>
	/// <returns>商品が選択されているか</returns>
	protected bool IsProductSelected(ProductModel product)
	{
		// バリエーションなしの場合
		if ((product.HasProductVariation == false) && (product.SelectedIndex1 != 0))
		{
			return true;
		}

		// バリエーションありの場合（2軸あるかによって分岐）
		if ((string.IsNullOrEmpty(product.VariationName2) == false)
			? (product.SelectedIndex1 != 0) && (product.SelectedIndex2 != 0)
			: (product.SelectedIndex1 != 0))
		{
			return true;
		}

		return false;
	}

	/// <summary>
	/// 2軸あるか
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <returns>2軸あるか</returns>
	protected bool HasTwoAxes(string productId)
	{
		foreach (DataRowView drvProduct in ProductCommon.GetProductInfo(this.ShopId, productId, this.MemberRankId, this.UserFixedPurchaseMemberFlg))
		{
			if (string.IsNullOrEmpty((string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2])) return false;
		}
		return true;
	}

	/// <summary>
	/// ドロップダウンリストを取得
	/// </summary>
	/// <param name="obj">オブジェクト</param>
	/// <param name="type">ドロップダウンタイプ</param>
	/// <returns>ドロップダウンリスト</returns>
	protected ListItemCollection GetDropDownList(object obj, int type)
	{
		var list = new ListItemCollection();
		var productId = (string)GetKeyValue(obj, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID);
		var productInfo = ProductCommon.GetProductInfo(this.ShopId, productId, this.MemberRankId, this.UserFixedPurchaseMemberFlg);

		list.Add(new ListItem(ReplaceTag("@@DispText.variation_name_list.unselected@@"), productId));
		if (HasVariation(productInfo[0]) == false)
		{
			var selected = ReplaceTag("@@DispText.variation_name_list.selected@@");
			list.Add(new ListItem(selected, productId + "," + selected));
			return list;
		}

		var variationName = (type == 1)
			? Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1
			: Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2;

		foreach (DataRowView drvProduct in productInfo)
		{
			var name = (string)drvProduct[variationName];
			var variation = new ListItem(name, productId + "," + name);
			if (list.Contains(variation) == false)
			{
				list.Add(variation);
			}
		}
		return list;
	}

	/// <summary>
	/// 選択されている索引値を取得
	/// </summary>
	/// <param name="obj">オブジェクト</param>
	/// <param name="type">タイプ</param>
	/// <returns>索引値</returns>
	protected int GetSelectedIndex(object obj, int type)
	{
		var productId = (string)GetKeyValue(obj, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID);

		foreach (var product in this.SelectedProductList)
		{
			if (product.ProductId == productId && type == 1)
			{
				return product.SelectedIndex1;
			}
			if (product.ProductId == productId && type == 2)
			{
				return product.SelectedIndex2;
			}
		}

		return 0;
	}

	/// <summary>
	/// いいねボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbLikeBtn_Click(object sender, System.EventArgs e)
	{
		if (string.IsNullOrEmpty(this.LoginUserId))
		{
			// 店IDとコーディネートIDと現在時刻を繋げたものを暗号化
			var rcAuthenticationKey
				= new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
			var authenticationKey = rcAuthenticationKey.Encrypt(this.ShopId + ":" + this.Coordinate.CoordinateId + ":" + DateTime.Now.ToString("yyyyMMddhhmmss"));

			this.Session[Constants.SESSION_KEY_LIKE_TOKEN] = authenticationKey;

			var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_LIKE_LIST).AddParam(
				Constants.REQUEST_KEY_LIKE_TOKEN,
				authenticationKey);
			var url = urlCreator.CreateUrl();
			this.Response.Redirect(url);
		}

		InsertActivityOrDeleteActivity(Constants.FLG_USERACTIVITY_MASTER_KBN_COORDINATE_LIKE);

		// いいねの情報を更新
		this.LikeCount = new UserService().GetUserActivityCountForManager(Constants.FLG_USERACTIVITY_MASTER_KBN_COORDINATE_LIKE, this.Coordinate.CoordinateId);
		this.WrLikeButton.DataBind();
	}

	/// <summary>
	/// フォローボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbFollowBtn_Click(object sender, System.EventArgs e)
	{
		if (string.IsNullOrEmpty(this.LoginUserId))
		{
			// 店IDとスタッフIDと現在時刻を繋げたものを暗号化
			var rcAuthenticationKey
				= new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
			var authenticationKey = rcAuthenticationKey.Encrypt(this.ShopId + ":" + this.Coordinate.StaffId + ":" + DateTime.Now.ToString("yyyyMMddhhmmss"));

			this.Session[Constants.SESSION_KEY_FOLLOW_TOKEN] = authenticationKey;

			var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_FOLLOW_LIST).AddParam(
				Constants.REQUEST_KEY_FOLLOW_TOKEN,
				authenticationKey);
			var url = urlCreator.CreateUrl();
			this.Response.Redirect(url);
		}

		InsertActivityOrDeleteActivity(Constants.FLG_USERACTIVITY_MASTER_KBN_COORDINATE_FOLLOW);
	}

	/// <summary>
	/// アクティビティを登録か削除する
	/// </summary>
	/// <param name="activityKbn">アクティビティ区分</param>
	protected void InsertActivityOrDeleteActivity(string activityKbn)
	{
		var service = new UserService();
		var model = new UserActivityModel
		{
			UserId = this.LoginUserId,
			MasterKbn = activityKbn,
			MasterId = (activityKbn == Constants.FLG_USERACTIVITY_MASTER_KBN_COORDINATE_LIKE)
				? this.Coordinate.CoordinateId
				: this.Coordinate.StaffId
		};

		if (activityKbn == Constants.FLG_USERACTIVITY_MASTER_KBN_COORDINATE_FOLLOW)
		{
			if (new StaffService().Get(model.MasterId) == null) return;
		}

		var isRegisterd = (service.GetUserActivity(model.UserId, model.MasterKbn, model.MasterId) != null);
		if (isRegisterd == false)
		{
			service.InsertUserActivity(model);
		}
		else
		{
			service.DeleteUserActivity(model.UserId, model.MasterKbn, model.MasterId);
		}
	}

	/// <summary>
	/// まとめてカートへ入れるボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCartAdd_Click(object sender, System.EventArgs e)
	{
		if (string.IsNullOrEmpty(AllProductAddCart()) == false)
		{
			SetCoordinateProductList();
			DataBindByClass(this.Page, "ChangesByVariation");
		}
	}

	/// <summary>
	/// 全ての商品のカート投入処理
	/// </summary>
	/// <returns></returns>
	protected string AllProductAddCart()
	{
		var errorMessage = "";
		this.ProductOptionSettingList = new ProductOptionSettingList();
		var productIds = new List<string>();
		var variationIds = new List<string>();
		var productCounts = new List<string>();
		var canUseCartListLp = CanUseCartListLp();

		foreach (var product in this.SelectedProductList)
		{
			if (IsProductSelected(product))
			{
				// 商品情報取得
				var dvProduct = ProductCommon.GetProductVariationInfo(this.ShopId, product.ProductId, product.VariationId, this.MemberRankId);
				if (dvProduct.Count == 0)
				{
					this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
					this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}

				// 商品在庫エラーは詳細画面に文言表示
				OrderErrorcode productError = OrderCommon.CheckProductStatus(dvProduct[0], 1, Constants.AddCartKbn.Normal, this.LoginUserId);
				if ((productError == OrderErrorcode.ProductNoStockBeforeCart) || (productError == OrderErrorcode.MaxSellQuantityError))
				{
					errorMessage += OrderCommon.GetErrorMessage(
						productError,
						ProductPage.CreateProductJointName(dvProduct[0]),
						MemberRankOptionUtility.GetMemberRankName((string)dvProduct[0][Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK]));
					errorMessage += "\n";
				}

				if (errorMessage == "")
				{
					if (productError != OrderErrorcode.NoError)
					{
						switch (productError)
						{
							case OrderErrorcode.SellMemberRankError:
								errorMessage = OrderCommon.GetErrorMessage(productError,
									ProductPage.CreateProductJointName(dvProduct[0]),
									MemberRankOptionUtility.GetMemberRankName((string)dvProduct[0][Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK]));
								break;

							default:
								errorMessage = OrderCommon.GetErrorMessage(productError, ProductPage.CreateProductJointName(dvProduct[0]));
								break;
						}

						this.Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
						this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
					}

					// コンテンツログ作成
					var contentsLog = new ContentsLogModel
					{
						ContentsType = Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_COORDINATE,
						ContentsId = this.CoordinateId,
					};

					if (canUseCartListLp == false)
					{
						// カート投入（商品付帯情報は何も選択されていない状態でカート投入）
						var cartList = GetCartObjectList();
						if (SessionManager.OrderCombineCartList == null)
						{
							cartList.AddProduct(
								dvProduct[0],
								Constants.AddCartKbn.Normal,
								StringUtility.ToEmpty(dvProduct[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]),
								1,
								this.ProductOptionSettingList,
								contentsLog);

							cartList.CartListShippingMethodUserUnSelected();
						}
						else
						{
							SessionManager.OrderCombineBeforeCartList.AddProduct(
								dvProduct[0],
								Constants.AddCartKbn.Normal,
								StringUtility.ToEmpty(dvProduct[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]),
								1,
								this.ProductOptionSettingList,
								contentsLog);
						}
					}
					else
					{
						var productId = StringUtility.ToEmpty(product.ProductId);
						var variationId = StringUtility.ToEmpty(product.VariationId);
						productIds.Add(productId);
						variationIds.Add(variationId);
						productCounts.Add("1");
					}
				}
			}
		}
		// カート一覧画面へ
		if (string.IsNullOrEmpty(errorMessage))
		{
			var pageCartList = Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST;

			if (canUseCartListLp)
			{
				var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST_LP)
					.AddParam(Constants.REQUEST_KEY_LPCART_PRODUCT_ID, string.Join(",", productIds.Select(productId => productId)))
					.AddParam(Constants.REQUEST_KEY_LPCART_VARIATION_ID, string.Join(",", variationIds.Select(variationId => variationId)))
					.AddParam(Constants.REQUEST_KEY_LPCART_ADD_CART_KBN, Constants.FLG_ADD_CART_KBN_NORMAL)
					.AddParam(Constants.REQUEST_KEY_LPCART_PRODUCT_COUNT, string.Join(",", productCounts.Select(productCount => productCount)))
					.CreateUrl();

				SessionManager.IsOnlyAddCartFirstTime = true;
				pageCartList = urlCreator;
			}

			Response.Redirect(pageCartList);
		}

		return errorMessage;
	}

	/// <summary>
	/// 商品詳細URL作成
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="isVariation"></param>
	/// <returns>商品詳細URL</returns>
	protected string CreateProductDetailUrl(object objProduct, bool isVariation = false)
	{
		var variationId = isVariation ? (string)GetKeyValueToNull(objProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) : "";
		return CreateProductDetailUrl(objProduct, variationId);
	}

	/// <summary>
	/// 商品詳細URL作成
	/// </summary>
	/// <param name="objProduct"></param>
	/// <param name="strVariationId">バリエーションID</param>
	/// <returns>商品詳細URL</returns>
	protected string CreateProductDetailUrl(object objProduct, string strVariationId)
	{
		var brandId = string.IsNullOrEmpty(this.BrandId) ? (string)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_BRAND_ID1) : this.BrandId;
		return ProductCommon.CreateProductDetailUrl(
			objProduct,
			strVariationId,
			"",
			brandId,
			"",
			ProductBrandUtility.GetProductBrandName(brandId));
	}

	/// <summary>
	/// いいね画像作成
	/// </summary>
	/// <returns>いいね画像URL</returns>
	protected string CreateLikeImage()
	{
		var path = Path.Combine(Constants.PATH_ROOT_FRONT_PC, Constants.PATH_COORDINATE);
		var isRegister = (new UserService().GetUserActivity(
			this.LoginUserId,
			Constants.FLG_USERACTIVITY_MASTER_KBN_COORDINATE_LIKE,
			this.Coordinate.CoordinateId) != null);

		path = (isRegister)
			? Path.Combine(path, "like_after.png")
			: Path.Combine(path, "like_before.png");

		return path;
	}

	/// <summary>
	/// 更新日
	/// </summary>
	/// <param name="date">日付</param>
	/// <returns>更新日</returns>
	protected string DisplayDate(DateTime? date)
	{
		var result = "";
		
		if (date.HasValue)
		{
			result = DateTimeUtility.ToStringFromRegion(date, DateTimeUtility.FormatType.LongDate1Letter);
		}
		return result;
	}

	/// <summary>
	/// 表示制限
	/// </summary>
	/// <param name="text">テキスト</param>
	/// <param name="length">テキストの長さ</param>
	/// <returns>制限されたテキスト</returns>
	public string DisplayLimit(string text, int length)
	{
		if (this.IsJapanese == false) length = length * 2;

		if (text.Length > length) return text.Substring(0, length) + "...";

		return text;
	}

	/// <summary>
	/// 公開しているか
	/// </summary>
	/// <param name="coordinate">コーディネート</param>
	/// <returns>公開しているか</returns>
	public bool IsPublic(CoordinateModel coordinate)
	{
		return ((coordinate.DisplayKbn == "PUBLIC") 
			&& ((coordinate.DisplayDate != null)
				&& ((DateTime)coordinate.DisplayDate < DateTime.Now)));
	}

	/// <summary>コーディネート商品リスト</summary>
	protected List<DataRowView> CoordinateProductList { get; set; }
	/// <summary>同じスタッフのコーディネートリスト</summary>
	public List<CoordinateModel> CoordinateListOfSameStaff
	{
		get { return (List<CoordinateModel>)this.ViewState["CoordinateListOfSameStaff"]; }
		private set { this.ViewState["CoordinateListOfSameStaff"] = value; }
	}
	/// <summary>同じ店舗のコーディネートリスト</summary>
	public List<CoordinateModel> CoordinateListOfSameRealShop
	{
		get { return (List<CoordinateModel>)this.ViewState["CoordinateListOfSameRealShop"]; }
		private set { this.ViewState["CoordinateListOfSameRealShop"] = value; }
	}
	/// <summary>同じ商品のコーディネートリスト</summary>
	public List<CoordinateModel> CoordinateListOfSameProduct
	{
		get { return (List<CoordinateModel>)this.ViewState["CoordinateListOfSameProduct"]; }
		private set { this.ViewState["CoordinateListOfSameProduct"] = value; }
	}
	/// <summary>商品付帯設定</summary>
	protected ProductOptionSettingList ProductOptionSettingList { get; set; }
	/// <summary>コーディネート画像</summary>
	protected List<string> CoordinateImages { get; set; }
	/// <summary>スタッフ画像パス</summary>
	protected string StaffImagePath { get; set; }
	/// <summary>スタッフを増やす表示数</summary>
	protected int AddDispCountOfSameStaff { get; set; }
	/// <summary>店舗を増やす表示数</summary>
	protected int AddDispCountOfSameRealShop { get; set; }
	/// <summary>商品を増やす表示数</summary>
	protected int AddDispCountOfSameProduct { get; set; }
	/// <summary>フォロー中か</summary>
	protected bool IsFollowing
	{
		get
		{
			return (new UserService().GetUserActivity(
				this.LoginUserId,
				Constants.FLG_USERACTIVITY_MASTER_KBN_COORDINATE_FOLLOW,
				this.Coordinate.StaffId) != null);
		}
	}
	/// <summary>プレビューか</summary>
	protected new bool IsPreview
	{
		get { return (string.IsNullOrEmpty(this.OperatorId) == false); }
	}
	/// <summary>いいね件数</summary>
	protected int LikeCount
	{
		get { return (int)this.ViewState["LikeCount"]; }
		private set { this.ViewState["LikeCount"] = value; }
	}
	/// <summary>コーディネート</summary>
	protected CoordinateModel Coordinate
	{
		get { return (CoordinateModel)this.ViewState["Coordinate"]; }
		private set { this.ViewState["Coordinate"] = value; }
	}
	/// <summary>全てのコーディネート</summary>
	protected List<CoordinateModel> AllCoordinateList
	{
		get { return (List<CoordinateModel>)this.ViewState["AllCoordinateList"]; }
		private set { this.ViewState["AllCoordinateList"] = value; }
	}
	/// <summary>同じスタッフの最大表示数</summary>
	protected int MaxDispCountOfSameStaff
	{
		get { return (int)this.ViewState["MaxDispCountOfSameStaff"]; }
		set { this.ViewState["MaxDispCountOfSameStaff"] = value; }
	}
	/// <summary>同じ店舗の最大表示数</summary>
	protected int MaxDispCountOfSameRealShop
	{
		get { return (int)this.ViewState["MaxDispCountOfSameRealShop"]; }
		set { this.ViewState["MaxDispCountOfSameRealShop"] = value; }
	}
	/// <summary>同じ商品の最大表示数</summary>
	protected int MaxDispCountOfSameProduct
	{
		get { return (int)this.ViewState["MaxDispCountOfSameProduct"]; }
		set { this.ViewState["MaxDispCountOfSameProduct"] = value; }
	}
	/// <summary>アラートメッセージ</summary>
	protected string AlertMessage
	{
		get { return (string)this.ViewState["AlertMessage"]; }
		private set { this.ViewState["AlertMessage"] = value; }
	}
	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage
	{
		get { return (string)this.ViewState["ErrorMessage"]; }
		private set { this.ViewState["ErrorMessage"] = value; }
	}
	/// <summary>選択されている商品リスト</summary>
	protected List<ProductModel> SelectedProductList
	{
		get { return (List<ProductModel>)this.ViewState["SelectedProductList"]; }
		set { this.ViewState["SelectedProductList"] = value; }
	}
	/// <summary>スタッフが有効か</summary>
	protected bool IsStaffValid
	{
		get { return (this.Coordinate.StaffValidFlg == Constants.FLG_STAFF_VALID_FLG_VALID); }
	}
}