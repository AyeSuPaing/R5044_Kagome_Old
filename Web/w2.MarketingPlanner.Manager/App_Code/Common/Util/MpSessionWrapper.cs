using System.Web;
using System.Web.SessionState;
using Input.Point;

/// <summary> Mpのセッション情報をラップ・扱いやすくした静的クラス </summary>
public static class MpSessionWrapper 
{
	/// <summary>セッションKEY：ポイントルールの入力値</summary>
	public const string SESSION_KEY_POINTRULE_INPUT = "w2mpMng_pointrule_input";

	/// <summary>ポイントルールの入力値</summary>
	public static PointRuleInput PointRuleInput
	{
		get { return (PointRuleInput) CurrentSession[SESSION_KEY_POINTRULE_INPUT]; }
		set { CurrentSession[SESSION_KEY_POINTRULE_INPUT] = value; }
	}

	/// <summary>現在のHttpセッション</summary>
	public static HttpSessionState CurrentSession { get { return HttpContext.Current.Session; } } 

}