using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;


namespace Test.w2.Plugin.P0011_Intercon.WebService
{
	/// <summary>
	/// UnitTest1 の概要の説明
	/// </summary>
	[TestClass]
	public class TestUserSync
	{

		public TestUserSync()
		{
			//
			// TODO: コンストラクター ロジックをここに追加します
			//
		}

		private TestContext testContextInstance;

		/// <summary>
		///現在のテストの実行についての情報および機能を
		///提供するテスト コンテキストを取得または設定します。
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region 追加のテスト属性
		//
		// テストを作成する際には、次の追加属性を使用できます:
		//
		// クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// 各テストを実行する前に、TestInitialize を使用してコードを実行してください
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// 各テストを実行した後に、TestCleanup を使用してコードを実行してください
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void 会員登録のテスト()
		{
			//引数データセット作成
			DataSet ds = new DataSet("TestDs");

			//処理区分データテーブルセット
			ds.Tables.Add(CreateProcTable("UserRegist"));

			//登録データ作成
			ds.Tables.Add(CreateInsertUserData());

			//webサービス実行
			WebRef.Iw2ServiceClient sc = new WebRef.Iw2ServiceClient();
			DataSet rtnds = sc.UserSyncExecute(ds);

			//戻ってきた処理結果検証
			Assert.AreEqual(rtnds.Tables["ResultTable"].Rows[0]["ProcResult"], "Success");
			Assert.AreEqual(rtnds.Tables["ResultTable"].Rows[0]["ProcMessage"], "正常終了");
			
			//戻ってきた登録ユーザID
			string rtnuserid = (string)rtnds.Tables["UserIDTable"].Rows[0]["UserID"];

			//登録ユーザーIDでDB検索
			DataTable getTab = new DataTable("gettab");
			string sqlStr = " SELECT * FROM W2_USER WHERE user_id = @user_id ";
			string connStr = WebConfigurationManager.ConnectionStrings["w2Database"].ToString();
			using(SqlConnection conn = new SqlConnection(connStr))
			{
				SqlCommand cmd = new SqlCommand(sqlStr, conn);
				cmd.Parameters.AddWithValue("@user_id", rtnuserid);

				SqlDataAdapter adp = new SqlDataAdapter(cmd);

				cmd.Connection.Open();

				adp.Fill(getTab);

				cmd.Connection.Close();
				
			}

			//取得データと比較
			foreach (DataColumn dc in ds.Tables["UserTable"].Columns)
			{
				string colname = dc.ColumnName;

				string targetval = ds.Tables["UserTable"].Rows[0][colname].ToString();
				string regival = getTab.Rows[0][colname].ToString();

				if (colname != "user_id" && colname != "password")
				{
					Assert.AreEqual(targetval, regival);
					
				}

			}
		}

		[TestMethod]
		public void 会員変更のテスト()
		{
			//引数データセット作成
			DataSet ds = new DataSet("TestDs");

			//処理区分データテーブルセット
			ds.Tables.Add(CreateProcTable("UserRegist"));

			//先に登録しておく

			//登録データ作成
			ds.Tables.Add(CreateInsertUserData());

			//webサービス実行
			WebRef.Iw2ServiceClient sc = new WebRef.Iw2ServiceClient();
			DataSet rtnds = sc.UserSyncExecute(ds);
			//戻ってきた登録ユーザID
			string rtnuserid = (string)rtnds.Tables["UserIDTable"].Rows[0]["UserID"];

			//登録したデータを対象に変更
			//引数データセット作成
			DataSet dsmod = new DataSet("TestModDs");

			//処理区分データテーブルセット
			dsmod.Tables.Add(CreateProcTable("UserModify"));

			//変更データ作成
			dsmod.Tables.Add(CreateUpdateUserData(rtnuserid));

			//webサービス実行
			DataSet rtnmodds = sc.UserSyncExecute(dsmod);
			//戻ってきた処理結果検証
			Assert.AreEqual(rtnds.Tables["ResultTable"].Rows[0]["ProcResult"], "Success");
			Assert.AreEqual(rtnds.Tables["ResultTable"].Rows[0]["ProcMessage"], "正常終了");

			//登録ユーザーIDでDB検索
			DataTable getTab = new DataTable("gettab");
			string sqlStr = " SELECT * FROM W2_USER WHERE user_id = @user_id ";
			string connStr = WebConfigurationManager.ConnectionStrings["w2Database"].ToString();
			using (SqlConnection conn = new SqlConnection(connStr))
			{
				SqlCommand cmd = new SqlCommand(sqlStr, conn);
				cmd.Parameters.AddWithValue("@user_id", rtnuserid);

				SqlDataAdapter adp = new SqlDataAdapter(cmd);

				cmd.Connection.Open();

				adp.Fill(getTab);

				cmd.Connection.Close();

			}

			//取得データと比較
			foreach (DataColumn dc in dsmod.Tables["UserTable"].Columns)
			{
				string colname = dc.ColumnName;

				string targetval = dsmod.Tables["UserTable"].Rows[0][colname].ToString();
				string regival = getTab.Rows[0][colname].ToString();

				if (colname != "user_id" && colname != "password")
				{
					Assert.AreEqual(targetval, regival);

				}

			}
		
		}

