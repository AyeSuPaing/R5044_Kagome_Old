/*
=========================================================================================================
  Module      : Lpページリポジトリ (LandingPageRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.LandingPage.Helper;

namespace w2.Domain.LandingPage
{
	/// <summary>
	/// Lpページデザインリポジトリ
	/// </summary>
	internal class LandingPageRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "LandingPage";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal LandingPageRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal LandingPageRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="publicDateKbn">公開日条件区分</param>
		/// <param name="searchWord">検索条件：タイトル又はファイル名</param>
		/// <param name="publicStatus">公開状態</param>
		/// <param name="designMode">デザインモード</param>
		/// <returns>検索結果</returns>
		internal LandingPageDesignModel[] Search(string publicDateKbn, string searchWord, string publicStatus, string designMode)
		{
			var para = new Hashtable
			{
				{Constants.FIELD_LANDINGPAGEDESIGN_PAGE_TITLE,searchWord},
				{"public_date_kbn", publicDateKbn},
				{Constants.FIELD_LANDINGPAGEDESIGN_PUBLIC_STATUS,publicStatus},
				{Constants.FIELD_LANDINGPAGEDESIGN_DESIGN_MODE, designMode},
			};
			var dv = Get(XML_KEY_NAME, "Search", para);
			return dv.Cast<DataRowView>().Select(drv => new LandingPageDesignModel(drv)).ToArray();
		}
		#endregion

		#region ~SearchByNumberOfPages ページ数分を検索
		/// <summary>
		/// ページ数分を検索
		/// </summary>
		/// <param name="publicDateKbn">公開日条件区分</param>
		/// <param name="searchWord">検索条件：タイトル又はファイル名</param>
		/// <param name="publicStatus">公開状態</param>
		/// <param name="designMode">デザインモード</param>
		/// <param name="isCartListLp">カートLPを使用しているか</param>
		/// <param name="bgnRowNum">取得開始番号</param>
		/// <param name="endRowNum">取得終了番号</param>
		/// <returns>検索結果</returns>
		internal LandingPageDesignModel[] SearchByNumberOfPages(
			string publicDateKbn,
			string searchWord,
			string publicStatus,
			string designMode,
			string isCartListLp,
			int bgnRowNum,
			int endRowNum)
		{
			var para = new Hashtable
			{
				{ Constants.FIELD_LANDINGPAGEDESIGN_PAGE_TITLE, searchWord },
				{ "public_date_kbn", publicDateKbn },
				{ Constants.FIELD_LANDINGPAGEDESIGN_PUBLIC_STATUS, publicStatus },
				{ Constants.FIELD_LANDINGPAGEDESIGN_DESIGN_MODE, designMode },
				{ "is_cart_list_lp", isCartListLp },
				{ Constants.FIELD_COMMON_BEGIN_NUM, bgnRowNum },
				{ Constants.FIELD_COMMON_END_NUM, endRowNum },
			};
			var dv = Get(XML_KEY_NAME, "SearchByNumberOfPages", para);
			return dv.Cast<DataRowView>().Select(drv => new LandingPageDesignModel(drv)).ToArray();
		}
		#endregion

		#region ~GetPageData ページデータ取得
		/// <summary>
		/// ページデータ取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>モデル</returns>
		internal LandingPageDesignModel GetPageData(string pageId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_LANDINGPAGEDESIGN_PAGE_ID, pageId}
			};
			var dv = Get(XML_KEY_NAME, "GetPageData", ht);
			if (dv.Count == 0) return null;
			return new LandingPageDesignModel(dv[0]);
		}
		#endregion

		#region ~GetPageBlockData ブロックデータ取得
		/// <summary>
		/// ブロックデータ取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>ブロックデータ</returns>
		internal LandingPageDesignBlockModel[] GetPageBlockData(string pageId, string designType)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_LANDINGPAGEDESIGNBLOCK_PAGE_ID, pageId},
				{Constants.FIELD_LANDINGPAGEDESIGNBLOCK_DESIGN_TYPE, designType}
			};
			var dv = Get(XML_KEY_NAME, "GetPageBlockData", ht);
			return dv.Cast<DataRowView>().Select(drv => new LandingPageDesignBlockModel(drv)).ToArray();
		}
		#endregion

		#region ~GetPageElementData 要素データ取得
		/// <summary>
		/// 要素データ取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>要素データ</returns>
		internal LandingPageDesignElementModel[] GetPageElementData(string pageId, string designType)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_LANDINGPAGEDESIGNBLOCK_PAGE_ID, pageId},
				{Constants.FIELD_LANDINGPAGEDESIGNBLOCK_DESIGN_TYPE, designType}
			};
			var dv = Get(XML_KEY_NAME, "GetPageElementData", ht);
			return dv.Cast<DataRowView>().Select(drv => new LandingPageDesignElementModel(drv)).ToArray();
		}
		#endregion

		#region ~GetPageElementData 属性データ取得
		/// <summary>
		/// 属性データ取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>属性データ</returns>
		internal LandingPageDesignAttributeModel[] GetPageAttributeData(string pageId, string designType)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_LANDINGPAGEDESIGNBLOCK_PAGE_ID, pageId},
				{Constants.FIELD_LANDINGPAGEDESIGNBLOCK_DESIGN_TYPE, designType}
			};
			var dv = Get(XML_KEY_NAME, "GetPageAttributeData", ht);
			return dv.Cast<DataRowView>().Select(drv => new LandingPageDesignAttributeModel(drv)).ToArray();
		}
		#endregion

		#region ~GetPageProductSetData 商品セットデータ取得
		/// <summary>
		/// 商品セットデータ取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>商品セットデータ</returns>
		internal LandingPageProductSetModel[] GetPageProductSetData(string pageId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_LANDINGPAGEDESIGN_PAGE_ID, pageId}
			};
			var dv = Get(XML_KEY_NAME, "GetPageProductSetData", ht);
			return dv.Cast<DataRowView>().Select(drv => new LandingPageProductSetModel(drv)).ToArray();
		}
		#endregion

		#region ~GetPageProductData 商品データ取得
		/// <summary>
		/// 商品データ取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>商品データ</returns>
		internal LandingPageProductModel[] GetPageProductData(string pageId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_LANDINGPAGEDESIGNBLOCK_PAGE_ID, pageId}
			};
			var dv = Get(XML_KEY_NAME, "GetPageProductData", ht);
			return dv.Cast<DataRowView>().Select(drv => new LandingPageProductModel(drv)).ToArray();
		}
		#endregion

		#region ~GetCount LP件数取得
		/// <summary>
		/// LP件数取得
		/// </summary>
		/// <returns>LP件数</returns>
		internal int GetCount()
		{
			var dv = Get(XML_KEY_NAME, "GetCount");
			var count = (int)dv[0][0];
			return count;
		}
		#endregion

		#region ~InsertPage LandingPage登録
		/// <summary>
		/// LandingPage登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertPage(LandingPageDesignModel model)
		{
			Exec(XML_KEY_NAME, "InsertPage", model.DataSource);
		}
		#endregion

		#region ~InsertBlock Block登録
		/// <summary>
		/// Block登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertBlock(LandingPageDesignBlockModel model)
		{
			Exec(XML_KEY_NAME, "InsertBlock", model.DataSource);
		}
		#endregion

		#region ~InsertElementDesign ElementDesign登録
		/// <summary>
		/// Element登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertElement(LandingPageDesignElementModel model)
		{
			Exec(XML_KEY_NAME, "InsertElement", model.DataSource);
		}
		#endregion

		#region ~InsertAttribute Attribute登録
		/// <summary>
		/// Attribute登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertAttribute(LandingPageDesignAttributeModel model)
		{
			Exec(XML_KEY_NAME, "InsertAttribute", model.DataSource);
		}
		#endregion

		#region ~InsertProductSet ProductSet登録
		/// <summary>
		/// ProductSet登録
		/// </summary>
		/// <param name="model">ProductSetモデル</param>
		internal void InsertProductSet(LandingPageProductSetModel model)
		{
			Exec(XML_KEY_NAME, "InsertProductSet", model.DataSource);
		}
		#endregion

		#region ~InsertProduct Product登録
		/// <summary>
		/// Product登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertProduct(LandingPageProductModel model)
		{
			Exec(XML_KEY_NAME, "InsertProduct", model.DataSource);
		}
		#endregion

		#region ~UpdatePage ページ更新
		/// <summary>
		/// ページ更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdatePage(LandingPageDesignModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdatePage", model.DataSource);
			return result;
		}
		#endregion

		#region ~DeletePage ページ削除
		/// <summary>
		/// ページ削除
		/// </summary>
		/// <param name="pageId">対象ID</param>
		internal void DeletePage(string pageId)
		{
			Exec(XML_KEY_NAME, "DeletePage", new Hashtable { { Constants.FIELD_LANDINGPAGEDESIGN_PAGE_ID, pageId } });
		}
		#endregion

		#region ~DeletePageBolocks ブロック削除
		/// <summary>
		/// ブロック削除
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="designType">デザインタイプ</param>
		internal void DeletePageBolocks(string pageId, string designType = "")
		{
			Exec(XML_KEY_NAME, "DeletePageBolocks", new Hashtable { { Constants.FIELD_LANDINGPAGEDESIGNBLOCK_PAGE_ID, pageId }, { Constants.FIELD_LANDINGPAGEDESIGNBLOCK_DESIGN_TYPE, designType } });
		}
		#endregion

		#region ~DeletePageElements 要素削除
		/// <summary>
		/// 要素削除
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="designType">デザインタイプ</param>
		internal void DeletePageElements(string pageId, string designType = "")
		{
			Exec(XML_KEY_NAME, "DeletePageElements", new Hashtable { { Constants.FIELD_LANDINGPAGEDESIGNELEMENT_PAGE_ID, pageId }, { Constants.FIELD_LANDINGPAGEDESIGNELEMENT_DESIGN_TYPE, designType } });
		}
		#endregion

		#region ~DeletePageAttributes 属性削除
		/// <summary>
		/// 属性削除
		/// </summary>
		/// <param name="pageid">ページID</param>
		/// <param name="designType">デザインタイプ</param>
		internal void DeletePageAttributes(string pageid, string designType = "")
		{
			Exec(XML_KEY_NAME, "DeletePageAttributes", new Hashtable { { Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_PAGE_ID, pageid }, { Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_DESIGN_TYPE, designType } });
		}
		#endregion

		#region ~DeletePageProductSets 商品セット削除
		/// <summary>
		/// 商品セット削除
		/// </summary>
		/// <param name="pageId">ページID</param>
		internal void DeletePageProductSets(string pageId)
		{
			Exec(XML_KEY_NAME, "DeletePageProductSets", new Hashtable { { Constants.FIELD_LANDINGPAGEPRODUCTSET_PAGE_ID, pageId } });
		}
		#endregion

		#region ~DeletePageProducts 商品削除
		/// <summary>
		/// 商品削除
		/// </summary>
		/// <param name="pageId">ページID</param>
		internal void DeletePageProducts(string pageId)
		{
			Exec(XML_KEY_NAME, "DeletePageProducts", new Hashtable { { Constants.FIELD_LANDINGPAGEPRODUCT_PAGE_ID, pageId } });
		}
		#endregion

		#region ~GetPageByFileName ファイル名からページを取得
		/// <summary>
		/// ファイル名からページを取得
		/// </summary>
		/// <param name="pageFileName">ファイル名</param>
		/// <returns>ページ情報</returns>
		public LandingPageDesignModel[] GetPageByFileName(string pageFileName)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_LANDINGPAGEDESIGN_PAGE_FILE_NAME, pageFileName}
			};
			var dv = Get(XML_KEY_NAME, "GetPageByFileName", ht);
			return dv.Cast<DataRowView>().Select(drv => new LandingPageDesignModel(drv)).ToArray();
		}
		#endregion

		#region ~GetAllPage 全ページ取得
		/// <summary>
		/// 全ページ取得
		/// </summary>
		/// <returns></returns>
		internal LandingPageDesignModel[] GetAllPage()
		{
			var dv = Get(XML_KEY_NAME, "GetAllPage");
			return dv.Cast<DataRowView>().Select(drv => new LandingPageDesignModel(drv)).ToArray();
		}
		#endregion

		#region ~GetCountOfSearchByParamModel パラメタ検索ヒット件数取得
		/// <summary>
		/// パラメタ検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		internal int GetCountOfSearchByParamModel(LandingPageSearchParamModel condition)
		{
			var input = condition.CreateHashtableParams();
			var dv = Get(XML_KEY_NAME, "GetCountOfSearchByParamModel", input);
			return (int)dv[0][0];
		}
		#endregion

		#region ~SearchByParamModel パラメタ検索
		/// <summary>
		/// パラメタ検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>Lpページ</returns>
		internal LandingPageDesignModel[] SearchByParamModel(LandingPageSearchParamModel condition)
		{
			var input = condition.CreateHashtableParams();
			var dv = Get(XML_KEY_NAME, "SearchByParamModel", input);
			return dv.Cast<DataRowView>().Select(drv => new LandingPageDesignModel(drv)).ToArray();
		}
		#endregion

		#region ~ABテストアイテム件数取得
		/// <summary>
		/// ABテストアイテム件数取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>件数</returns>
		internal int GetCountInAbTestItemByPageId(string pageId)
		{
			var dv = Get(XML_KEY_NAME, "GetCountInAbTestItemByPageId", new Hashtable { { Constants.FIELD_ABTESTITEM_PAGE_ID, pageId } });
			return (int)dv[0][0];
		}
		#endregion
	}
}
