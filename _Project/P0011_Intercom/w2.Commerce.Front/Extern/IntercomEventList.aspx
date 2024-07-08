<%--
=========================================================================================================
  Module      : インターコム用レコメンドイベント一覧画面(IntercomEventList.aspx)
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
<%@ Page Title="イベント一覧" Language="C#" Inherits="BasePage" MasterPageFile="~/Form/Common/DefaultPage.master" %>
<%@ Import Namespace="System.Data" %>

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
		
	/// <summary>イベントID格納セッションキー</summary>
	private const string PLUGIN_SESSION_KEY_RECOMMEND = "PLUGIN_SESSION_KEY_RECOMMEND";
	/// <summary>レコメンドイベント情報が正常に取得できたかのフラグ格納セッションキー</summary>
	private const string PLUGIN_SESSION_KEY_RECOMMEND_FLAG = "PLUGIN_SESSION_KEY_RECOMMEND_FLAG";
	/// <summary>イベントID用クエリストリングキー</summary>
	private const string QUERY_KEY_EVENTID = "eventid";
	/// <summary>シリアル管理フラグID用クエリストリングキー</summary>
	private const string QUERY_KEY_SERIALFLAG = "serialflag";
	/// <summary>カテゴリ用クエリストリングキー</summary>
	private const string QUERY_KEY_CATEGORY = "cat";
	/// <summary>優待販売のカテゴリID</summary>
	private const string CATEGORY_YUTAI = "080";
	

	/// <summary>リピータデータソース用テーブル名</summary>
	private const string TAB_NAME_EVENT = "EventTable";
	/// <summary>項目名_イベントID</summary>
	private const string COL_NAME_EVENT_ID = "EventID";
	/// <summary>項目名_シリアル管理フラグ</summary>
	private const string COL_NAME_SERIAL_FLAG = "SerialFlag";
	/// <summary>項目名_並び順</summary>
	private const string COL_NAME_DISPORDER = "DisplayOrder";

	private const string ERR_MSG_NON_RECOMMEND = "レコメンド情報が利用不可です。時間をおいてから再度ログイン後お願いします。";

	private const string MSG_NON_EVENT = "イベント情報がありません。";

	//------------------------------------------------------
	// 設定ファイルからロードするもの
	//------------------------------------------------------
	/// <summary>レコメンド対象商品一覧画面</summary>
	private string RECOMMEND_PRODUCT_LIST_URL = "";
	/// <summary>シリアル番号入力画面</summary>
	private string SERIAL_INPUT_URL = "";
	/// <summary>イベントバナー格納</summary>
	private string EVENT_BANNER_URL = "";
	/// <summary>イベント一覧画面</summary>
	private string ASPX_EVENT_LIST = "";
	
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
		
		//レコメンド商品一覧画面URL
		RECOMMEND_PRODUCT_LIST_URL = Constants.PATH_ROOT + IntercomSettings.GetAppStringSetting("ASPX_PRODUCT_LIST");
		//シリアルNo入力画面URL
		SERIAL_INPUT_URL = Constants.PATH_ROOT + IntercomSettings.GetAppStringSetting("ASPX_SERIAL_INPUT");
		//イベントバナー格納ディレクトリURL
		EVENT_BANNER_URL = Constants.PATH_ROOT + IntercomSettings.GetAppStringSetting("IC_BANNER_PATH");
		//イベント一覧画面
		ASPX_EVENT_LIST = Constants.PATH_ROOT + IntercomSettings.GetAppStringSetting("ASPX_EVENT_LIST");
		
	}
		
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>
	/// ログインしていない場合はログインページへ遷移する
	/// セッションからログインプラグインでセットされたイベントの情報を取得する
	/// イベントの情報が取得できなかった場合はエラー画面へ遷移する
	/// イベントの一覧情報をリピータのデータソースとして割り当てる。
	/// この際、並び順はイベントIDの降順
	/// </remarks>
	protected void Page_Load(object sender, EventArgs e)
	{	
		//設定ファイルロード
		Load_IntercomConfig();
		
		//ログインしていなかったらログインページへ
		if (this.IsLoggedIn == false)
		{
			string loginUrl = this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN
				+ "?" + Constants.REQUEST_KEY_NEXT_URL + "=" + HttpUtility.UrlEncode("https://" + this.Request.Url.Authority + ASPX_EVENT_LIST);

			Response.Redirect(loginUrl);
		}
		
		if (!IsPostBack)
		{
		
			//レコメンドイベントディクショナリ	
			IDictionary<string, Hashtable> eventDic = null;

			if (this.Session != null)
			{
				try
				{
					//セッションからレコメンドID取り出し
					eventDic = (IDictionary<string, Hashtable>)Session[PLUGIN_SESSION_KEY_RECOMMEND];
				}
				catch
				{
					//エラー時はNullで作る
					eventDic = null;
				}
			}

			if (eventDic == null)
			{
				//レコメンドイベントディクショナリ取れない場合はエラー画面へ
				Session[Constants.SESSION_KEY_ERROR_MSG] = ERR_MSG_NON_RECOMMEND;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			if (eventDic.Count == 0)
			{
				//レコメンド取得フラグが1の場合は、イベントがありませんメッセージ
				if(Session[PLUGIN_SESSION_KEY_RECOMMEND_FLAG] != null)
				{
					if (Session[PLUGIN_SESSION_KEY_RECOMMEND_FLAG].ToString() == "1")
					{
						lblNotFindEvent.Text = MSG_NON_EVENT;
					}
				}
				else
				{
					//ディクショナリにイベント一件もない場合もエラー画面へ
					Session[Constants.SESSION_KEY_ERROR_MSG] = ERR_MSG_NON_RECOMMEND;
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}

			//バインド用のデータテーブル作成
			System.Data.DataTable dt = new System.Data.DataTable(TAB_NAME_EVENT);
			dt.Columns.Add(COL_NAME_EVENT_ID);
			dt.Columns.Add(COL_NAME_SERIAL_FLAG);
			dt.Columns.Add(COL_NAME_DISPORDER,typeof(Int32));

			//foreach
			//バインド用のデータテーブルにイベントIDとシリアル管理フラグをいb年とがある分だけセット
			foreach (string eventID in eventDic.Keys)
			{
				System.Data.DataRow dr = dt.NewRow();
				dr[COL_NAME_EVENT_ID] = eventID;
				dr[COL_NAME_SERIAL_FLAG] = (string)eventDic[eventID][COL_NAME_SERIAL_FLAG];
				dr[COL_NAME_DISPORDER] = Convert.ToInt32(eventDic[eventID][COL_NAME_DISPORDER]);
				dt.Rows.Add(dr);
			}

			//バインド
			//バインドの際はイベントID降順で表示するためデータビューにしてから
			DataView dv = dt.DefaultView;
			dv.Sort = COL_NAME_DISPORDER + " DESC ";
			RepEventList.DataSource = dv;
			RepEventList.DataBind();
		}
		
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

<%-- ▽編集可能領域：コンテンツ▽ --%>    

      <div class="boxblue_common mg_b35">
        <p>このページはお客様がご購入可能な優待、またはバージョンアップ販売の一覧画面です。</p>
        ご希望のイベントを下記よりお選びいただき、ご購入の手続きを行ってください。
      </div>
      
      <div class="mg_b35">
      <asp:Label runat="server" ID="lblNotFindEvent"></asp:Label>
			<asp:Repeater ID="RepEventList" runat="server" >
			<ItemTemplate>
			<div id="Div1" style="display:<%#((string)Eval("SerialFlag") == "0")?"block":"none"%>">
				<a href="<%=RECOMMEND_PRODUCT_LIST_URL + "?" + QUERY_KEY_EVENTID + "="%><%# HttpUtility.UrlEncode((string)Eval(COL_NAME_EVENT_ID)) %><%= "&" + QUERY_KEY_SERIALFLAG + "="%><%#HttpUtility.UrlEncode((string)Eval(COL_NAME_SERIAL_FLAG)) %><%= "&" + QUERY_KEY_CATEGORY + "=" + HttpUtility.UrlEncode(CATEGORY_YUTAI)%>" ><image src="<%#EVENT_BANNER_URL + "/" + Eval(COL_NAME_EVENT_ID)%>.jpg"></image></a>
			</div>
			<div id="Div2" style="display:<%#((string)Eval("SerialFlag") != "0")?"block":"none"%>">
				<a href="<%=SERIAL_INPUT_URL + "?" + QUERY_KEY_EVENTID + "="%><%# HttpUtility.UrlEncode((string)Eval(COL_NAME_EVENT_ID)) %><%= "&" + QUERY_KEY_SERIALFLAG + "="%><%#HttpUtility.UrlEncode((string)Eval(COL_NAME_SERIAL_FLAG)) %><%= "&" + QUERY_KEY_CATEGORY + "=" + HttpUtility.UrlEncode(CATEGORY_YUTAI)%>" ><image src="<%#EVENT_BANNER_URL + "/" + Eval(COL_NAME_EVENT_ID)%>.jpg"></image></a>
			</div>
			</br>
			</ItemTemplate>
			</asp:Repeater>
      </div>

      
      <h3>会員についてのお問い合わせ</h3>
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

<%-- △編集可能領域△ --%>

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