		[TestMethod]
		public void 会員退会のテスト()
		{
			//引数データセット作成
			DataSet ds = new DataSet("TestDs");

			//処理区分データテーブルセット
			ds.Tables.Add(CreateProcTable("UserRegist"));

			//先に登録しておく

			//登録データ作成
			ds.Tables.Add(CreateInsertUserData());

			//webサービス実行
			WebRef.Iw2ServiceClient sc = new WebRef.Iw2ServiceClient();
			DataSet rtnds = sc.UserSyncExecute(ds);
			//戻ってきた登録ユーザID
			string rtnuserid = (string)rtnds.Tables["UserIDTable"].Rows[0]["UserID"];

			//登録したデータを対象に変更
			//引数データセット作成
			DataSet dsmod = new DataSet("TestModDs");

			//処理区分データテーブルセット
			dsmod.Tables.Add(CreateProcTable("UserModify"));

			//退会データ作成
			dsmod.Tables.Add(CreateUpdateUserData(rtnuserid));

			//webサービス実行
			DataSet rtnmodds = sc.UserSyncExecute(dsmod);
			//戻ってきた処理結果検証
			Assert.AreEqual(rtnds.Tables["ResultTable"].Rows[0]["ProcResult"], "Success");
			Assert.AreEqual(rtnds.Tables["ResultTable"].Rows[0]["ProcMessage"], "正常終了");

			//登録ユーザーIDでDB検索
			DataTable getTab = new DataTable("gettab");
			string sqlStr = " SELECT * FROM W2_USER WHERE user_id = @user_id ";
			string connStr = WebConfigurationManager.ConnectionStrings["w2Database"].ToString();
			using (SqlConnection conn = new SqlConnection(connStr))
			{
				SqlCommand cmd = new SqlCommand(sqlStr, conn);
				cmd.Parameters.AddWithValue("@user_id", rtnuserid);

				SqlDataAdapter adp = new SqlDataAdapter(cmd);

				cmd.Connection.Open();

				adp.Fill(getTab);

				cmd.Connection.Close();

			}

			//取得データの中身チェック
			Assert.AreEqual(getdtval(getTab, "name"), "********");
			Assert.AreEqual(getdtval(getTab, "user_id"), rtnuserid);
			Assert.AreEqual(getdtval(getTab, "name1"), "****");
			Assert.AreEqual(getdtval(getTab, "name2"), "****");
			Assert.AreEqual(getdtval(getTab, "name_kana"), "********");
			Assert.AreEqual(getdtval(getTab, "name_kana1"), "****");
			Assert.AreEqual(getdtval(getTab, "name_kana2"), "****");
			Assert.AreEqual(getdtval(getTab, "nick_name"), "********");

			Assert.AreEqual(getdtval(getTab, "mail_addr"), "********");
			Assert.AreEqual(getdtval(getTab, "mail_addr2"), "********");
			Assert.AreEqual(getdtval(getTab, "addr"), "********");
			Assert.AreEqual(getdtval(getTab, "nick_name"), "********");
			Assert.AreEqual(getdtval(getTab, "nick_name"), "********");
			Assert.AreEqual(getdtval(getTab, "nick_name"), "********");
			Assert.AreEqual(getdtval(getTab, "nick_name"), "********");
			Assert.AreEqual(getdtval(getTab, "nick_name"), "********");
			Assert.AreEqual(getdtval(getTab, "nick_name"), "********");
			Assert.AreEqual(getdtval(getTab, "nick_name"), "********");
			Assert.AreEqual(getdtval(getTab, "nick_name"), "********");
			Assert.AreEqual(getdtval(getTab, "nick_name"), "********");
			Assert.AreEqual(getdtval(getTab, "nick_name"), "********");
		}

		string getdtval(DataTable dt, string colname)
		{
			return dt.Rows[0][colname].ToString();
		}


