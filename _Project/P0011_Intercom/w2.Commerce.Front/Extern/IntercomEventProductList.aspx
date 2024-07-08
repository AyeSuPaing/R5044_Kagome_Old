<%--
=========================================================================================================
  Module      : インターコム用レコメンド商品一覧画面(IntercomEventProductList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Application : w2.Commerce.Front
  BaseVersion : V5.0
  Author      : M.Ochiai
  email       : product@w2solution.co.jp
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
  URL         : http://www.w2solution.co.jp/
=========================================================================================================
PKG-V5.0[PF0001] 2010/09/10 M.Ochiai        v5.0用に分離
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductSearchBox" Src="~/Form/Common/Product/BodyProductSearchBox.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductCategoryTree" Src="~/Form/Common/Product/BodyProductCategoryTree.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductRanking" Src="~/Form/Common/Product/BodyProductRanking.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductHistory" Src="~/Form/Common/Product/BodyProductHistory.ascx" %>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Title="イベント商品一覧" Language="C#" Inherits="ProductPage" MasterPageFile="~/Form/Common/DefaultPage.master" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Product" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<script runat="server">
	
	/*
	 * セッションにセットしているレコメンド情報について
	 * セッションKEY：PLUGIN_SESSION_KEY_RECOMMENDにディクショナリで登録
	 * ディクショナリ内はKEY：イベントID、Value：シリアル_商品情報ハッシュ 
	 * シリアル_商品情報ハッシュは以下のKeyとValue
	 * KEY：SerialNo　value：シリアルNo（String）
	 * Key：EventProduct Value：商品情報ハッシュのリスト(IList<HashTable>)
	 * 商品情報ハッシュは以下のKeyとValue
	 * Key：ProductID Value：商品ID（String)
	 * Key：VariationID Value：商品バリエーションID（String)
	 * 
	 */
	
	
	//ここだけ環境ごとに変更_インターコム用フォルダパス
	private const string INTERCOM_SETTINGS_DIR_PATH = @"C:\inetpub\wwwroot\R5044_Kagome.Develop\Web\w2.Commerce.Front\Extern\";
	
	
	//------------------------------------------------------
	// 各種定数
	//------------------------------------------------------
	/// <summary>イベントID格納セッションキー</summary>
	private const string PLUGIN_SESSION_KEY_RECOMMEND = "PLUGIN_SESSION_KEY_RECOMMEND";
	/// <summary>商品情報格納ハッシュキー</summary>
	public const string COL_EVENT_PRODUCT = "EventProduct"; 
	/// <summary>インターコムID格納セッションキー</summary>
	private const string PLUGIN_SESSION_KEY_ICID = "PLUGIN_SESSION_KEY_ICID";
	/// <summary>イベントID用クエリストリングキー</summary>
	private const string QUERY_KEY_EVENTID = "eventid";
	/// <summary>レコメンド商品連携連携用区分</summary>
	private const string PROC_TYPE_RECOMMEND_PRODUCT = "RecommendProduct";
	
	/// <summary>レコメンド対象情報データテーブル</summary>
	private const string TAB_RECOMMEND_TARGET = "EventTable";
	/// <summary>イベントIDカラム</summary>
	private const string COL_EVENTID = "EventID";
	/// <summary>シリアルNoカラム</summary>
	private const string COL_SERIAL = "SerialNo";

	/// <summary>ユーザー情報データテーブル</summary>
	private const string TAB_USER = "UserIDTable";
	/// <summary>w2ユーザーIDカラム</summary>
	private const string COL_USERID = "LinkedUserID";
	/// <summary>インターコムユーザーIDカラム</summary>
	private const string COL_LINKED_USERID = "UserID";
	
	
	/// <summary>レコメンド商品データテーブル</summary>
	private const string TAB_RECOMMEND_PRODUCT = "EventProductTable";
	/// <summary>商品IDカラム</summary>
	private const string COL_PRODUCTID = "ProductID";
	/// <summary>商品バリエーションIDカラム</summary>
	private const string COL_VARIATIONID = "VariationID";
	
	
	/// <summary>リピータ内ボタンのCommand引数区切り文字</summary>
	private const string CMD_ARG_SPLIT = ",";
	/// <summary>レコメンド商品連携エラー時のメッセージ</summary>
	private const string ERR_MSG_NON_RECOMMEND = "レコメンド情報が利用不可です。時間をおいてから再度ログイン後お願いします。";
	/// <summary>商品情報データテーブル名</summary>
	private const string TAB_NAME_PRODUCT_INFO = "ProductInfo";
		
	//画面設定値
	private int MaxDispCount = 20;
	private string ImageSize = "S";

	//------------------------------------------------------
	// 設定ファイルからロードするもの
	//------------------------------------------------------	
	/// <summary>連携DLLの名前</summary>
	private string SYNC_DLL_NAME = "";
	/// <summary>連携インスタンスの名前</summary>
	private string SYNC_INSTANCE_NAME = "";
	/// <summary>連携メソッドの名前</summary>
	private string SYNC_METHOD_NAME = "";
	/// <summary>連携用ログフォルダパス</summary>
	private string SYNC_LOG_DIR = "";
	private string PA = "";
	private string ID = "";
	private string DOM = "";
	private string FLG = "";
	/// <summary>イベントバナー格納</summary>
	private string EVENT_BANNER_URL = "";

	/// <summary>
	/// 設定ファイルロード
	/// </summary>
	private void Load_IntercomConfig()
	{
		//設定ファイルロード
		w2.App.Common.ConfigurationSetting IntercomSettings = new w2.App.Common.ConfigurationSetting(
			INTERCOM_SETTINGS_DIR_PATH,
			w2.App.Common.ConfigurationSetting.ReadKbn.C300_MarketingPlanner,
			w2.App.Common.ConfigurationSetting.ReadKbn.C300_Pc);

		//各種設定内容ロード
		//連携DLL名
		SYNC_DLL_NAME = IntercomSettings.GetAppStringSetting("SYNC_DLL_NAME");
		//連携インスタンス名
		SYNC_INSTANCE_NAME = IntercomSettings.GetAppStringSetting("SYNC_INSTANCE_NAME");
		//連携メソッド名
		SYNC_METHOD_NAME = IntercomSettings.GetAppStringSetting("SYNC_METHOD_NAME");
		//連携ログディレクトリパス
		SYNC_LOG_DIR = IntercomSettings.GetAppStringSetting("SYNC_LOG_DIR");


		PA = IntercomSettings.GetAppStringSetting("PA");
		ID = IntercomSettings.GetAppStringSetting("ID");
		DOM = IntercomSettings.GetAppStringSetting("DOM");
		FLG = IntercomSettings.GetAppStringSetting("FLG");
		
		//イベントバナー格納ディレクトリURL
		EVENT_BANNER_URL = Constants.PATH_ROOT + IntercomSettings.GetAppStringSetting("IC_BANNER_PATH");
		
	}
	
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>
	/// クエリストリングからイベントIDを取り出し、
	/// イベントIDをレコメンド商品情報取得のwebサービスに投げる。
	/// webサービスから受け取った商品ID、バリエーションIDを条件にw2_productviewを検索し、
	/// 検索結果をリピーターのデータソースに割り当て後、バインドする
	/// webサービスが失敗した場合はエラー画面へ遷移する
	/// クエリストリングやセッションから処理に必要な情報が取れない場合は何もしない（returnして終わり）
	/// </remarks>
	protected void Page_Load(object sender, EventArgs e)
	{
		//設定ファイルロード
		Load_IntercomConfig();
						
		
		//ログインしていなかったらログインページへ
		if (this.IsLoggedIn == false)
		{
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN);
		}
		
		//クエリストリング取り出し
		string eventid = this.Context.Request.QueryString[QUERY_KEY_EVENTID];
		
		//セッションがなければreturn
		if (Session == null)
		{
			//return;
			//レコメンドイベントディクショナリ取れない場合はエラー画面へ
			Session[Constants.SESSION_KEY_ERROR_MSG] = ERR_MSG_NON_RECOMMEND;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		//セッションにレコメンドイベント情報がなければreturn
		if (Session[PLUGIN_SESSION_KEY_RECOMMEND] == null)
		{
			//return;
			//レコメンドイベントディクショナリ取れない場合はエラー画面へ
			Session[Constants.SESSION_KEY_ERROR_MSG] = ERR_MSG_NON_RECOMMEND;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		//クエリストリングからイベントIDが取れなければreturn
		if (eventid == null)
		{
			//return;
			//レコメンドイベントディクショナリ取れない場合はエラー画面へ
			Session[Constants.SESSION_KEY_ERROR_MSG] = ERR_MSG_NON_RECOMMEND;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		//クエリストリングからイベントIDが取れなければreturn
		if (eventid == "")
		{
			//return;
			//レコメンドイベントディクショナリ取れない場合はエラー画面へ
			Session[Constants.SESSION_KEY_ERROR_MSG] = ERR_MSG_NON_RECOMMEND;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
		
		//セッションにイベントIDに該当するレコメンド情報がなければreturn
		if(((IDictionary<string,Hashtable>)Session[PLUGIN_SESSION_KEY_RECOMMEND]).Keys.Contains(eventid) == false)
		{
			//return;
			//レコメンドイベントディクショナリ取れない場合はエラー画面へ
			Session[Constants.SESSION_KEY_ERROR_MSG] = ERR_MSG_NON_RECOMMEND;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}	
		
		if (!IsPostBack)
		{
			
			//セッションからイベントIDに該当するレコメンド商品情報を取得
			object productList = ((IDictionary<string, Hashtable>)Session[PLUGIN_SESSION_KEY_RECOMMEND])[eventid]["EventProduct"];

			//------------------------------------------------------
			// 外部連携を利用してイベントに該当する商品情報を取得
			//------------------------------------------------------			
			//セッションに商品情報がないときだけ外部連携実行
			/* 20111011_Mod 商品情報は常にwebサービスを利用して取得するよう変更 */
			//if (productList == null)
			//{
				try
				{
					//イベントIDを利用してwebメソッド実行
					//連携用データセットの生成
					DataTable dt = new DataTable(TAB_RECOMMEND_TARGET);
					//カラム追加
					dt.Columns.Add(COL_EVENTID);
					dt.Columns.Add(COL_SERIAL);
					//データ追加
					DataRow dr = dt.NewRow();
					dr[COL_EVENTID] = eventid;
					//シリアル入力から来ていない場合は空がセットされる
					dr[COL_SERIAL] = ((IDictionary<string, Hashtable>)Session[PLUGIN_SESSION_KEY_RECOMMEND])[eventid][COL_SERIAL];
					dt.Rows.Add(dr);

					//ユーザーデータ
					DataTable userdata = new DataTable(TAB_USER);
					userdata.Columns.Add(COL_USERID);
					userdata.Columns.Add(COL_LINKED_USERID);
					//データ追加
					DataRow userdr = userdata.NewRow();
					userdr[COL_USERID] = this.LoginUserId;
					userdr[COL_LINKED_USERID] = (string)Session[PLUGIN_SESSION_KEY_ICID];
					userdata.Rows.Add(userdr);

					DataSet sendds = new DataSet();
					sendds.Tables.Add(dt);
					sendds.Tables.Add(userdata);
					

					//連携用ライブラリのロード
					//パス
					string dllpath = Path.GetDirectoryName(Constants.PHYSICALDIRPATH_PLUGINS_STORAGE_LOCATION)
						+ @"\" + SYNC_DLL_NAME;

					//アセンブリロード
					Assembly asm = null;
					asm = Assembly.LoadFile(dllpath);
					//IntercomWebServiceインスタンス生成
					object cry = asm.CreateInstance(SYNC_INSTANCE_NAME);

					//MethodInfo IntercomWebServiceインスタンスのExecuteメソッド
					MethodInfo mi = cry.GetType().GetMethod(SYNC_METHOD_NAME);

					//MethodInfoを利用して実行
					//引数はobject配列にセット
					//第一引数は連携区分（文字列で指定、【RecommendProduct】）
					//第二引数はwebサービスの引数データセットに含めるデータテーブル、イベントIDを格納したもの
					//第三引数はwebサービス呼出時のログ出力ディレクトリのパス
					DataSet returnds = (DataSet)mi.Invoke(cry, new object[] { PROC_TYPE_RECOMMEND_PRODUCT, sendds
						, SYNC_LOG_DIR
						, ID
						, PA
						, DOM
						, FLG});

					//実行結果のデータセットから商品情報取り出し
					productList = new List<Hashtable>();

					//商品情報ある分だけハッシュつくってリストにセット
					foreach (DataRow productRow in returnds.Tables[TAB_RECOMMEND_PRODUCT].Rows)
					{
						Hashtable productHash = new Hashtable();
						productHash.Add(COL_PRODUCTID, (string)productRow[COL_PRODUCTID]);
						productHash.Add(COL_VARIATIONID, (string)productRow[COL_VARIATIONID]);
						
						((List<Hashtable>)productList).Add(productHash);
					}


					//セッションの該当イベントIDに商品情報リストを詰める
					//ここで詰めておけば二回目はwebサービスよばない
					((IDictionary<string, Hashtable>)Session[PLUGIN_SESSION_KEY_RECOMMEND])[eventid][COL_EVENT_PRODUCT] = (List<Hashtable>)productList;
				}
				catch
				{
					//商品情報が取れない場合はエラー画面へ
					Session[Constants.SESSION_KEY_ERROR_MSG] = ERR_MSG_NON_RECOMMEND;
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			//}

			//------------------------------------------------------
			// DBから商品情報取得
			//------------------------------------------------------
			//接続文字列
			string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["w2Database"].ToString();
			//実行結果のデータテーブル
			DataTable producttab = new DataTable(TAB_NAME_PRODUCT_INFO);
			try
			{

				//w2_productviewを検索
				using (SqlConnection conn = new SqlConnection(connStr))
				{
					//SqlCommand
					SqlCommand selCmd = new SqlCommand();

					StringBuilder pidINStatementBuilder = new StringBuilder(); //Product_IDのIN条件文
					StringBuilder vidINStatementBuilder = new StringBuilder();	//Valiation_IDのIN条件文
					int loopCnt = 0; //ループカウンタ

					//セッションの商品情報分ループしてDBから該当商品情報を取得するためのIN句を作成
					foreach (Hashtable productHash in
						(List<Hashtable>)((IDictionary<string, Hashtable>)Session[PLUGIN_SESSION_KEY_RECOMMEND])[eventid][COL_EVENT_PRODUCT])
					{
						//二回目以降は区切りのカンマ入れる
						if (loopCnt > 0) { pidINStatementBuilder.Append(","); vidINStatementBuilder.Append(","); }

						string pidParaName = "@" + COL_PRODUCTID + "_" + loopCnt;
						string vidParaName = "@" + COL_VARIATIONID + "_" + loopCnt;

						//SQL文
						pidINStatementBuilder.Append(pidParaName);
						vidINStatementBuilder.Append(vidParaName);

						//SQLパラメータ
						selCmd.Parameters.AddWithValue(pidParaName, (string)productHash[COL_PRODUCTID]);
						//バリエーションIDが空の場合は商品IDをバリエーションIDとする
						if ((string)productHash[COL_VARIATIONID] == "")
						{
							selCmd.Parameters.AddWithValue(vidParaName, (string)productHash[COL_PRODUCTID]);
						}
						else
						{
							selCmd.Parameters.AddWithValue(vidParaName, (string)productHash[COL_VARIATIONID]);
						}
						
						//ループカウンタインクリメント
						loopCnt++;
					}

					//Select文
					string sqlSrt = " SELECT * FROM w2_productview "
						+ " WHERE product_id IN(" + pidINStatementBuilder.ToString() + ")"
						+ " AND variation_id IN(" + vidINStatementBuilder.ToString() + ")"
            + " ORDER BY name_kana";

					//ConnectionSet
					selCmd.Connection = conn;

					//CommandText
					selCmd.CommandText = sqlSrt;

					//Adapter
					SqlDataAdapter adp = new SqlDataAdapter(selCmd);

					//Fill
					adp.Fill(producttab);
				}
			}
			catch(Exception ex)
			{
				//商品情報が取れない場合はエラー画面へ
				Session[Constants.SESSION_KEY_ERROR_MSG] = ERR_MSG_NON_RECOMMEND;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
		
			//バインド、データソースに上記でFillしたデータテーブル割り当てる
			RepProducttList.DataSource = producttab;
			RepProducttList.DataBind();
			
			//イベントIDをビューステートに保存
			this.ViewState.Add(QUERY_KEY_EVENTID, eventid);
			
		}
			
	}

	///=============================================================================================
	/// <summary>
	/// カート投入処理
	/// </summary>
	/// <param name="addCartKbn">カート投入区分</param>
	/// <param name="strCartAddProductCount">注文数</param>
	/// <param name="blSingleSelectAddCart">単体カート投入有無（単体:true、複数:false）</param>
	///=============================================================================================
	private void AddCart(Constants.AddCartKbn addCartKbn, string strCartAddProductCount, bool blSingleSelectAddCart,
		string productID,string variationID,string eventID, RepeaterCommandEventArgs e)
	{
				
		//------------------------------------------------------
		// 商品情報取得
		//------------------------------------------------------
		DataView dvProduct = GetProduct(this.ShopId, productID, variationID);
		if (dvProduct.Count == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
		//------------------------------------------------------
		// カート投入処理
		//------------------------------------------------------
		// カートが存在しないときはカート作成（この時点ではレコードは作成されないし、採番もされない）
		if (Session[Constants.SESSION_KEY_CART_LIST] == null)
		{
			Session[Constants.SESSION_KEY_CART_LIST]
				= new CartObjectList(StringUtility.ToEmpty(this.LoginUserId), Constants.FLG_ORDER_ORDER_KBN_PC, true);
		}

		// カート投入
		CartObjectList cartList = (CartObjectList)Session[Constants.SESSION_KEY_CART_LIST];
		//イベントIDをセットしてカート投入
		CartProduct ca = cartList.AddProduct(dvProduct[0], addCartKbn, StringUtility.ToEmpty(dvProduct[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]), 
			Convert.ToInt32(strCartAddProductCount), new w2.App.Common.Product.ProductOptionSettingList(""));
		//カートのProductOptionSettingにイベントIDを入れておく
		
		//すでに同じイベントが登録されている場合はスルー
		//if (ca.ProductOptionSettingList.Items.Count == 0)
		//{
		//AddProductメソッドの結果でカートオブジェクトが帰ってきた場合のみ
		if (ca != null)
		{
			ca.ProductOptionSettingList.Add(new ProductOptionSetting("", QUERY_KEY_EVENTID, new List<string> { eventID }));
			//------------------------------------------------------
			// カート投入後の画面遷移
			//------------------------------------------------------
			// カート一覧画面へ
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
		}
		else
		{
			((Label)e.Item.FindControl("valmsg")).Text = "商品は一回のお買い物で購入できる数量を超えました。 ";
		}
		
	}
	
	/// <summary>
	/// リピーター内のカート投入ボタン押下イベント
	/// </summary>
	/// <param name="source">イベント発生コントロール</param>
	/// <param name="e">RepeaterCommandEventArgs</param>
	protected void RepProducttList_ItemCommand1(object source, RepeaterCommandEventArgs e)
	{
		//引数取得
		string arg = (string)e.CommandArgument;
		//引数を商品IDとバリエーションIDに分解
		string productID = arg.Split(CMD_ARG_SPLIT.ToCharArray())[0];
		string variationID = arg.Split(CMD_ARG_SPLIT.ToCharArray())[1];

		//viesStateからイベントID取り出し
		string eventID = (string)this.ViewState[QUERY_KEY_EVENTID];
		
			
		//購入数は1	
		int cnt = 1;
								
		//カート投入処理へ
		AddCart(Constants.AddCartKbn.Normal, cnt.ToString(), true, productID, variationID, eventID,e);
	}

	protected string GetStockMessageImg(string ProductId)
	{
		DataView Product = GetProduct("0", ProductId, ProductId);

		string stockMessage = "";
    string stockMessageImageName = "";
		AppLogger.WriteInfo("商品数：" + Product.Count.ToString());

		if (Product.Count != 0)
		{
			stockMessage = w2.App.Common.Order.ProductCommon.CreateProductStockMessage(Product[0], true);
			AppLogger.WriteInfo("在庫文言テスト");
			AppLogger.WriteInfo(stockMessage);
			AppLogger.WriteInfo((string)Product[0]["stock_message_id"]);
			switch((string)Product[0]["stock_message_id"])
			{
				case "000":
					stockMessageImageName = (stockMessage == "在庫少量") ? "little" : ((stockMessage == "売切れ") ? "soldout" : "ok");
					break;

				case "001":
					stockMessageImageName = (stockMessage == "在庫少量") ? "little" : ((stockMessage == "3～5日入荷") ? "3days" : "ok");
					break;
				
				case "002":
					stockMessageImageName = (stockMessage == "在庫少量") ? "little" : ((stockMessage == "予約受付中") ? "reserve" : "ok");
					break;
					
				case "004":
					stockMessageImageName = (stockMessage == "在庫少量") ? "little" : ((stockMessage == "7月5日より発送！") ? "reserve" : "ok");
					break;
					

				default:
					stockMessageImageName = "ok";
					break;
			}
		}

		return (stockMessageImageName != "") ? stockMessageImageName : "ok";
	}

</script>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LastChanged="最終更新者" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <%-- ▽編集可能領域：HEAD追加部分▽ --%>
  <%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server"> 
  
  <!--#################　パン屑ナビ開始　#################-->
  <div id="id-navi-wrap">
    <div id="id-navi">
      <ul>
        <li class="home"><a href="<%= Constants.PATH_ROOT %>">ホーム</a></li>
        <li>優待販売</li>
      </ul>
    </div>
  </div>
  <!--#################　パン屑ナビ終了　#################--> 
  
  <!--#################　メイン部分囲み開始　#################-->
  <div id="id-main-wrap" class="clearfix"> 
    
    <!--#################　コンテンツ部分囲み開始　#################-->
    <div id="id-contents-wrap"> 
      
      <!--リストページのコンテンツの囲み開始-->
      <div id="productlist">
        <div id="divTopArea">
          <%-- ▽レイアウト領域：トップエリア▽ --%>
          <img src="<%=EVENT_BANNER_URL + "/" + (string)this.ViewState[QUERY_KEY_EVENTID]%>.jpg">
           <br/>
           <br/>
          <%-- △レイアウト領域△ --%>
        </div>
        <%-- ▽編集可能領域：コンテンツ▽ --%>
        <%-- 商品一覧リピーター。Evalで使える項目はDBから取得したw2_productviewの項目全部  --%>
        <asp:Repeater ID="RepProducttList" runat="server" onitemcommand="RepProducttList_ItemCommand1">
          <ItemTemplate> 
            
            <!--1商品開始-->
            <div class="productonewrap clearfix">
              <div class="pphoto"><span>
                <w2c:ProductImage ID="ProductImage1" ImageSize="S" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" />
                </span></div>
              <div class="pdetail">
                <h3><%#WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_NAME))%></h3>
                <p class="catchcopy"><%#WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_CATCHCOPY))%></p>
                <p><%#WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_OUTLINE))%></p>
                <asp:Label ID="valmsg" runat="server" ForeColor="Red" Font-Size="Small"></asp:Label>
                <table class="idtable mg_b25">
                  <tr>
                    <th>商品種別</th>
                    <td><%#WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_COOPERATION_ID1))%></td>
                    <th>メーカー</th>
                    <td><%#WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_COOPERATION_ID3))%></td>
                  </tr>
                  <tr>
                    <th>在庫状況</th>
                    <td><img src="<%= Constants.PATH_ROOT %>images/icon_stock/<%# GetStockMessageImg(Eval(Constants.FIELD_PRODUCT_PRODUCT_ID).ToString()) %>.gif" alt="在庫状況"></td>
                    <th>メーカー製品情報</th>
                    <td><ul>
                        <li class="li_blank"><%#(StringUtility.ToEmpty(Eval(Constants.FIELD_PRODUCT_URL)) != "") ? "<a href=\"" + WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_URL)) + "\" target=\"_blank\">あり</a>" : "なし"%></li>
                      </ul></td>
                  </tr>
                  <tr>
                    <%-- 会員価格 --%>
                    <th visible='<%# GetProductMemberRankPriceValid(Container.DataItem) %>' runat="server">会員価格（税込）</th>
                    <td colspan="3" class="tdcolspan" visible='<%# GetProductMemberRankPriceValid(Container.DataItem) %>' runat="server"><span class="price"><%# WebSanitizer.HtmlEncode(GetProductMemberRankPrice(Container.DataItem)) %></span><span class="yen">円</span> <span>【メーカー希望価格：<%#WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_COOPERATION_ID5))%>】</span></td>
                    <%-- 特別価格 --%>
                    <th visible='<%# GetProductSpecialPriceValid(Container.DataItem) %>' runat="server">販売価格（税込）</th>
                    <td colspan="3" class="tdcolspan" visible='<%# GetProductSpecialPriceValid(Container.DataItem) %>' runat="server"><span class="price"><%# WebSanitizer.HtmlEncode(GetProductSpecialPriceNumeric(Container.DataItem)) %></span><span class="yen">円</span> <span>【メーカー希望価格：<%#WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_COOPERATION_ID5))%>】</span></td>
                    <%-- 通常価格 --%>
                    <th visible='<%# GetProductNormalPriceValid(Container.DataItem) %>' runat="server">販売価格（税込）</th>
                    <td colspan="3" class="tdcolspan" visible='<%# GetProductNormalPriceValid(Container.DataItem) %>' runat="server"><span class="price"><%# WebSanitizer.HtmlEncode(GetProductPriceNumeric(Container.DataItem)) %></span><span class="yen">円</span> <span>【メーカー希望価格：<%#WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_COOPERATION_ID5))%>】</span></td>
                  </tr>
                </table>
                <div class="btnwrap">
                  <asp:LinkButton ID="lbCartAdd" runat="server" CommandName="lbCartAdd_Click" CommandArgument="<%#(string)Eval(Constants.FIELD_PRODUCT_PRODUCT_ID)+CMD_ARG_SPLIT+(string)Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)%>" ><img src="<%= Constants.PATH_ROOT %>images13/btn/cart_off.gif" width="128" height="30" alt="カートに入れる" class="mg_l15" /></asp:LinkButton>
                </div>
              </div>
            </div>
            <!--1商品終了--> 
            
          </ItemTemplate>
        </asp:Repeater>
        <%-- △編集可能領域△ --%>
      </div>
      <!--productlist終了 --> 
      
    </div>
    <!--#################　コンテンツ部分囲み終了　#################--> 
    
    <!--#################　サイドメニュー囲み開始　#################-->
    <div id="id-sidecontents-wrap">
      <%-- ▽レイアウト領域：レフトエリア▽ --%>
      <uc:BodyProductCategoryTree runat="server" />
      <%-- △レイアウト領域△ --%>
    </div>
    <!--#################　サイドメニュー囲み終了　#################--> 
    
  </div>
  <!--#################　メイン部分囲み終了　#################--> 
  
  <script type="text/javascript">
  	// バリエーション選択チェック(通常)
  	function add_cart_check() {
  		//この場合は常にTrue
  		return true;
  	}
</script> 
</asp:Content>