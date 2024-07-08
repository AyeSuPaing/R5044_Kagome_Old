using System;

public partial class CrossPoint_CrossPointAppMock : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
	{
		this.AppKey = "Test";
		this.MemberId = "071111";
		this.PinCd = "071111";
	}

	/// <summary>アプリキー</summary>
	protected string AppKey { get; set; }
	/// <summary>会員番号</summary>
	protected string MemberId { get; set; }
	/// <summary>PINコード</summary>
	protected string PinCd { get; set; }
}