		#region 処理区分データテーブル作成
		/// <summary>
		/// 処理区分用データテーブル作成
		/// </summary>
		/// <param name="procType">設定する処理区分の値</param>
		/// <returns>作成した処理区分用データテーブル</returns>
		private DataTable CreateProcTable(string procType)
		{
			DataTable dt = new DataTable("ProcTable");
			dt.Columns.Add("ProcType");

			DataRow dr = dt.NewRow();
			dr[0] = procType;

			dt.Rows.Add(dr);

			return dt;
		}
		#endregion

		#region 登録用ユーザーデータ作成
		/// <summary>
		/// テスト用の登録用ユーザーデータ作成
		/// </summary>
		/// <returns></returns>
		private DataTable CreateInsertUserData()
		{
			DataTable dt = new DataTable("UserTable");

			dt.Columns.Add("user_id");
			dt.Columns.Add("user_kbn");
			dt.Columns.Add("mall_id");
			dt.Columns.Add("name");
			dt.Columns.Add("name1");
			dt.Columns.Add("name2");
			dt.Columns.Add("name_kana");
			dt.Columns.Add("name_kana1");
			dt.Columns.Add("name_kana2");
			dt.Columns.Add("nick_name");
			dt.Columns.Add("mail_addr");
			dt.Columns.Add("mail_addr2");
			dt.Columns.Add("zip");
			dt.Columns.Add("zip1");
			dt.Columns.Add("zip2");
			dt.Columns.Add("addr");
			dt.Columns.Add("addr1");
			dt.Columns.Add("addr2");
			dt.Columns.Add("addr3");
			dt.Columns.Add("addr4");
			dt.Columns.Add("tel1");
			dt.Columns.Add("tel1_1");
			dt.Columns.Add("tel1_2");
			dt.Columns.Add("tel1_3");
			dt.Columns.Add("sex");
			dt.Columns.Add("birth");
			dt.Columns.Add("birth_year");
			dt.Columns.Add("birth_month");
			dt.Columns.Add("birth_day");
			dt.Columns.Add("company_name");
			dt.Columns.Add("company_post_name");
			dt.Columns.Add("advcode_first");
			dt.Columns.Add("attribute1");
			dt.Columns.Add("login_id");
			dt.Columns.Add("password");
			dt.Columns.Add("question");
			dt.Columns.Add("answer");
			dt.Columns.Add("career_id");
			dt.Columns.Add("mobile_uid");
			dt.Columns.Add("remote_addr");
			dt.Columns.Add("mail_flg");
			dt.Columns.Add("user_memo");
			dt.Columns.Add("del_flg");
			dt.Columns.Add("last_changed");
			dt.Columns.Add("member_rank_id");

			DataRow dr = dt.NewRow();

			dr["user_id"] = "";
			dr["user_kbn"] = "PC_USER";
			dr["mall_id"] = "OWN_SITE";
			dr["name"] = "うぇぶてすと";
			dr["name1"] = "うぇぶ";
			dr["name2"] = "てすと";
			dr["name_kana"] = "ウェブテスト";
			dr["name_kana1"] = "ウェブ";
			dr["name_kana2"] = "テスト";
			dr["nick_name"] = "うぇぶてす";
			dr["mail_addr"] = "f.nagaki@w2solution.co.jp";
			dr["mail_addr2"] = "";
			dr["zip"] = "123-4567";
			dr["zip1"] = "123";
			dr["zip2"] = "4567";
			dr["addr"] = "テスト住所1住所2住所3住所4";
			dr["addr1"] = "住所1";
			dr["addr2"] = "住所2";
			dr["addr3"] = "住所3";
			dr["addr4"] = "住所4";
			dr["tel1"] = "0000-00-0000";
			dr["tel1_1"] = "0000";
			dr["tel1_2"] = "00";
			dr["tel1_3"] = "0000";
			dr["sex"] = "MALE";
			dr["birth"] = Convert.ToDateTime("1980/02/17");
			dr["birth_year"] = "1980";
			dr["birth_month"] = "2";
			dr["birth_day"] = "17";
			dr["company_name"] = "企業テスト1";
			dr["company_post_name"] = "企業部署1";
			dr["advcode_first"] = "";
			dr["attribute1"] = "ic0001";
			dr["login_id"] = DateTime.Now.ToString("yyyyMMddhhmmss");
			dr["password"] = "pass123456";
			dr["question"] = "質問1";
			dr["answer"] = "答え1";
			dr["career_id"] = "";
			dr["mobile_uid"] = "";
			dr["remote_addr"] = "127.0.0.1";
			dr["mail_flg"] = "ON";
			dr["user_memo"] = "特記事項";
			dr["del_flg"] = "0";
			dr["last_changed"] = "webservice";
			dr["member_rank_id"] = "";

			dt.Rows.Add(dr);

			return dt;

		}

