/*
=========================================================================================================
  Module      : Lpページサービス (LandingPageService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.LandingPage.Helper;
using w2.Domain.SubscriptionBox;

namespace w2.Domain.LandingPage
{
	/// <summary>
	/// Lpページデザインサービス
	/// </summary>
	public class LandingPageService : ServiceBase, ILandingPageService
	{
		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="publicDateKbn">公開日条件区分</param>
		/// <param name="searchWord">検索条件：タイトル又はファイル名</param>
		/// <param name="publicStatus">公開状態</param>
		/// <param name="designMode">デザインモード</param>
		/// <returns>検索結果列</returns>
		public LandingPageDesignModel[] Search(string publicDateKbn, string searchWord, string publicStatus, string designMode)
		{
			using (var repository = new LandingPageRepository())
			{
				var results = repository.Search(publicDateKbn, searchWord, publicStatus, designMode);

				if (Constants.CART_LIST_LP_OPTION == false)
				{
					results = results.Where(lPage => (lPage.IsCartListLp == false)).ToArray();
				}

				results = results.Select(
					r =>
						{
							r.ProductSets = GetPageProductSets(r.PageId);
							return r;
						}).ToArray();
				return results;
			}
		}
		#endregion

		#region +SearchByNumberOfPages ページ数分を検索
		/// <summary>
		/// ページ数分を検索
		/// </summary>
		/// <param name="publicDateKbn">公開日条件区分</param>
		/// <param name="searchWord">検索条件：タイトル又はファイル名</param>
		/// <param name="publicStatus">公開状態</param>
		/// <param name="designMode">デザインモード</param>
		/// <param name="bgnRowNum">取得開始番号</param>
		/// <param name="endRowNum">取得終了番号</param>
		/// <returns>検索結果列</returns>
		public LandingPageDesignModel[] SearchByNumberOfPages(
			string publicDateKbn,
			string searchWord,
			string publicStatus,
			string designMode,
			int bgnRowNum,
			int endRowNum)
		{
			using (var repository = new LandingPageRepository())
			{
				var landingPageDesignModels = repository.SearchByNumberOfPages(
					publicDateKbn,
					searchWord,
					publicStatus,
					designMode,
					Constants.CART_LIST_LP_OPTION ? Constants.FLG_CART_LIST_LP_ON : Constants.FLG_CART_LIST_LP_OFF,
					bgnRowNum,
					endRowNum);

				var results = landingPageDesignModels.Select(
					landingPageDesignModel =>
					{
						landingPageDesignModel.ProductSets = GetPageProductSets(landingPageDesignModel.PageId);
						return landingPageDesignModel;
					}).ToArray();
				return results;
			}
		}
		#endregion

		#region +Get ページ情報を取得
		/// <summary>
		/// ページ情報を取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>モデル</returns>
		public LandingPageDesignModel Get(string pageId)
		{
			using (var repository = new LandingPageRepository())
			{
				var model = repository.GetPageData(pageId);
				return model;
			}
		}
		#endregion

		#region +GetPageDataWithDesign デザイン情報とともにページ情報を取得
		/// <summary>
		/// デザイン情報とともにページ情報を取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>モデル</returns>
		public LandingPageDesignModel GetPageDataWithDesign(string pageId)
		{
			var modelPc = this.GetPageDataWithDesign(pageId, LandingPageConst.PAGE_DESIGN_TYPE_PC);
			var modelSp = this.GetPageDataWithDesign(pageId, LandingPageConst.PAGE_DESIGN_TYPE_SP);
			var bolcks = modelPc.Blocks.Concat(modelSp.Blocks).ToArray();
			modelPc.Blocks = bolcks;

			return modelPc;
		}
		/// <summary>
		/// デザイン情報とともにページ情報を取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>モデル</returns>
		public LandingPageDesignModel GetPageDataWithDesign(string pageId, string designType)
		{
			using (var repository = new LandingPageRepository())
			{
				var model = repository.GetPageData(pageId);
				if (model == null) { return null; }
				var blocks = repository.GetPageBlockData(pageId, designType).ToArray();
				var elements = repository.GetPageElementData(pageId, designType).ToArray();
				var attributes = repository.GetPageAttributeData(pageId, designType).ToArray();
				var productSets = GetPageProductSets(pageId);
				model.ProductSets = productSets;
				model.Blocks = blocks;

				foreach (var b in model.Blocks)
				{
					b.Elements = elements.Where(e => b.PageId == e.PageId
						&& b.DesignType == e.DesignType
						&& b.BlockIndex == e.BlockIndex)
						.ToArray();

					foreach (var e in b.Elements)
					{
						e.Attributes = attributes.Where(a => e.PageId == a.PageId
							&& b.DesignType == e.DesignType
							&& e.BlockIndex == a.BlockIndex
							&& e.ElementIndex == a.ElementIndex)
							.ToArray();
					}
				}

				return model;
			}
		}
		#endregion

		#region +GetPageProductSets 商品セット情報取得
		/// <summary>
		/// 商品セット情報取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>商品セット情報</returns>
		public LandingPageProductSetModel[] GetPageProductSets(string pageId)
		{
			using (var repository = new LandingPageRepository())
			{
				var productSets = repository.GetPageProductSetData(pageId);
				var products = repository.GetPageProductData(pageId);

				productSets = productSets.Select(
					ps =>
					{
						ps.Products = string.IsNullOrEmpty(ps.SubscriptionBoxCourseId)
							? products.Where(lpp => (ps.BranchNo == lpp.BranchNo)).ToArray()
							: GetSubscriptionBoxProducts(ps.SubscriptionBoxCourseId);
						return ps;
					}).ToArray();

				return productSets;
			}
		}
		#endregion

		#region +GetPageProducts 商品情報取得
		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>商品情報</returns>
		public LandingPageProductModel[] GetPageProducts(string pageId)
		{
			using (var repository = new LandingPageRepository())
			{
				var results = repository.GetPageProductData(pageId);
				return results;
			}
		}
		#endregion

		#region +GetSubscriptionBoxProducts
		/// <summary>
		/// Get product of SubscriptionBox based on subscriptionBoxCourseID
		/// </summary>
		/// <param name="subscriptionBoxCourseId">subscription Box Course Id</param>
		/// <returns>Array of LandingPageProductModel</returns>
		[Obsolete("製品に入れる前に廃止予定。")]
		public LandingPageProductModel[] GetSubscriptionBoxProducts(string subscriptionBoxCourseId)
		{
			// HACK: 商品決定処理をSubscriprionBox"Service"へ移譲。
			//       クエリでやるのではなくServiceかDataCacheControllerでやる。
			using (var repository = new SubscriptionBoxRepository())
			{
				var defaultItems = repository.GetProductsByHanpukaiIDAndNowDate(subscriptionBoxCourseId);
				var necessaryProducts = defaultItems.Where(p => (p.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID)).ToArray();
				if (necessaryProducts.Any()) defaultItems = necessaryProducts;
				var products = defaultItems
					.Where(p => string.IsNullOrEmpty(p.ProductId) == false)
					.Select(
						p => new LandingPageProductModel
						{
							ShopId = p.ShopId,
							BranchNo = p.BranchNo,
							ProductId = p.ProductId,
							VariationId = p.VariationId,
							Quantity = p.ItemQuantity
						})
					.ToArray();
				return products;
			}
		}
		#endregion

		#region ~GetCount LP件数取得
		/// <summary>
		/// LP件数取得
		/// </summary>
		/// <returns>LP件数</returns>
		public int GetCount()
		{
			using (var repository = new LandingPageRepository())
			{
				var count = repository.GetCount();
				return count;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(LandingPageDesignModel model)
		{
			using (var ac = new SqlAccessor())
			using (var repository = new LandingPageRepository(ac))
			{
				ac.OpenConnection();
				ac.BeginTransaction();
				repository.InsertPage(model);
				model.Blocks.ToList().ForEach((b) =>
				{
					repository.InsertBlock(b);
					b.Elements.ToList().ForEach((e) =>
					{
						repository.InsertElement(e);
						e.Attributes.ToList().ForEach((a) =>
						{
							repository.InsertAttribute(a);
						});
					});
				});
				model.ProductSets.ToList().ForEach(ps =>
				{
					ps.PageId = model.PageId;
					ps.LastChanged = model.LastChanged;
					repository.InsertProductSet(ps);
				});
				model.ProductSets.ToList().ForEach(ps =>
				{
					if (ps.Products != null)
					{
						ps.Products.ToList().ForEach(p =>
						{
							p.PageId = model.PageId;
							p.LastChanged = model.LastChanged;
							repository.InsertProduct(p);
						});
					}
				});
				ac.CommitTransaction();
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int UpdatePage(LandingPageDesignModel model)
		{
			using (var ac = new SqlAccessor())
			using (var repository = new LandingPageRepository(ac))
			{
				ac.OpenConnection();
				ac.BeginTransaction();
				var result = repository.UpdatePage(model);
				repository.DeletePageProductSets(model.PageId);
				repository.DeletePageProducts(model.PageId);
				model.ProductSets.ToList().ForEach(ps =>
				{
					ps.PageId = model.PageId;
					ps.LastChanged = model.LastChanged;
					repository.InsertProductSet(ps);
				});
				model.ProductSets.ToList().ForEach(ps =>
				{
					if (ps.Products != null)
					{
						ps.Products.ToList().ForEach(p =>
						{
							p.PageId = model.PageId;
							p.LastChanged = model.LastChanged;
							repository.InsertProduct(p);
						});
					}
				});
				ac.CommitTransaction();
				return result;
			}
		}
		#endregion

		#region +UpdatePageDesign
		/// <summary>
		/// ページデザイン更新
		/// </summary>
		/// <param name="model"></param>
		public void UpdatePageDesign(LandingPageDesignModel model)
		{
			using (var ac = new SqlAccessor())
			using (var repository = new LandingPageRepository(ac))
			{
				ac.OpenConnection();
				ac.BeginTransaction();

				repository.DeletePageBolocks(model.PageId, model.Blocks.First().DesignType);
				repository.DeletePageElements(model.PageId, model.Blocks.First().DesignType);
				repository.DeletePageAttributes(model.PageId, model.Blocks.First().DesignType);

				model.Blocks.ToList().ForEach((b) =>
				{
					repository.InsertBlock(b);
					b.Elements.ToList().ForEach((e) =>
					{
						repository.InsertElement(e);
						e.Attributes.ToList().ForEach((a) =>
						{
							repository.InsertAttribute(a);
						});
					});
				});

				ac.CommitTransaction();
			}
		}
		#endregion

		#region +DeletePage 削除
		/// <summary>
		/// ページ削除
		/// </summary>
		/// <param name="pageId">ページID</param>
		public void DeletePageData(string pageId)
		{
			using (var ac = new SqlAccessor())
			using (var repository = new LandingPageRepository(ac))
			{
				ac.OpenConnection();
				ac.BeginTransaction();
				repository.DeletePage(pageId);
				repository.DeletePageBolocks(pageId);
				repository.DeletePageElements(pageId);
				repository.DeletePageAttributes(pageId);
				repository.DeletePageProductSets(pageId);
				repository.DeletePageProducts(pageId);
				ac.CommitTransaction();
			}
		}
		#endregion

		#region +GetPageByFileName ファイル名からページを取得
		/// <summary>
		/// ファイル名からページを取得
		/// </summary>
		/// <param name="pageFileName">ファイル名</param>
		/// <returns>ページ情報</returns>
		public LandingPageDesignModel[] GetPageByFileName(string pageFileName)
		{
			using (var repository = new LandingPageRepository())
			{
				var results = repository.GetPageByFileName(pageFileName);
				return results;
			}
		}
		#endregion

		#region +GetAllPage 全ページ取得
		/// <summary>
		/// 全ページ取得
		/// </summary>
		/// <returns></returns>
		public LandingPageDesignModel[] GetAllPage()
		{
			using (var repository = new LandingPageRepository())
			{
				var results = repository.GetAllPage();
				return results;
			}
		}
		#endregion

		#region +GetCountOfSearchByParamModel パラメタ検索ヒット件数取得
		/// <summary>
		/// パラメタ検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetCountOfSearchByParamModel(LandingPageSearchParamModel condition)
		{
			using (var repository = new LandingPageRepository())
			{
				var count = repository.GetCountOfSearchByParamModel(condition);
				return count;
			}
		}
		#endregion

		#region +SearchByParamModel パラメタ検索
		/// <summary>
		/// パラメタ検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>Lpページ</returns>
		public LandingPageDesignModel[] SearchByParamModel(LandingPageSearchParamModel condition)
		{
			using (var repository = new LandingPageRepository())
			{
				var results = repository.SearchByParamModel(condition);
				return results;
			}
		}
		#endregion

		#region +ABテストアイテム件数取得
		/// <summary>
		/// ABテストアイテム件数取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>件数</returns>
		public int GetCountInAbTestItemByPageId(string pageId)
		{
			using (var repository = new LandingPageRepository())
			{
				var result = repository.GetCountInAbTestItemByPageId(pageId);
				return result;
			}
		}
		#endregion
	}
}
