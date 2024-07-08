/*
=========================================================================================================
  Module      : 特集ページカテゴリコントローラ(FeaturePageCategoryController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.ParamModels.FeaturePageCategory;
using w2.Cms.Manager.ViewModels.FeaturePageCategory;
using w2.Cms.Manager.WorkerServices;
using w2.Domain.FeaturePageCategory;
using Constants = w2.Cms.Manager.Codes.Constants;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// 特集ページカテゴリコントローラ
	/// </summary>
	public class FeaturePageCategoryController : BaseController
	{
		/// <summary>
		/// 一覧表示
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult List(FeaturePageCategoryListParamModel pm)
		{
			// 検索条件が入力されているか確認
			if ((string.IsNullOrEmpty(pm.SortKbn) == false)
				|| (string.IsNullOrEmpty(pm.SortCategoryId) == false))
			{
				// 検索条件あり
				var action = List(
					pm,
					pm.ParentCategoryId,
					pm.CategoryId,
					pm.CategoryName,
					pm.DisplayOrder,
					pm.CategoryOutline,
					pm.ValidFlg,
					"条件あり検索");
				this.TempData.FeaturePageCategoryListParamModel = pm;

				return action;
            }

            // カテゴリ詳細からの遷移かどうかを確認
            if ((this.TempData.FeaturePageCategoryListParamModel != null)
				&& this.TempData.FeaturePageCategoryListParamModel.IsCategoryDetail)
			{
				this.TempData.FeaturePageCategoryListParamModel.PagerNo = pm.PagerNo;
				var paramModel = this.TempData.FeaturePageCategoryListParamModel;

				var detail = List(
					paramModel,
					paramModel.ParentCategoryId,
					paramModel.CategoryId,
					paramModel.CategoryName,
					paramModel.DisplayOrder,
					paramModel.CategoryOutline,
					paramModel.ValidFlg,
					string.Empty);

				this.TempData.FeaturePageCategoryListParamModel = paramModel;
				return detail;
			}

			// 検索条件なし
			var vm = this.Service.CreateListVm(new FeaturePageCategoryListParamModel { PagerNo = pm.PagerNo });
			this.TempData.FeaturePageCategoryListParamModel =
				(this.FeaturePageRootCategoryRegistFlg)
					? new FeaturePageCategoryListParamModel()
					: pm;

			return View(vm);
		}

		/// <summary>
		/// 一覧表示
		/// </summary>
		/// <param name="pm">モデル</param>
		/// <param name="parentCategoryId">親カテゴリID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <param name="categoryName">カテゴリ名</param>
		/// <param name="displayOrder">表示順</param>
		/// <param name="categoryOutline">カテゴリ説明文</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="clickButton">クリックされたボタン</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[Button(ButtonName = "clickButton")]
		public ActionResult List(
			FeaturePageCategoryListParamModel pm,
			string parentCategoryId,
			string categoryId,
			string categoryName,
			string displayOrder,
			string categoryOutline,
			string validFlg,
			string clickButton)
		{
			ListViewModel vm;

			var parentCategory =
				((string.IsNullOrEmpty(parentCategoryId))
					|| (parentCategoryId == Constants.FLG_FEATURE_PAGE_CATEGORY_ROOT_CATEGORY))
					? string.Empty
					: parentCategoryId;
			var flg = ((validFlg == Constants.FLG_FEATURE_PAGE_CATEGORY_VALID_FLG_ON) || (validFlg == "on"))
				? Constants.FLG_FEATURE_PAGE_CATEGORY_VALID_FLG_ON
				: Constants.FLG_FEATURE_PAGE_CATEGORY_VALID_FLG_OFF;
			var setParentCateogry = (string.IsNullOrEmpty(parentCategory))
				? Constants.FLG_FEATURE_PAGE_CATEGORY_NOT_PARENT_CATEGORY
				: parentCategory;

			switch (clickButton)
			{
				case ("  登録する  "):
					var categoryRegist =
						(string.IsNullOrEmpty(this.TempData.FeaturePageCategoryListParamModel.ParentCategoryId))
							? Constants.FLG_FEATURE_PAGE_CATEGORY_ROOT_CATEGORY
							: this.TempData.FeaturePageCategoryListParamModel.ParentCategoryId;
					var htRegist = new Hashtable
					{
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_SHOP_ID, this.SessionWrapper.LoginShopId },
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_PARENT_CATEGORY_ID, categoryRegist },
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_ID, categoryId },
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_NAME, categoryName },
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_DISPLAY_ORDER, displayOrder },
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_OUTLINE, categoryOutline },
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_VALID_FLG, flg },
					};
					this.Service.Insert(htRegist);
					vm = this.Service.CreateListVm(new FeaturePageCategoryListParamModel { PagerNo = pm.PagerNo });
					return View(vm);

				case ("  更新する  "):
					var htUpdate = new Hashtable
					{
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_SHOP_ID, this.SessionWrapper.LoginShopId },
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_PARENT_CATEGORY_ID, this.ParentCategoryId },
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_ID, categoryId },
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_NAME, categoryName },
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_DISPLAY_ORDER, displayOrder },
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_OUTLINE, categoryOutline },
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_VALID_FLG, flg },
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_BEFORE_CATEGORY_ID, this.TempData.FeaturePageCategoryListParamModel.BeforeCategoryId },
					};
						this.Service.Update(htUpdate);
						vm = this.Service.CreateListVm(new FeaturePageCategoryListParamModel { PagerNo = pm.PagerNo });
						return View(vm);

				case ("  削除する  "):
					var htDelete = new Hashtable
					{
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_SHOP_ID, this.SessionWrapper.LoginShopId },
						{ Constants.FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_ID, categoryId },
					};
					this.Service.Delete(htDelete);
					vm = this.Service.CreateListVm(pm);
					return View(vm);

				case ("  最上位カテゴリ登録  "):
					this.FeaturePageRootCategoryRegistFlg = true;
					vm = this.Service.CreateListVm(new FeaturePageCategoryListParamModel { PagerNo = pm.PagerNo });
					SetParamModel(string.Empty, string.Empty, string.Empty, displayOrder, string.Empty, "0", false);
					break;

				case ("条件あり検索"):
						vm = this.Service.CreateListVm(pm);
						break;

				case ("  編集する  "):
						vm = this.Service.CreateListVm(pm);
						var categoryInput = categoryId.Substring(categoryId.Length - 3);
						SetViewBag(setParentCateogry, categoryInput, categoryName, displayOrder, categoryOutline, flg, parentCategory);
						SetParamModel(parentCategory, categoryInput, categoryName, displayOrder, categoryOutline, flg, true, categoryId);
						return View(vm);

				case ("  子カテゴリの登録  "):
						vm = this.Service.CreateListVm(pm);
						SetViewBag(categoryId, string.Empty, string.Empty, displayOrder, string.Empty, flg, categoryId);
						return View(vm);

				case ("  確認する  "):
					vm = this.Service.CreateListVm(pm);
					var categoryConfirm = (parentCategory + categoryId);
					if (this.TempData.FeaturePageCategoryListParamModel.IsCategoryDetail)
					{
						if (this.DuplicateCategoryFlg)
						{
							SetViewBag(setParentCateogry, categoryConfirm, categoryName, displayOrder, categoryOutline, flg);
						}
						else
						{
							SetViewBag(setParentCateogry, categoryId, categoryName, displayOrder, categoryOutline, flg, parentCategoryId);
						}
						return View(vm);
					}
					this.ParentCategoryId = string.IsNullOrEmpty(parentCategoryId) ? Constants.FLG_FEATURE_PAGE_CATEGORY_NOT_PARENT_CATEGORY : parentCategoryId;
					this.CategoryId = categoryConfirm;
					this.CategoryName = categoryName;
					this.DisplayOrder = displayOrder;
					this.CategoryOutline = categoryOutline;
					this.ValidFlg = flg;
					break;

				case ("  戻る  "):
					vm = this.Service.CreateListVm(pm);
					return View(vm);

				default:
					vm = this.Service.CreateListVm(new FeaturePageCategoryListParamModel { PagerNo = pm.PagerNo } );
					break;
			}

			var setReadParentCategory = (parentCategory == Constants.FLG_FEATURE_PAGE_CATEGORY_NOT_PARENT_CATEGORY)
				? string.Empty
				: parentCategory;

			SetViewBag(setParentCateogry, categoryId, categoryName, displayOrder, categoryOutline, flg, setReadParentCategory);
			return View(vm);
		}

		/// <summary>
		/// 編集ボタンクリック時処理
		/// </summary>
		/// <param name="parentCategoryId">親カテゴリID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <param name="categoryName">カテゴリ名</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="displayOrder">表示順</param>
		/// <param name="categoryOutline">カテゴリ説明文</param>
		public void FeaturePageCategoryUpdate(
			string parentCategoryId,
			string categoryId,
			string categoryName,
			string validFlg,
			string displayOrder,
			string categoryOutline)
		{
			var parentCategory =
				((string.IsNullOrEmpty(parentCategoryId))
					|| (parentCategoryId == Constants.FLG_FEATURE_PAGE_CATEGORY_ROOT_CATEGORY))
						? Constants.FLG_FEATURE_PAGE_CATEGORY_NOT_PARENT_CATEGORY
						: parentCategoryId;
			var category = string.Format("{0}{1}", parentCategory, categoryId);
			SetParamModel(parentCategory, category, categoryName, displayOrder, categoryOutline, validFlg, true);
		}

		/// <summary>
		/// カテゴリクリック時処理
		/// </summary>
		/// <param name="parentCategoryId">親カテゴリID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <param name="categoryName">カテゴリ名</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="displayOrder">表示順</param>
		/// <param name="categoryOutline">カテゴリ説明文</param>
		public void FeaturePageCategoryDetail(
			string parentCategoryId,
			string categoryId,
			string categoryName,
			string validFlg,
			string displayOrder,
			string categoryOutline)
		{
			this.ParentCategoryId = parentCategoryId;
			this.CategoryId = categoryId;
			this.CategoryName = categoryName;
			this.DisplayOrder = displayOrder;
			this.CategoryOutline = categoryOutline;
			this.ValidFlg = validFlg;
			SetParamModel(parentCategoryId, categoryId, categoryName, displayOrder, categoryOutline, validFlg, true);
		}

		/// <summary>
		/// カテゴリクリック時処理
		/// </summary>
		/// <param name="parentCategoryId">親カテゴリID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <param name="categoryName">カテゴリ名</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="displayOrder">表示順</param>
		/// <param name="categoryOutline">カテゴリ説明文</param>
		public void FeaturePageCategoryConfirm(
			string parentCategoryId,
			string categoryId,
			string categoryName,
			string validFlg,
			string displayOrder,
			string categoryOutline)
		{
			SetParamModel(parentCategoryId, categoryId, categoryName, displayOrder, categoryOutline, validFlg, true);
		}

		/// <summary>
		/// 最上位カテゴリ作成
		/// </summary>
		public void FeaturePageCategoryRegistRootCategory()
		{
			SetParamModel(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "0", false);
		}

		/// <summary>
		/// 子カテゴリ作成
		/// </summary>
		/// <param name="categoryId">カテゴリID</param>
		public void FeaturePageCategoryRegistChildCategory(string categoryId)
		{
			SetParamModel(categoryId, string.Empty, string.Empty, string.Empty, string.Empty, "0", true);
		}

		/// <summary>
		/// 戻るボタンクリック時処理
		/// </summary>
		public void FeaturePageCategoryPageBack()
		{
			SetViewBag(this.ParentCategoryId, this.CategoryId, this.CategoryName, this.DisplayOrder, this.CategoryOutline, this.ValidFlg);
			SetParamModel(this.ParentCategoryId, this.CategoryId, this.CategoryName, this.DisplayOrder, this.CategoryOutline, this.ValidFlg, true);
		}

		/// <summary>
		/// 戻るボタンクリック時処理（カテゴリ詳細へ戻る）
		/// </summary>
		public void FeaturePageCategoryPageBackDetail()
		{
			SetParamModel(this.ParentCategoryId, this.CategoryId, this.CategoryName, this.DisplayOrder, this.CategoryOutline, this.ValidFlg, false);
		}

		/// <summary>
		/// 戻るボタンクリック時処理（入力画面へ戻る）
		/// </summary>
		public void FeaturePageCategoryPageBackInput(
			string parentCategoryId,
			string categoryId,
			string categoryName,
			string displayOrder,
			string categoryOutline,
			string validFlg,
			string readParentCategory)
		{
			SetViewBag(parentCategoryId, categoryId, categoryName, displayOrder, categoryOutline, validFlg, readParentCategory);
			SetParamModel(parentCategoryId, categoryId, categoryName, displayOrder, categoryOutline, validFlg, true, readParentCategory);
		}

		/// <summary>
		/// カテゴリパラムモデルに値をセットする
		/// </summary>
		/// <param name="parentCategoryId">親カテゴリID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <param name="categoryName">カテゴリ名</param>
		/// <param name="displayOrder">表示順</param>
		/// <param name="categoryOutline">カテゴリ説明文</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="isCategoryDetail">カテゴリ詳細からの遷移か</param>
		/// <param name="beforeCategoryId">変更前カテゴリID</param>
		private void SetParamModel(
			string parentCategoryId,
			string categoryId,
			string categoryName,
			string displayOrder,
			string categoryOutline,
			string validFlg,
			bool isCategoryDetail,
			string beforeCategoryId = null)
		{
			this.TempData.FeaturePageCategoryListParamModel = new FeaturePageCategoryListParamModel
			{
				ParentCategoryId = (string.IsNullOrEmpty(parentCategoryId) == false) ? parentCategoryId : Constants.FLG_FEATURE_PAGE_CATEGORY_NOT_PARENT_CATEGORY,
				CategoryId = categoryId,
				CategoryName = categoryName,
				DisplayOrder = displayOrder,
				CategoryOutline = categoryOutline,
				ValidFlg = validFlg,
				IsCategoryDetail = isCategoryDetail,
				BeforeCategoryId = beforeCategoryId
			};
			this.IsCategoryDetail = isCategoryDetail;
			this.BeforeCategoryId = beforeCategoryId;
		}

		/// <summary>
		/// カテゴリパラムモデルに値をセットする
		/// </summary>
		/// <param name="parentCategoryId">親カテゴリID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <param name="categoryName">カテゴリ名</param>
		/// <param name="displayOrder">表示順</param>
		/// <param name="categoryOutline">カテゴリ説明文</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="readParentCategory">読み取り専用親カテゴリ</param>
		private void SetViewBag(
			string parentCategoryId,
			string categoryId,
			string categoryName,
			string displayOrder,
			string categoryOutline,
			string validFlg,
			string readParentCategory = "")
		{
			this.ViewBag.ParentCategoryId = parentCategoryId;
			this.ViewBag.CategoryId = categoryId;
			this.ViewBag.CategoryName = categoryName;
			this.ViewBag.DisplayOrder = displayOrder;
			this.ViewBag.CategoryOutline = categoryOutline;
			this.ViewBag.ValidFlg = validFlg;
			this.ViewBag.ReadParentCategoryId = readParentCategory;
		}

		/// <summary>
		/// バリデータ
		/// </summary>
		/// <returns>カテゴリ重複なし：True　あり：False</returns>
		public bool Validate(string parentCategoryId, string categoryId, string displayOrder)
		{
			this.DuplicateCategoryFlg = true;
			Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_DISPLAY_ORDER_ERRORMESSSAGE] = string.Empty;
			Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_CATEGORY_ERRORMESSSAGE] = string.Empty;

			// 空値チェック
			if (string.IsNullOrEmpty(displayOrder))
			{
				Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_DISPLAY_ORDER_ERRORMESSSAGE] = WebMessages.FeaturePageCategoryDisplayOrderEmptyError;
				this.DuplicateCategoryFlg = false;
			}

			// 桁数チェック
			if ((string.IsNullOrEmpty(displayOrder) == false) && ((displayOrder.Length < 8) && (displayOrder.Length >= 0) == false))
			{
				Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_DISPLAY_ORDER_ERRORMESSSAGE] = WebMessages.FeaturePageCategoryDisplayOrderDigitError;
				this.DuplicateCategoryFlg = false;
			}

			int result;
			if (int.TryParse(displayOrder, out result) == false)
			{
				Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_DISPLAY_ORDER_ERRORMESSSAGE] = WebMessages.FeaturePageCategoryDisplayOrderTypeError;
				this.DuplicateCategoryFlg = false;
			}

			// 空値チェック
			if (string.IsNullOrEmpty(categoryId))
			{
				Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_CATEGORY_ERRORMESSSAGE] = WebMessages.FeaturePageCategoryEmptyError;
				this.DuplicateCategoryFlg = false;
				return this.DuplicateCategoryFlg;
			}

			// 数値チェック
			if (Regex.IsMatch(categoryId, @"^[0-9]+$") == false)
			{
				Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_CATEGORY_ERRORMESSSAGE] = WebMessages.FeaturePageCategoryTypeError;
				this.DuplicateCategoryFlg = false;
				return this.DuplicateCategoryFlg;
			}

			// 桁数チェック
			if ((categoryId.Length % 3) != 0)
			{
				Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_CATEGORY_ERRORMESSSAGE] = WebMessages.FeaturePageCategoryDigitError;
				this.DuplicateCategoryFlg = false;
				return this.DuplicateCategoryFlg;
			}

			if (this.TempData.FeaturePageCategoryListParamModel.CategoryId != categoryId)
			{
				var service = new FeaturePageCategoryService();
				var count = service.GetMatchingCategoryId(this.SessionWrapper.LoginShopId, (parentCategoryId + categoryId));

				if (count != 0)
				{
					Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_CATEGORY_ERRORMESSSAGE] = WebMessages.FeaturePageCategoryDuplicateError;
					this.DuplicateCategoryFlg = false;
					return this.DuplicateCategoryFlg;
				}
			}
			return this.DuplicateCategoryFlg;
		}

		/// <summary>特集ページカテゴリワークサービス</summary>
		private FeaturePageCategoryWorkerService Service { get { return GetDefaultService<FeaturePageCategoryWorkerService>(); } }
		/// <summary>最上位カテゴリID</summary>
		public string ParentCategoryId
		{
			get { return (string)Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_PARENT_CATEGORY_ID]; }
			set { Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_PARENT_CATEGORY_ID] = value; }
		}
		/// <summary>子カテゴリID</summary>
		public string CategoryId
		{
			get { return (string)Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_CATEGORY_ID]; }
			set { Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_CATEGORY_ID] = value; }
		}
		/// <summary>カテゴリ名</summary>
		public string CategoryName
		{
			get { return (string)Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_CATEGORY_NAME]; }
			set { Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_CATEGORY_NAME] = value; }
		}
		/// <summary>表示順</summary>
		public string DisplayOrder
		{
			get { return (string)Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_DISPLAY_ORDER]; }
			set { Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_DISPLAY_ORDER] = value; }
		}
		/// <summary>カテゴリ説明</summary>
		public string CategoryOutline
		{
			get { return (string)Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_CATEGORY_OUTLINE]; }
			set { Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_CATEGORY_OUTLINE] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_VALID_FLG]; }
			set { Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_VALID_FLG] = value; }
		}
		/// <summary>詳細画面からの遷移か</summary>
		public bool IsCategoryDetail
		{
			get { return (bool)Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_PARENT_IS_CATEGORY_DETAIL]; }
			set { Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_PARENT_IS_CATEGORY_DETAIL] = value; }
		}
		/// <summary>変更前カテゴリID</summary>
		public string BeforeCategoryId
		{
			get { return (string)Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_BEFORE_CATEGORY_ID]; }
			set { Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_BEFORE_CATEGORY_ID] = value; }
		}
		/// <summary>カテゴリ重複フラグ</summary>
		public bool DuplicateCategoryFlg
		{
			get { return (bool)Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_DUPLICATE_CATEGORY_FLG]; }
			set { Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_DUPLICATE_CATEGORY_FLG] = value; }
		}
		/// <summary>最上位カテゴリ登録フラグ</summary>
		public bool FeaturePageRootCategoryRegistFlg
		{
			get
			{
				if (Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_ROOT_CATEGORY_REGIST_FLG] == null) return false;
				return (bool)Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_ROOT_CATEGORY_REGIST_FLG];
			}
			set { Session[Constants.SESSION_KEY_FEATURE_PAGE_CATEGORY_ROOT_CATEGORY_REGIST_FLG] = value; }
		}
	}
}