		#endregion

		#region 更新用ユーザーデータ作成

		/// <summary>
		/// テスト用の更新用ユーザーデータ作成
		/// </summary>
		/// <returns></returns>
		private DataTable CreateUpdateUserData(string userID)
		{
			DataTable dt = new DataTable("UserTable");

			dt.Columns.Add("user_id");
			dt.Columns.Add("user_kbn");
			dt.Columns.Add("mall_id");
			dt.Columns.Add("name");
			dt.Columns.Add("name1");
			dt.Columns.Add("name2");
			dt.Columns.Add("name_kana");
			dt.Columns.Add("name_kana1");
			dt.Columns.Add("name_kana2");
			dt.Columns.Add("nick_name");
			dt.Columns.Add("mail_addr");
			dt.Columns.Add("mail_addr2");
			dt.Columns.Add("zip");
			dt.Columns.Add("zip1");
			dt.Columns.Add("zip2");
			dt.Columns.Add("addr");
			dt.Columns.Add("addr1");
			dt.Columns.Add("addr2");
			dt.Columns.Add("addr3");
			dt.Columns.Add("addr4");
			dt.Columns.Add("tel1");
			dt.Columns.Add("tel1_1");
			dt.Columns.Add("tel1_2");
			dt.Columns.Add("tel1_3");
			dt.Columns.Add("sex");
			dt.Columns.Add("birth");
			dt.Columns.Add("birth_year");
			dt.Columns.Add("birth_month");
			dt.Columns.Add("birth_day");
			dt.Columns.Add("company_name");
			dt.Columns.Add("company_post_name");
			dt.Columns.Add("advcode_first");
			dt.Columns.Add("attribute1");
			dt.Columns.Add("login_id");
			dt.Columns.Add("password");
			dt.Columns.Add("question");
			dt.Columns.Add("answer");
			dt.Columns.Add("career_id");
			dt.Columns.Add("mobile_uid");
			dt.Columns.Add("remote_addr");
			dt.Columns.Add("mail_flg");
			dt.Columns.Add("user_memo");
			dt.Columns.Add("del_flg");
			dt.Columns.Add("last_changed");
			dt.Columns.Add("member_rank_id");

			DataRow dr = dt.NewRow();

			dr["user_id"] = userID;
			dr["user_kbn"] = "PC_USER";
			dr["mall_id"] = "OWN_SITE";
			dr["name"] = "うぶてと";
			dr["name1"] = "うぶ";
			dr["name2"] = "てと";
			dr["name_kana"] = "ウブテト";
			dr["name_kana1"] = "ウブ";
			dr["name_kana2"] = "テト";
			dr["nick_name"] = "うぶす";
			dr["mail_addr"] = "f.nagaki@w2solution.co.jp_X";
			dr["mail_addr2"] = "";
			dr["zip"] = "321-7654";
			dr["zip1"] = "321";
			dr["zip2"] = "7654";
			dr["addr"] = "テスト住所11住所21住所31住所41";
			dr["addr1"] = "住所11";
			dr["addr2"] = "住所21";
			dr["addr3"] = "住所31";
			dr["addr4"] = "住所41";
			dr["tel1"] = "1111-11-1111";
			dr["tel1_1"] = "1111";
			dr["tel1_2"] = "11";
			dr["tel1_3"] = "1111";
			dr["sex"] = "UNKNOWN";
			dr["birth"] = Convert.ToDateTime("1983/03/03");
			dr["birth_year"] = "1983";
			dr["birth_month"] = "3";
			dr["birth_day"] = "3";
			dr["company_name"] = "企業テスト8";
			dr["company_post_name"] = "企業部署8";
			dr["advcode_first"] = "";
			dr["attribute1"] = "ic0001";
			dr["login_id"] = "testloginx";
			dr["password"] = "pass654321";
			dr["question"] = "質問2";
			dr["answer"] = "答え4";
			dr["career_id"] = "";
			dr["mobile_uid"] = "";
			dr["remote_addr"] = "127.0.0.2";
			dr["mail_flg"] = "OFF";
			dr["user_memo"] = "特記事項変更！！";
			dr["del_flg"] = "0";
			dr["last_changed"] = "webmod";
			dr["member_rank_id"] = "";

			dt.Rows.Add(dr);

			return dt;

		}

		#endregion
		
	}
}
