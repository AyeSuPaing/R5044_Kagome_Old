<%--
=========================================================================================================
  Module      : インターコム用レコメンドイベント一覧画面(IntercomSerialInput.aspx)
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
<%@ Page Title="シリアル入力" Language="C#" Inherits="BasePage" MasterPageFile="~/Form/Common/DefaultPage.master" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Product" %>

<script runat="server">
	
	/*
	 * セッションにセットしているレコメンド情報について
	 * セッションKEY：PLUGIN_SESSION_KEY_RECOMMENDにディクショナリで登録
	 * ディクショナリ内はKey：イベントID、Value：シリアル_商品情報ハッシュ 
	 * シリアル_商品情報ハッシュは以下のKeyとValue
	 * Key：SerialNo　value：シリアルNo（String）
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
	/// <summary>インターコムID格納セッションキー</summary>
	private const string PLUGIN_SESSION_KEY_ICID = "PLUGIN_SESSION_KEY_ICID";
	/// <summary>イベントID用クエリストリングキー</summary>
	private const string QUERY_KEY_EVENTID = "eventid";
	/// <summary>シリアル管理フラグID用クエリストリングキー</summary>
	private const string QUERY_KEY_SERIALFLAG = "serialflag";
	/// <summary>シリアルチェックの連携処理区分</summary>
	private const string PROC_TYPE_SERIAL_CHECK = "SerialCheck";
	/// <summary>シリアルチェックターゲットデータテーブル名</summary>
	private const string TAB_SERIAL_TARGET = "SerialTargetTable";
	/// <summary>イベントIDカラム名</summary>
	private const string COL_EVENTID = "EventID";
	/// <summary>シリアルNoカラム名</summary>
	private const string COL_SERIALNO = "SerialNo";
	/// <summary>商品IDカラム名</summary>
	private const string COL_PRODUCTID = "ProductID";
	/// <summary>バリエーションIDカラム名</summary>
	private const string COL_VARIATIONID = "VariationID";
	/// <summary>購入数カラム名</summary>
	private const string COL_PRODUCTCOUNT = "ProductCount";
	/// <summary>処理結果データテーブル名</summary>
	private const string TAB_RESULT = "ResultTable";
	/// <summary>処理結果区分カラム名</summary>
	private const string COL_PROC_RESULT = "ProcResult";
	/// <summary>処理結果メッセージカラム名</summary>
	private const string COL_PROC_MESSAGE = "ProcMessage";
	/// <summary>ユーザー情報データテーブル</summary>
	private const string TAB_USER = "UserIDTable";
	/// <summary>w2ユーザーIDカラム</summary>
	private const string COL_USERID = "LinkedUserID";
	/// <summary>インターコムユーザーIDカラム</summary>
	private const string COL_LINKED_USERID = "UserID";
	/// <summary>処理結果区分_正常終了</summary>
	private const string RESULT_SUCCESS = "Success";
	/// <summary>処理結果区分_チェックNG</summary>
	private const string RESULT_CHECKNG = "CheckNG";
	/// <summary>処理結果区分_失敗</summary>
	private const string RESULT_FAILED = "Failed";
	/// <summary>シリアルチェックNoの場合</summary>
	private const string ERR_MSG_NON_RECOMMEND = "入力したシリアルNoは利用できません。再度入力をお願いします。";
	/// <summary>カテゴリ用クエリストリングキー</summary>
	private const string QUERY_KEY_CATEGORY = "cat";
	/// <summary>優待販売のカテゴリID</summary>
	private const string CATEGORY_YUTAI = "080";

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
	/// <summary>レコメンド対象商品一覧画面</summary>
	private string RECOMMEND_PRODUCT_LIST_URL = "";
	/// <summary>イベントバナー格納</summary>
	private string EVENT_BANNER_URL = "";
	private string PA = "";
	private string ID = "";
	private string DOM = "";
	private string FLG = "";
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
		//連携ディレクトリ名
		SYNC_LOG_DIR = IntercomSettings.GetAppStringSetting("SYNC_LOG_DIR");
		//イベントバナー格納ディレクトリURL
		EVENT_BANNER_URL = Constants.PATH_ROOT + IntercomSettings.GetAppStringSetting("IC_BANNER_PATH");
		//レコメンド商品一覧画面
		RECOMMEND_PRODUCT_LIST_URL = Constants.PATH_ROOT + IntercomSettings.GetAppStringSetting("ASPX_PRODUCT_LIST");
		PA = IntercomSettings.GetAppStringSetting("PA");
		ID = IntercomSettings.GetAppStringSetting("ID");
		DOM = IntercomSettings.GetAppStringSetting("DOM");
		FLG = IntercomSettings.GetAppStringSetting("FLG");
		
	}
	
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>
	/// ログインしていない場合はログイン画面へ遷移する
	/// クエリストリングからイベントIDを取得し、セッション内にイベントIDに該当するシリアルNoを取得する。
	/// シリアルNoが取得できた場合はレコメンド商品一覧画面へ遷移する。
	/// シリアルNoが取得できない場合はビューステートへイベントIDを格納し、本画面を表示する。
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

		if (!IsPostBack)
		{
			
			//クエリストリング取り出し
			string eventid = this.Context.Request.QueryString[QUERY_KEY_EVENTID];

			//クエリストリングからイベントIDが取れなければトップへ
			if (eventid == null)
			{
				Response.Redirect(Constants.PATH_ROOT);
			}
			
			//セッションからイベントIDディクショナリのイベントIDに該当する情報を取得
			Hashtable ht = ((IDictionary<string,Hashtable>)Session[PLUGIN_SESSION_KEY_RECOMMEND])[eventid];
				
			//すでにイベントIDに該当するシリアルがあればProductListへ
			if ((string)ht[COL_SERIALNO] != "")
			{
				Response.Redirect(RECOMMEND_PRODUCT_LIST_URL + "?" + QUERY_KEY_EVENTID + "=" + eventid);
			}
			
			//取出したクエリストリングをビューステートへ
			this.ViewState.Add(QUERY_KEY_EVENTID, eventid);
		}
	}

	/// <summary>
	/// シリアル入力ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>
	/// ビューステートからイベントIDを取り出し、外部連携を利用してイベントIDと、入力したシリアルNoのチェックを行う。
	/// チェックがOKであれば、シリアルNoをセッションに格納し、レコメンド商品一覧画面へ遷移する。
	/// チェックNGの場合は、エラー画面へ遷移する。
	/// </remarks>
	protected void Button1_Click(object sender, EventArgs e)
	{
		//viesStateからイベントID取り出し
		string eveid = (string)this.ViewState[QUERY_KEY_EVENTID];

		//------------------------------------------------------
		// 外部連携を利用してシリアルNoのチェック
		//------------------------------------------------------	

		//イベントIDを利用してwebメソッド実行
		//連携用データセットの生成
		DataTable dt = new DataTable(TAB_SERIAL_TARGET);
		//カラム追加
		dt.Columns.Add(COL_EVENTID);
		dt.Columns.Add(COL_SERIALNO);
		//データ追加
		DataRow dr = dt.NewRow();
		dr[COL_EVENTID] = eveid;
		dr[COL_SERIALNO] = InputSerial.Text;
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
		
		//連携用データセット
		DataSet sendds = new DataSet();
		sendds.Tables.Add(dt);
		sendds.Tables.Add(userdata);

		try
		{

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
			DataSet returnds = (DataSet)mi.Invoke(cry, new object[] { PROC_TYPE_SERIAL_CHECK, sendds
				, SYNC_LOG_DIR
				, ID
				, PA
				, DOM 
				, FLG});

			/* ＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃＃ */

			string returnResult = (string)returnds.Tables[TAB_RESULT].Rows[0][COL_PROC_RESULT];

			asm = null;
				
			//OKだったらシリアルNo格納後ProductListへ
			switch (returnResult)
			{

				case RESULT_SUCCESS:
					//OKだったら何もしない、Tryぬけてからredirect
					break;
				default:
					//エラーの場合、エラーメッセージ画面へ
					Session[Constants.SESSION_KEY_ERROR_MSG] = ERR_MSG_NON_RECOMMEND;
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
					break;
			}
		}
		catch(Exception ex)
		{
			//エラーの場合、エラーメッセージ画面へ
			Session[Constants.SESSION_KEY_ERROR_MSG] = ERR_MSG_NON_RECOMMEND;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		//OKだったらシリアルNoをセッションの街頭イベントに格納後ProductListへ
		((IDictionary<string, Hashtable>)Session[PLUGIN_SESSION_KEY_RECOMMEND])[eveid][COL_SERIALNO] = this.InputSerial.Text;
		Response.Redirect(RECOMMEND_PRODUCT_LIST_URL + "?" + QUERY_KEY_EVENTID + "=" + eveid
			+ "&" + QUERY_KEY_CATEGORY + "=" + HttpUtility.UrlEncode(CATEGORY_YUTAI));
		
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
		
      <div id="divTopArea">
        <%-- ▽レイアウト領域：トップエリア▽ --%>
        <%-- △レイアウト領域△ --%>
      </div>
      
      <div class="center mg_b20"><img src="<%=EVENT_BANNER_URL + "/" + (string)this.ViewState[QUERY_KEY_EVENTID]%>.jpg"></div>
      
      <div class="center mg_b20">平素より弊社商品をご愛用いただき誠にありがとうございます。 <br>
      このページではご登録のシリアル番号を確認させていただきます。</div>
      
      <div id="idform" class="mg_b30">
        <div class="box_common_s">
          <div class="mg_b20"><span class="f_bold">ご登録</span>のシリアル番号をご入力ください。<br />
          <span class="f_12">【シリアル番号の入力例：XXXXXXX-XXXXXX-XXXX（7桁-6桁-4桁）】</span></div>
          <div class="center mg_b10"><span class="f_bold">シリアル番号</span>：<asp:TextBox ID="InputSerial" runat="server" CssClass="w200" style="ime-mode: disabled;" size="40" maxlength="25"></asp:TextBox></div>
          <div class="center mg_b15"><asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="　送 信　" /></div>
          <p class="mg_b00"><span class="c_at">※</span>シリアル番号がご不明な方は会員ページをご覧ください。</p>
          <ul><li class="li_link"><a href="https://secure.intercom.co.jp/IntercomDirect/member/login.aspx" target="_blank">会員ページ</a></li></ul>
        </div>
      </div>
      
      <h3>お申し込みの流れと注意事項</h3>
      <p>お手持ちのご登録シリアル番号をご入力いただき、下記の流れに沿ってお申し込みください。</p>

      <ol class="mg_b20">
        <li class="f_middle">ご登録シリアル番の入力 （この画面です）</li>
        <li class="f_middle">ご購入商品の確認</li>
        <li class="f_middle">買い物かごの内容の確認</li>
        <li class="f_middle">ご注文情報（お届け先・お支払い方法）の入力</li>
        <li class="f_middle">ご注文情報の確認（最終確認）とご注文の決定</li>
      </ol>
      <ul class="mg_b50">
        <li class="li_list">既にバージョンアップ版をご購入いただいているお客様は、ご購入いただけません。</li>
      </ul>


      <h3>お問い合わせ</h3>
      <p class="mg_b05 f_bold">【お問い合わせ窓口】</p>
      <p class="mg_l10 mg_b20">株式会社インターコム  インターコムダイレクト担当</p>
      <p class="mg_b05 f_bold">【電話番号】</p>
      <p class="mg_l10 mg_b20">03-3839-6820</p>
      <p class="mg_b05 f_bold">【お問い合わせ時間】</p>
      <p class="mg_l10 mg_b05">月曜日～金曜日　10:00～12:00 / 13:30～17:00<br />
        （土曜、日曜、祝日、年末年始および弊社の休業日はお休みとさせていただきます）<br />
        お問い合わせフォームは24時間受付いたしますが、回答は上記の時間となります。</p>
      <ul class="mg_l10">
        <li class="li_blank"><a href="<%= Constants.PATH_ROOT %>Form/Inquiry/InquiryInput.aspx" target="_blank">お問い合わせフォーム</a></li>
      </ul>

      <div id="divBottomArea">
        <%-- ▽レイアウト領域：ボトムエリア▽ --%>
        <%-- △レイアウト領域△ --%>
      </div>
      
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
  
</asp:Content>