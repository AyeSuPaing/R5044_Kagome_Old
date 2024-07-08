/*
=========================================================================================================
  Module      : 商品カテゴリツリー出力コントローラ処理(BodyProductCategoryTree.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Option;
using w2.App.Common.Web.WrappedContols;
using w2.App.Common.Global.Translation;
using w2.Domain.NameTranslationSetting.Helper;
using w2.App.Common.Global.Region;
using w2.Domain.NameTranslationSetting;

public partial class Form_Common_Product_BodyProductCategoryTree : ProductUserControl
{
	#region ラップ済コントロール宣言
	WrappedRepeater WrCategoryList { get { return GetWrappedControl<WrappedRepeater>("rCategoryList"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// カテゴリツリー情報取得
			//------------------------------------------------------
			DataView categories = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("Product", "GetCategoryTree"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_PRODUCTCATEGORY_SHOP_ID, this.ShopId);
				input.Add(Constants.FIELD_PRODUCT_BRAND_ID1, this.BrandId);
				input.Add("root_category_sort_kbn", Constants.ROOT_CATEGORY_SORT_KBN);
				input.Add(Constants.FIELD_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG, this.UserFixedPurchaseMemberFlg);

				// 特定階層までデフォルトで表示する為、SQLの条件文を作成
				string likeMask = new string('_', Constants.CONST_CATEGORY_ID_LENGTH);
				StringBuilder where = new StringBuilder();
				where.Append(Constants.TABLE_PRODUCTCATEGORY).Append(".").Append(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID).Append(" like '").Append(likeMask).Append("'");
				if ((this.DefaultDisplayCategoryDepth != 1)
					&& (this.DefaultDisplayCategoryDepth <= Constants.CONST_CATEGORY_DEPTH))
				{
					for (int loop = 1; loop < this.DefaultDisplayCategoryDepth; loop++)
					{
						StringBuilder likeMasks = new StringBuilder().Insert(0, likeMask, (loop + 1));
						where.Append(" OR ").Append(Constants.TABLE_PRODUCTCATEGORY).Append(".").Append(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID).Append(" like '").Append(likeMasks).Append("'");
					}
				}
				// 指定カテゴリの子階層までを表示する為、SQLの条件文を作成
				for (int loop = 0; loop < (this.CategoryId.Length / Constants.CONST_CATEGORY_ID_LENGTH); loop++)
				{
					string paramKey = "category_id_like_escaped_" + (loop + 1).ToString();
					where.Append(" OR ").Append(Constants.TABLE_PRODUCTCATEGORY).Append(".").Append(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID).Append(" like @").Append(paramKey).Append(" + '").Append(likeMask).Append("' ESCAPE '#'");

					input.Add(paramKey, StringUtility.SqlLikeStringSharpEscape(this.CategoryId.Substring(0, Constants.CONST_CATEGORY_ID_LENGTH * (loop + 1))));
					sqlStatement.AddInputParameters(paramKey, SqlDbType.NVarChar, 60);
				}

				// 条件文置換
				string whereReplace = where.ToString();
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ first_category_where @@", whereReplace.Replace(Constants.TABLE_PRODUCTCATEGORY + ".", "first_cat."));
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ category_where @@", whereReplace.Replace(Constants.TABLE_PRODUCTCATEGORY + ".", "rec_cat."));

				// SQL発行（Windowsエクスプローラ風にカテゴリ情報を取得）
				categories = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
			}

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				var categoryIds = (categories != null && categories.Count != 0)
					? categories.Cast<DataRowView>()
						.Select(drv => ((string)drv[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID]).Replace("'", "''")).ToList()
					: new List<string>();

				var searchCondition = new NameTranslationSettingSearchCondition
				{
					DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY,
					MasterId1List = categoryIds,
					LanguageCode = RegionManager.GetInstance().Region.LanguageCode,
					LanguageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId,
				};
				var categoryTranslationSetting =
					new NameTranslationSettingService().GetTranslationSettingsByMultipleMasterId1(searchCondition);

				categories = NameTranslationCommon.SetTranslationDataToDataView(
					categories,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY,
					categoryTranslationSetting);
			}

			//------------------------------------------------------
			// カテゴリツリー作成
			//------------------------------------------------------
			// ツリーノード
			List<ProductCategoryTreeNode> productCategoryTreeNodes = new List<ProductCategoryTreeNode>();

			List<ProductCategoryTreeNode> currentTree = productCategoryTreeNodes;
			Stack<List<ProductCategoryTreeNode>> stack = new Stack<List<ProductCategoryTreeNode>>();

			int beforeDepth = 0;		// 0～
			ProductCategoryTreeNode undisplayCategory = null;
			foreach (DataRowView drv in categories)
			{
				// ツリーノード作成
				ProductCategoryTreeNode pctn = new ProductCategoryTreeNode(drv);

				// 非表示カテゴリの子カテゴリだったらcontinue
				if ((undisplayCategory != null)
					&& (pctn.CategoryId.StartsWith(undisplayCategory.CategoryId)))
				{
					continue;
				}

				// 会員ランク表示判定
				if ((pctn.LowerMemberCanDisplayTreeFlg == Constants.FLG_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG_VALID)
					|| MemberRankOptionUtility.CheckMemberRankPermission(this.MemberRankId, pctn.MemberRankId))
				{
					// 深さが増したら、追加先リスト更新
					int currentDepth = (pctn.CategoryId.Length / Constants.CONST_CATEGORY_ID_LENGTH) - 1;
					if (currentDepth > beforeDepth)
					{
						stack.Push(currentTree);
						currentTree = currentTree[currentTree.Count - 1].Childs;
					}
					else
					{
						for (int loop = 0; loop < (beforeDepth - currentDepth); loop++)
						{
							currentTree = stack.Pop();
						}
					}

					// 追加して深さのパラメタ更新
					currentTree.Add(pctn);
					beforeDepth = currentDepth;
				}
				else
				{
					undisplayCategory = pctn;
				}
			}
			this.DataSource = productCategoryTreeNodes;
		}
		this.WrCategoryList.DataSource = this.DataSource;
		this.WrCategoryList.DataBind();
	}

	/// <summary>デフォルト表示カテゴリー階層</summary>
	public int DefaultDisplayCategoryDepth { get; set; }
	/// <summary>データソース</summary>
	public List<ProductCategoryTreeNode> DataSource
	{
		get { return (List<ProductCategoryTreeNode>)ViewState["ProductCategoryTreeDataSource"]; }
		set { ViewState["ProductCategoryTreeDataSource"] = value; }
	}
	/// <summary>表示するか</summary>
	public bool IsVisibleCategory
	{
		get
		{
			return (this.WrCategoryList.DataSource != null)
				&& (((List<ProductCategoryTreeNode>)this.WrCategoryList.DataSource).Count > 0);
		}
	}
}
