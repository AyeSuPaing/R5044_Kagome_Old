using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using w2.App.Common.Web.Page;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Logger;

/// <summary>
/// ZeusLinkPointReceiver の概要の説明です
/// </summary>
public abstract class ZeusLinkPointReceiverBase : CommonPage
{
	/// <summary>決済タイプ</summary>
	public enum PaymentTypes
	{
		/// <summary>与信</summary>
		Auth,
		/// <summary>登録</summary>
		Register,
	}

	/// <summary>ZEUS決済ページディレクトリパス</summary>
	public const string PAYMENT_PAGE_DIRPATH = "/Payment/CreditCardZeus/";
	/// <summary>ZEUS決済通知ページ名（与信）</summary>
	public const string PAYMENT_NOTICE_PAGENAME_AUTH = "ZeusCreditCardPaymentReceiveOrderNotice.aspx";
	/// <summary>ZEUS決済通知ページ名（登録）</summary>
	public const string PAYMENT_NOTICE_PAGENAME_REGISTER = "ZeusCreditCardRegisterReceiveOrderNotice.aspx";

	#region WrappedControls
	/// <summary>通知URL</summary>
	private WrappedTextBox WtbOrderNoticeUrl { get { return GetWrappedControl<WrappedTextBox>(this.Form, "tbOrderNoticeUrl"); } }
	/// <summary>通知URLをhttpに変えてつうちするか</summary>
	private WrappedCheckBox WcbChangeOrderNoticeUrlHttp { get { return GetWrappedControl<WrappedCheckBox>(this.Form, "cbChangeOrderNoticeUrlHttp"); } }
	/// <summary>通知結果メッセージ</summary>
	private WrappedLiteral WlbtnSendOrderNoticeResultMessage { get { return GetWrappedControl<WrappedLiteral>(this.Form, "lbtnSendOrderNoticeResultMessage"); } }
	#endregion

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="paymentType">決済タイプ</param>
	protected ZeusLinkPointReceiverBase(PaymentTypes paymentType)
	{
		this.PaymentType = paymentType;
	}

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			DataBind();
		}
	}

	/// <summary>
	/// 結果通知送信＆結果表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendOrderNotice_Click(object sender, EventArgs e)
	{
		var message = SendOrderNotice(this.WtbOrderNoticeUrl.Text);
		this.WlbtnSendOrderNoticeResultMessage.Text = "[" + message + "]";
	}

	/// <summary>
	/// 決済通知送信
	/// </summary>
	/// <param name="url">通知先URL</param>
	/// <returns>通知結果</returns>
	protected string SendOrderNotice(string url)
	{
		var request = WebRequest.Create(url);
		request.Method = "GET";
		request.ContentType = "application/x-www-form-urlencoded";

		try
		{
			using (var responseStream = request.GetResponse().GetResponseStream())
			using (var reader = new StreamReader(responseStream, Encoding.GetEncoding("Shift_JIS")))
			{
				var message = reader.ReadToEnd();
				return message;
			}
		}
		catch (Exception ex)
		{
			FileLogger.WriteError(ex);
			return "response error...";
		}
	}

	/// <summary>
	/// 通知URL作成
	/// </summary>
	/// <returns>通知URL</returns>
	protected string CreateNoticeUrl()
	{
		var paramStrng = string.Join(
			"&",
			new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("result", "OK"),
				new KeyValuePair<string, string>("clientip", this.Request.Form["clientip"]),
				new KeyValuePair<string, string>("ordd", this.Ordd),
				new KeyValuePair<string, string>("money", this.Request.Form["money"]),
				new KeyValuePair<string, string>("telno", this.Request.Form["telno"]),
				new KeyValuePair<string, string>("email", this.Request.Form["email"]),
				new KeyValuePair<string, string>("sendid", this.Request.Form["sendid"]),
				new KeyValuePair<string, string>("sendpoint", this.Request.Form["sendpoint"]),
				new KeyValuePair<string, string>("cardnumber", "0124"),
				new KeyValuePair<string, string>("cardbrand", "DMY"),
				new KeyValuePair<string, string>("yuko", "0102"),
				new KeyValuePair<string, string>("div", "01")
			}.Select(p => string.Format("{0}={1}", p.Key, HttpUtility.UrlEncode(p.Value))));

		// 成功URLから通知ページを予測
		var successUrl = Request.Form["success_url"];
		var splitedSuccessUrl = successUrl.Split(new[] { PAYMENT_PAGE_DIRPATH }, StringSplitOptions.RemoveEmptyEntries);
		var urlHead = Regex.Replace(splitedSuccessUrl[0], @"/\(S\(.*\)\)$", "", RegexOptions.IgnoreCase);
		// パラメータ削る
		urlHead = urlHead.Split('?')[0];
		var noticeUrl = urlHead + (urlHead.Contains("?") ? "&" : "?") + paramStrng;
		if (this.WcbChangeOrderNoticeUrlHttp.Checked)
		{
			return noticeUrl.Replace("https:", "http:");
		}
		return noticeUrl;
	}

	/// <summary>決済タイプ</summary>
	public PaymentTypes PaymentType { get; set; }
	/// <summary>オーダNO</summary>
	public string Ordd
	{
		get
		{
			var id = (this.Request.Form["sendpoint"] ?? ":").Split(':')[1];
			return ((string.IsNullOrEmpty(id) == false) ? string.Format("TEST-{0}", id) : "");
		}
	}
}