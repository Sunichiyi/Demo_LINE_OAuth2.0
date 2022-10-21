Public Class logout
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim authCookie As HttpCookie = Request.Cookies(FormsAuthentication.FormsCookieName)
        Try
            LogoutCtrlByCookie(authCookie, False)
            Response.Redirect("default.aspx")
        Catch ex As Exception
            '
        End Try
    End Sub

    Sub LogoutCtrlByCookie(ByVal UserCookie As HttpCookie, ByVal isTransPage As Boolean)

        Dim context As HttpContext = HttpContext.Current

        If UserCookie IsNot Nothing AndAlso context.Request.Cookies(UserCookie.Name) IsNot Nothing Then
            Dim clearCookie As HttpCookie = New HttpCookie(UserCookie.Name)
            clearCookie.Expires = DateTime.Now.AddDays(-1)  '- 1 day
            clearCookie.Values.Clear()
            context.Response.Cookies.Set(clearCookie)
        End If

        context.Session.RemoveAll()
        context.Session.Abandon()
        FormsAuthentication.SignOut()

        If isTransPage Then
            FormsAuthentication.RedirectToLoginPage()
        End If

    End Sub
End Class