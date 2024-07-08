using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Security.Cryptography;
using w2.Common.Util;
using w2.Common.Util.Security;
using w2.Common.Web;

public partial class SBPS_TEST_SBPS_ReceiveMultiPaymentOrder : System.Web.UI.Page
{
	/// <summary>チェックサム用バッファ</summary>
	private StringBuilder m_checkSumBuffer = new StringBuilder();
	/// <summary>ハッシュキー</summary>
	string m_hashKey = Constants.PAYMENT_SETTING_SBPS_HASHKEY;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.HashKey = m_hashKey;
		if (!IsPostBack)
		{
			m_form = Request.Form;
			tbUrl.Text = GetUrl(m_form["pagecon_url"]);

			divButtonsForCredit3DSecure.Visible = (m_form["pay_method"] == "credit3d");
			divButtonsForCreditEMV3DSecure.Visible = (m_form["pay_method"] == "credit3d2");
			divButtonsForOthers.Visible =
				this.m_form["pay_method"] != "credit3d" && this.m_form["pay_method"] != "credit3d2";
		}
	}

	/// <summary>
	/// フォームinput作成
	/// </summary>
	/// <param name="hashEncoding">ハッシュエンコーディング</param>
	/// <param name="errorCode">エラーコード</param>
	/// <returns></returns>
	protected string CreateFormInputs(Encoding hashEncoding, string errorCode = "")
	{
		List<KeyValuePair<string, string>> datas = CreateParamsBase();
		datas.Add(new KeyValuePair<string, string>("request_date", AddToCheckSumBuffer(m_form["request_date"])));
		datas.Add(new KeyValuePair<string, string>("res_pay_method", AddToCheckSumBuffer(m_form["pay_method"])));
		datas.Add(new KeyValuePair<string, string>("res_result", AddToCheckSumBuffer("OK")));
		datas.Add(new KeyValuePair<string, string>("res_tracking_id", AddToCheckSumBuffer("123456789")));
		datas.Add(new KeyValuePair<string, string>("res_sps_cust_no", AddToCheckSumBuffer("987654321")));
		datas.Add(new KeyValuePair<string, string>("res_sps_payment_no", AddToCheckSumBuffer("KKKK")));
		datas.Add(new KeyValuePair<string, string>("res_payinfo_key", AddToCheckSumBuffer("KESSAI")));
		datas.Add(new KeyValuePair<string, string>("res_payment_date", AddToCheckSumBuffer(DateTime.Now.ToString("yyyyMMddHHmmss"))));
		datas.Add(new KeyValuePair<string, string>("res_err_code", AddToCheckSumBuffer(errorCode)));
		datas.Add(new KeyValuePair<string, string>("res_date", AddToCheckSumBuffer(DateTime.Now.ToString("yyyyMMddHHmmss"))));
		datas.Add(new KeyValuePair<string, string>("limit_second", AddToCheckSumBuffer(m_form["limit_second"])));
		datas.Add(new KeyValuePair<string, string>("sps_hashcode", ComputeHashSHA1AndClearBuffer(hashEncoding)));
		StringBuilder form = new StringBuilder();
		foreach (var kv in datas)
		{
			form.Append("<input type=\"hidden\" name=\"").Append(kv.Key).Append("\" value=\"").Append(HtmlSanitizer.HtmlEncode(kv.Value)).Append("\">\r\n");
		}

		return form.ToString();
	}

	/// <summary>
	/// 結果通知送信＆結果表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendOrderNotice_Click(object sender, EventArgs e)
	{
		List<KeyValuePair<string, string>> datas = CreateParamsBase();
		datas.Add(new KeyValuePair<string, string>("request_date", AddToCheckSumBuffer(m_form["request_date"])));
		datas.Add(new KeyValuePair<string, string>("res_pay_method", AddToCheckSumBuffer(m_form["pay_method"])));
		datas.Add(new KeyValuePair<string, string>("res_result", AddToCheckSumBuffer("OK")));
		datas.Add(new KeyValuePair<string, string>("res_tracking_id", AddToCheckSumBuffer("123456789")));
		datas.Add(new KeyValuePair<string, string>("res_sps_cust_no", AddToCheckSumBuffer("987654321")));
		datas.Add(new KeyValuePair<string, string>("res_sps_payment_no", AddToCheckSumBuffer("KKKK")));
		datas.Add(new KeyValuePair<string, string>("res_payinfo_key", AddToCheckSumBuffer("KESSAI")));
		datas.Add(new KeyValuePair<string, string>("res_payment_date", AddToCheckSumBuffer(DateTime.Now.ToString("yyyyMMddHHmmss"))));
		datas.Add(new KeyValuePair<string, string>("res_err_code", AddToCheckSumBuffer("")));
		datas.Add(new KeyValuePair<string, string>("res_date", AddToCheckSumBuffer(DateTime.Now.ToString("yyyyMMddHHmmss"))));
		datas.Add(new KeyValuePair<string, string>("limit_second", AddToCheckSumBuffer(m_form["limit_second"])));
		datas.Add(new KeyValuePair<string, string>("sps_hashcode", ComputeHashSHA1AndClearBuffer(Encoding.UTF8)));

		//------------------------------------------------------
		// POST送信用パラメタをバイト列へ変換
		//------------------------------------------------------
		Encoding eEndoding = Encoding.GetEncoding("Shift_JIS");
		StringBuilder sbParams = new StringBuilder();
		foreach (KeyValuePair<string, string> kvpParam in datas)
		{
			if (sbParams.Length != 0)
			{
				sbParams.Append("&");
			}
			sbParams.Append(kvpParam.Key).Append("=").Append(HttpUtility.UrlEncode(kvpParam.Value, eEndoding));
		}
		byte[] bParamDatas = Encoding.ASCII.GetBytes(sbParams.ToString());

		//------------------------------------------------------
		// POST送信・レスポンス受信
		//------------------------------------------------------
		var response = SendOrderNotice(bParamDatas, eEndoding);
		lSendOrderNoticeMessage.Text = "[ " + response + " ]";
	}

	/// <summary>
	/// 決済通知送信
	/// </summary>
	/// <param name="bParamDatas">パラメタ</param>
	/// <param name="eEndoding">エンコーディング</param>
	/// <returns>通知結果</returns>
	private string SendOrderNotice(byte[] bParamDatas, Encoding eEndoding)
	{
		HttpWebRequest wrRequest = (HttpWebRequest)WebRequest.Create(tbUrl.Text);
		// メソッドにPOSTを指定
		wrRequest.Method = "POST";
		// ContentTypeを"application/x-www-form-urlencoded"にする
		wrRequest.ContentType = "application/x-www-form-urlencoded";
		// POST送信するデータの長さを指定
		wrRequest.ContentLength = bParamDatas.Length;
		// ユーザーエージェントを指定
		wrRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.108 Safari/537.36";

		// データをPOST送信（書き込み）
		using (Stream reqStream = wrRequest.GetRequestStream())
		{
			reqStream.Write(bParamDatas, 0, bParamDatas.Length);
		}

		// レスポンス受信
		try
		{
			using (var res = wrRequest.GetResponse())
			using (var resStream = res.GetResponseStream())
			using (var sr = new StreamReader(resStream, eEndoding))
			{
				var response = sr.ReadToEnd();
				return response;
			}
		}
		catch (Exception ex)
		{
			throw new Exception("レスポンスが取得できませんでした：" + tbUrl.Text);
		}
	}

	/// <summary>
	/// パラメタ作成
	/// </summary>
	/// <returns></returns>
	private List<KeyValuePair<string, string>> CreateParamsBase()
	{
		List<KeyValuePair<string, string>> datas = new List<KeyValuePair<string, string>>();
		datas.Add(new KeyValuePair<string, string>("pay_method", AddToCheckSumBuffer(m_form["pay_method"])));
		datas.Add(new KeyValuePair<string, string>("merchant_id", AddToCheckSumBuffer(m_form["merchant_id"])));
		datas.Add(new KeyValuePair<string, string>("service_id", AddToCheckSumBuffer(m_form["service_id"]))); 
		datas.Add(new KeyValuePair<string, string>("cust_code", AddToCheckSumBuffer(m_form["cust_code"])));
		//datas.Add(new KeyValuePair<string, string>("sps_cust_no", AddToCheckSumBuffer("")));		// 当連携モデルの場合は未設定（空文字）
		//datas.Add(new KeyValuePair<string, string>("sps_payment_no", AddToCheckSumBuffer("")));	// 当連携モデルの場合は未設定（空文字）
		datas.Add(new KeyValuePair<string, string>("order_id", AddToCheckSumBuffer(m_form["order_id"])));
		datas.Add(new KeyValuePair<string, string>("item_id", AddToCheckSumBuffer(m_form["item_id"])));
		//datas.Add(new KeyValuePair<string, string>("pay_item_id", AddToCheckSumBuffer("")));		// 当連携モデルの場合は未設定（空文字）
		datas.Add(new KeyValuePair<string, string>("item_name", AddToCheckSumBuffer(m_form["item_name"])));
		//datas.Add(new KeyValuePair<string, string>("tax", AddToCheckSumBuffer("")));
		datas.Add(new KeyValuePair<string, string>("amount", AddToCheckSumBuffer(m_form["amount"])));
		datas.Add(new KeyValuePair<string, string>("pay_type", AddToCheckSumBuffer(m_form["pay_type"])));	// 0:都度購入／ 1:継続課金（簡易）／ 2:継続課金（定期・従量）
		if (m_form["auto_charge_type"] != null)
		{
			datas.Add(new KeyValuePair<string, string>("auto_charge_type", AddToCheckSumBuffer(m_form["auto_charge_type"])));
		}
		if (m_form["div_settele"] != null)
		{
			datas.Add(new KeyValuePair<string, string>("div_settele", AddToCheckSumBuffer(m_form["div_settele"])));		// 0:前払い（継続課金のみ）
		}
		datas.Add(new KeyValuePair<string, string>("service_type", AddToCheckSumBuffer(m_form["service_type"])));		// 0:　売上（購入）／ 1:取消（月額課金解約）
		//datas.Add(new KeyValuePair<string, string>("last_charge_month", AddToCheckSumBuffer("")));	// 継続課金のみ
		//datas.Add(new KeyValuePair<string, string>("camp_type", AddToCheckSumBuffer("")));			// 継続課金のみ
		datas.Add(new KeyValuePair<string, string>("tracking_id", AddToCheckSumBuffer(m_form["tracking_id"])));		// 取り消しのみ
		datas.Add(new KeyValuePair<string, string>("terminal_type", AddToCheckSumBuffer(m_form["terminal_type"])));
		datas.Add(new KeyValuePair<string, string>("free1", AddToCheckSumBuffer("")));
		datas.Add(new KeyValuePair<string, string>("free2", AddToCheckSumBuffer("")));
		datas.Add(new KeyValuePair<string, string>("free3", AddToCheckSumBuffer("")));

		return datas;
	}

	/// <summary>
	/// チェックサムバッファへ追加しつつ同じものを戻す
	/// </summary>
	/// <param name="value">値</param>
	/// <returns>値</returns>
	protected string AddToCheckSumBuffer(string value)
	{
		m_checkSumBuffer.Append(StringUtility.ToEmpty(value).Trim());

		return value;
	}

	/// <summary>
	/// バッファをSHA1ハッシュ計算して返す（バッファクリアもする）
	/// </summary>
	/// <returns>ハッシュ値</returns>
	protected string ComputeHashSHA1AndClearBuffer(Encoding encoding)
	{
		string hashString = ComputeHashSHA1(m_checkSumBuffer.ToString() + this.HashKey, encoding);

		m_checkSumBuffer.Clear();

		return hashString;
	}
	/// <summary>
	/// SHA1ハッシュ計算
	/// </summary>
	/// <returns>ハッシュ値</returns>
	protected string ComputeHashSHA1(string value, Encoding encoding)
	{
		SHA1Managed calculator = new SHA1Managed();
		byte[] hash = calculator.ComputeHash(encoding.GetBytes(value));

		StringBuilder hashString = new StringBuilder();
		foreach (byte b in hash)
		{
			hashString.Append(b.ToString("x2"));
		}
		return hashString.ToString();
	}

	/// <summary>
	/// URL取得
	/// </summary>
	/// <param name="src"></param>
	/// <returns></returns>
	protected string GetUrl(string url)
	{
		if (cbChangeHttp.Checked)
		{
			return StringUtility.ToEmpty(url).Replace("https:", "http:");
		}

		return url;
	}

	/// <summary>
	/// 楽天ペイ金額変更通知送信
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void bRakutenAmountChange_Click(object sender, EventArgs e)
	{
		List<KeyValuePair<string, string>> datas = CreateParamsBase();
		datas.Add(new KeyValuePair<string, string>("request_date", AddToCheckSumBuffer(DateTime.Now.ToString("yyyyMMddHHmmss"))));
		datas.Add(new KeyValuePair<string, string>("res_pay_method", AddToCheckSumBuffer("rakuten")));
		datas.Add(new KeyValuePair<string, string>("res_result", AddToCheckSumBuffer("OK")));
		datas.Add(new KeyValuePair<string, string>("res_tracking_id", AddToCheckSumBuffer("123456789")));
		datas.Add(new KeyValuePair<string, string>("res_sps_cust_no", AddToCheckSumBuffer("987654321")));
		datas.Add(new KeyValuePair<string, string>("res_sps_payment_no", AddToCheckSumBuffer("KKKK")));
		datas.Add(new KeyValuePair<string, string>("res_payinfo_key", AddToCheckSumBuffer("R02")));
		datas.Add(new KeyValuePair<string, string>("res_payment_date", AddToCheckSumBuffer(DateTime.Now.ToString("yyyyMMddHHmmss"))));
		datas.Add(new KeyValuePair<string, string>("res_err_code", AddToCheckSumBuffer("")));
		datas.Add(new KeyValuePair<string, string>("res_date", AddToCheckSumBuffer(DateTime.Now.ToString("yyyyMMddHHmmss"))));
		datas.Add(new KeyValuePair<string, string>("limit_second", AddToCheckSumBuffer("600")));
		datas.Add(new KeyValuePair<string, string>("sps_hashcode", ComputeHashSHA1AndClearBuffer(Encoding.UTF8)));

		//------------------------------------------------------
		// POST送信用パラメタをバイト列へ変換
		//------------------------------------------------------
		Encoding eEndoding = Encoding.GetEncoding("Shift_JIS");
		StringBuilder sbParams = new StringBuilder();
		foreach (KeyValuePair<string, string> kvpParam in datas)
		{
			if (sbParams.Length != 0)
			{
				sbParams.Append("&");
			}
			sbParams.Append(kvpParam.Key).Append("=").Append(HttpUtility.UrlEncode(kvpParam.Value, eEndoding));
		}
		byte[] bParamDatas = Encoding.ASCII.GetBytes(sbParams.ToString());

		//------------------------------------------------------
		// POST送信・レスポンス受信
		//------------------------------------------------------
		var response = SendOrderNotice(bParamDatas, eEndoding);
		lSendOrderNoticeMessage.Text = "[ " + response + " ]";
	}

	/// <summary>
	/// 楽天ペイ売上結果通知送信
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void bRakutenSalesComplete_Click(object sender, EventArgs e)
	{
		List<KeyValuePair<string, string>> datas = CreateParamsBase();
		datas.Add(new KeyValuePair<string, string>("request_date", AddToCheckSumBuffer(DateTime.Now.ToString("yyyyMMddHHmmss"))));
		datas.Add(new KeyValuePair<string, string>("res_pay_method", AddToCheckSumBuffer("rakuten")));
		datas.Add(new KeyValuePair<string, string>("res_result", AddToCheckSumBuffer("OK")));
		datas.Add(new KeyValuePair<string, string>("res_tracking_id", AddToCheckSumBuffer("123456789")));
		datas.Add(new KeyValuePair<string, string>("res_sps_cust_no", AddToCheckSumBuffer("987654321")));
		datas.Add(new KeyValuePair<string, string>("res_sps_payment_no", AddToCheckSumBuffer("KKKK")));
		datas.Add(new KeyValuePair<string, string>("res_payinfo_key", AddToCheckSumBuffer("R03")));
		datas.Add(new KeyValuePair<string, string>("res_payment_date", AddToCheckSumBuffer(DateTime.Now.ToString("yyyyMMddHHmmss"))));
		datas.Add(new KeyValuePair<string, string>("res_err_code", AddToCheckSumBuffer("")));
		datas.Add(new KeyValuePair<string, string>("res_date", AddToCheckSumBuffer(DateTime.Now.ToString("yyyyMMddHHmmss"))));
		datas.Add(new KeyValuePair<string, string>("limit_second", AddToCheckSumBuffer("600")));
		datas.Add(new KeyValuePair<string, string>("sps_hashcode", ComputeHashSHA1AndClearBuffer(Encoding.UTF8)));

		//------------------------------------------------------
		// POST送信用パラメタをバイト列へ変換
		//------------------------------------------------------
		Encoding eEndoding = Encoding.GetEncoding("Shift_JIS");
		StringBuilder sbParams = new StringBuilder();
		foreach (KeyValuePair<string, string> kvpParam in datas)
		{
			if (sbParams.Length != 0)
			{
				sbParams.Append("&");
			}
			sbParams.Append(kvpParam.Key).Append("=").Append(HttpUtility.UrlEncode(kvpParam.Value, eEndoding));
		}
		byte[] bParamDatas = Encoding.ASCII.GetBytes(sbParams.ToString());

		//------------------------------------------------------
		// POST送信・レスポンス受信
		//------------------------------------------------------
		var response = SendOrderNotice(bParamDatas, eEndoding);
		lSendOrderNoticeMessage.Text = "[ " + response + " ]";
	}

	/// <summary>
	/// 楽天ペイ与信結果送信
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRakutenAuthorizedResult_Click(object sender, EventArgs e)
	{
		List<KeyValuePair<string, string>> datas = CreateParamsBase();
		datas.Add(new KeyValuePair<string, string>("request_date", AddToCheckSumBuffer(DateTime.Now.ToString("yyyyMMddHHmmss"))));
		datas.Add(new KeyValuePair<string, string>("res_pay_method", AddToCheckSumBuffer("rakuten")));
		datas.Add(new KeyValuePair<string, string>("res_result", AddToCheckSumBuffer("OK")));
		datas.Add(new KeyValuePair<string, string>("res_tracking_id", AddToCheckSumBuffer("123456789")));
		datas.Add(new KeyValuePair<string, string>("res_sps_cust_no", AddToCheckSumBuffer("987654321")));
		datas.Add(new KeyValuePair<string, string>("res_sps_payment_no", AddToCheckSumBuffer("KKKK")));
		datas.Add(new KeyValuePair<string, string>("res_payinfo_key", AddToCheckSumBuffer("R01")));
		datas.Add(new KeyValuePair<string, string>("res_payment_date", AddToCheckSumBuffer(DateTime.Now.ToString("yyyyMMddHHmmss"))));
		datas.Add(new KeyValuePair<string, string>("res_err_code", AddToCheckSumBuffer("")));
		datas.Add(new KeyValuePair<string, string>("res_date", AddToCheckSumBuffer(DateTime.Now.ToString("yyyyMMddHHmmss"))));
		datas.Add(new KeyValuePair<string, string>("limit_second", AddToCheckSumBuffer("600")));
		datas.Add(new KeyValuePair<string, string>("sps_hashcode", ComputeHashSHA1AndClearBuffer(Encoding.UTF8)));

		//------------------------------------------------------
		// POST送信用パラメタをバイト列へ変換
		//------------------------------------------------------
		Encoding eEndoding = Encoding.GetEncoding("Shift_JIS");
		StringBuilder sbParams = new StringBuilder();
		foreach (KeyValuePair<string, string> kvpParam in datas)
		{
			if (sbParams.Length != 0)
			{
				sbParams.Append("&");
			}
			sbParams.Append(kvpParam.Key).Append("=").Append(HttpUtility.UrlEncode(kvpParam.Value, eEndoding));
		}
		byte[] bParamDatas = Encoding.ASCII.GetBytes(sbParams.ToString());

		//------------------------------------------------------
		// POST送信・レスポンス受信
		//------------------------------------------------------
		var response = SendOrderNotice(bParamDatas, eEndoding);
		lSendOrderNoticeMessage.Text = "[ " + response + " ]";
	}

	/// <summary>マーチャントID</summary>
	protected string MerchantId { get; set; }
	/// <summary>サービスID</summary>
	protected string ServiceId { get; set; }
	/// <summary>ハッシュキー</summary>
	protected string HashKey { get; set; }
	/// <summary>リクエスト</summary>
	protected System.Collections.Specialized.NameValueCollection m_form
	{
		get { return (System.Collections.Specialized.NameValueCollection)ViewState["m_form"]; }
		set { ViewState["m_form"] = value; }
	}
}