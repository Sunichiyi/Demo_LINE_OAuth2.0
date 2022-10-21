Imports Demo_LINE_OAuth2._0.CommonUtility.General
Public Class login
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'https://access.line.me/oauth2/v2.1/authorize?response_type=code&client_id=1234567890&redirect_uri=https%3A%2F%2Fexample.com%2Fauth%3Fkey%3Dvalue&state=12345abcde&scope=profile%20openid&nonce=09876xyz
        Dim response_type = "response_type=" + HttpUtility.UrlEncode("code")
        Dim client_id = "client_id=" + HttpUtility.UrlEncode(GetAppSetting("client_id"))
        Dim redirect_uri = "redirect_uri=" + HttpUtility.UrlEncode(GetAppSetting("redirect_uri"))
        Dim state = "state=" + HttpUtility.UrlEncode(GetAppSetting("state"))
        Dim scope = "scope=" + HttpUtility.UrlEncode(GetAppSetting("scope"))
        Dim nonce = "nonce=" + HttpUtility.UrlEncode(GetAppSetting("nonce"))
        Dim sep = "&"
        Dim sb As New StringBuilder()
        sb.Append(response_type).Append(sep).Append(client_id).Append(sep)
        sb.Append(redirect_uri).Append(sep).Append(state).Append(sep).Append(scope).Append(sep).Append(nonce)

        Response.Redirect("https://access.line.me/oauth2/v2.1/authorize?" + sb.ToString())
    End Sub

End Class