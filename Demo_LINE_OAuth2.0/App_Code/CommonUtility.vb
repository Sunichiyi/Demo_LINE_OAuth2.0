Imports Oracle.ManagedDataAccess.Client
Namespace CommonUtility
    Public Class General

        Shared ReadOnly DB As New DBConn
        Public Shared ReadOnly myConn As New OracleConnection(DB.GetConn)

        '網頁路徑
        Public Shared ReadOnly LogoutPage As String = "./Logout.aspx"

        '[String] GetAppSetting : 讀取AppSettings
        Shared Function GetAppSetting(key As String) As String
            '資料來源：https://msdn.microsoft.com/zh-tw/library/system.configuration.configurationmanager.appsettings(v=vs.110).aspx
            Try
                Dim appSettings = ConfigurationManager.AppSettings
                Return If(String.IsNullOrEmpty(appSettings(key)), "Not Found", appSettings(key))
            Catch ex As ConfigurationErrorsException
                Return "Error Msg : " & ex.ToString()
            End Try
        End Function

        '[Sub] SetGUCookies : 依使用者登入ID，設定Cookie
        Public Shared Sub SetCookies(ByVal UserID As String, ByVal isCookiePersistent As Boolean, ByVal UserGroup As String)
            Dim context As HttpContext = HttpContext.Current
            ' 資料來源：http://msdn.microsoft.com/zh-tw/library/System.Web.Security.FormsAuthenticationTicket(v=vs.110).aspx
            ' 輸入參數： 使用 Cookie 名稱、版本、目錄路徑、核發日期、到期日期、永續性和使用者定義的資料
            ' 此 Cookie 路徑設定為在Web.Config組態檔中建立的預設值，也就是  path="/"。
            Dim authTicket As New FormsAuthenticationTicket(1, UserID, DateTime.Now, DateTime.Now.AddMinutes(60), isCookiePersistent, UserGroup)
            '-- 每個參數的用意 ---------------------------------------------------------------------------------------------
            '--  version  類型：System.Int32  票證(ticket)的版本號碼。
            '--  name  類型：System.String  與票證相關的使用者名稱。
            '--  issueDate  類型：System.DateTime  核發此票證時的本機日期和時間。
            '--  expiration  類型：System.DateTime  票證到期的本機日期和時間。
            '--  isPersistent  類型：System.Boolean  如果票證將存放於持續性 Cookie 中 (跨瀏覽器工作階段儲存)，則為 true，否則為 false。 
            '                                                                   如果票證是存放於 URL 中，則忽略這個值。 
            '--  userData  類型：System.String  要與票證一起存放的使用者特定資料。

            'Encrypt the ticket. 加密，以策安全！
            Dim encryptedTicket As String = FormsAuthentication.Encrypt(authTicket)
            'Create a cookie, and then add the encrypted ticket to the cookie as data.
            Dim authCookie As New HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            If isCookiePersistent Then
                authCookie.Expires = authTicket.Expiration   ' cookie過期日
            End If
            'Add the cookie to the outgoing cookies collection.
            context.Response.Cookies.Add(authCookie)
        End Sub

        '[String] GetLoginUserIDSrc : 讀取Cookie，取得登入者ID
        Public Shared Function GetLoginUserIDByCookie() As String
            Try
                'Cookie解密，還原使用者代碼
                Dim context As HttpContext = HttpContext.Current
                Dim encryptedAuthKey As HttpCookie = context.Request.Cookies.Item(FormsAuthentication.FormsCookieName)
                Dim sysAuthKey As FormsAuthenticationTicket = FormsAuthentication.Decrypt(encryptedAuthKey.Value())

                Return sysAuthKey.Name
            Catch
                Return Nothing
            End Try
        End Function
    End Class
End Namespace