/*
=========================================================================================================
  Module      : 商品子カテゴリ一覧出力コントローラ処理(BodyProductChildCategoryList.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Option;
using w2.App.Common.Product;

public partial class Form_Common_Product_BodyProductChildCategoryList : ProductUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		// HACK: DataBind未対応
		//		 対応するなら、DataBind継承して、DispChildCategories呼び出す。
		if (!IsPostBack)
		{
			// HACK: WrappedControlがコードビハインドなしAspxページに対応してないので、直接指定することにする。
			//		 このコントロールは他にaspxコントロールを持たないので、
			//		 Repeater消したらコントロールの意味がなくなる。
			//		 必要なら、rCategoryListを探し出すコードをここに追加することとする。
			DispChildCategories(rCategoryList,
				string.IsNullOrEmpty(this.ShopId) ? Constants.CONST_DEFAULT_SHOP_ID : this.ShopId, // 店舗IDは表に見せたくないので、指定がなくても動くようにする
				this.BrandId, 
				this.CategoryId, 
				new CacheOption()
				{
					NoCache = this.NoCache,
					CacheSpan = this.CacheHour
				});
		}
	}

	/// <summary>
	/// リピーターに子カテゴリ一覧を表示
	/// </summary>
	/// <param name="repeater">表示するリピーター</param>
	/// <param name="shopId">店舗ID</param>
	/// <param name="brandId">ブランドID</param>
	/// <param name="categoryId">カテゴリID</param>
	/// <param name="cacheOption">キャッシュオプション</param>
	/// <remarks>UIコントロールとデータを紐づける部分として、他メソッドから切り分けている。つまりMVC的な意味でのコントロール層</remarks>
	protected void DispChildCategories(Repeater repeater, string shopId, string brandId, string categoryId, CacheOption cacheOption)
	{
		repeater.DataSource = GetChildCategoriesFromCacheOrDB(shopId, brandId, categoryId, cacheOption);

		repeater.DataBind();
	}

	/// <summary>
	/// キャッシュから、ないしDBから子カテゴリ一覧データをとってくる
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="brandId">ブランドID</param>
	/// <param name="categoryId">カテゴリID</param>
	/// <param name="cacheOption">キャッシュオプション</param>
	/// <returns>子カテゴリ一覧</returns>
	/// <remarks>本来であれば、別クラスにわけるべきだが、今回はマイナーバージョンアップなので、影響を極小にするべくそれを避ける。</remarks>
	protected ProductCategoryTreeNode[] GetChildCategoriesFromCacheOrDB(string shopId, string brandId, string categoryId, CacheOption cacheOption)
	{
		var cacheKey = this.GetType().FullName + string.Join(",", shopId, brandId, categoryId);
		ProductCategoryTreeNode[] res = Cache[cacheKey] as ProductCategoryTreeNode[];
		if ((cacheOption.NoCache) || (res == null))
		{
			var data = CreateCategoryNodesFromDataView(
						GetChildCategoriesFromDB(shopId, brandId, categoryId));

			Cache.Insert(cacheKey,
				data, 
				null, 
				System.DateTime.Now.AddHours(cacheOption.CacheSpan < 1 ? 1 : cacheOption.CacheSpan), 
				System.Web.Caching.Cache.NoSlidingExpiration);

			return data;
		}
		else
		{
			return res;
		}
	}

	/// <summary>
	/// DBから子カテゴリ一覧を取得
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="brandId">ブランドID</param>
	/// <param name="categoryId">カテゴリID</param>
	/// <param name="cacheOption">キャッシュオプション</param>
	/// <returns>子カテゴリ一覧</returns>
	/// <remarks>本来であれば、別クラスにわけるべきだが、今回はマイナーバージョンアップなので、影響を極小にするべくそれを避ける。</remarks>
	protected DataView GetChildCategoriesFromDB(string shopId, string brandId, string categoryId)
	{
		Hashtable input = new Hashtable()
		{
			{Constants.FIELD_PRODUCTCATEGORY_SHOP_ID, shopId},
			{Constants.FIELD_PRODUCT_BRAND_ID1, brandId},
			{Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID, categoryId}
		};

		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("Product", "GetChildCategories"))
		{
			return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
		}
	}

	/// <summary>
	/// カテゴリーノードをDataViewから生成
	/// </summary>
	/// <param name="data">DataViewによるカテゴリテーブル</param>
	/// <returns>カテゴリーノード</returns>
	protected ProductCategoryTreeNode[] CreateCategoryNodesFromDataView(DataView data)
	{
		List<ProductCategoryTreeNode> res = new List<ProductCategoryTreeNode>();
		foreach (DataRowView row in data)
		{
			res.Add(new ProductCategoryTreeNode(row));
		}

		return res.ToArray();
	}

	/// <summary>
	/// キャッシュのオプションを提供するクラス
	/// </summary>
	/// <remarks>キャッシュを利用するコントロールではこの二つはだいたい利用するので、w2.Web.UI.Controlのような名前空間があればそちらに移動させたい。</remarks>
	protected class CacheOption
	{
		/// <summary> キャッシュ時間　単位は任意 </summary>
		public int CacheSpan{ get; set; }

		/// <summary> キャッシュを利用しない </summary>
		public bool NoCache{ get; set; }
	}

	/// <summary> 店舗ID</summary><remarks>DataBind未対応  なお親クラスであるProductUserControlに同名プロパティがあるが用途が違うためnewしている。親クラスではリクエストパラメタを参照しているが、当クラスでは値指定のために使う。</remarks>
	public new string ShopId{ get; set; }
	/// <summary> カテゴリID </summary><remarks>DataBind未対応  なお親クラスであるProductUserControlに同名プロパティがあるが用途が違うためnewしている。親クラスではリクエストパラメタを参照しているが、当クラスでは値指定のために使う。</remarks>
	public new string CategoryId{ get; set; }
	/// <summary> キャッシュを利用しない </summary>
	public bool NoCache { get; set; }
	/// <summary> キャッシュ時間 (1以下の場合、1が使われる) </summary>
	public int CacheHour { get; set; }
}
