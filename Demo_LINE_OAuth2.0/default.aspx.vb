Imports System.IdentityModel.Tokens.Jwt
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports Demo_LINE_OAuth2._0.CommonUtility.General
Imports Newtonsoft.Json
Imports Oracle.ManagedDataAccess.Client

Public Class _default
    Inherits System.Web.UI.Page

    ReadOnly UserID As String = GetLoginUserIDByCookie()
    Dim access_token As String = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        If Not IsPostBack Then
            copyright.Text = "Copyright &copy; " & Today.Year.ToString & " Chang Gung University."
        End If

        If Not String.IsNullOrEmpty(UserID) Then

            'Dim TokenInfo As New Dictionary(Of String, String)
            'Dim handler As New JwtSecurityTokenHandler()
            'Dim jwtIDToken = handler.ReadJwtToken(UserID)
            'Dim claims = jwtIDToken.Claims.ToList()
            'For Each claim In claims
            '    TokenInfo.Add(claim.Type, claim.Value)
            'Next
            'Dim iss As String = jwtIDToken.Issuer
            'Dim subj As String = jwtIDToken.Subject
            'Dim audiences As New List(Of String)(jwtIDToken.Audiences)
            'Dim exp As DateTime = jwtIDToken.ValidTo.AddHours(8)
            'Dim iat As DateTime = jwtIDToken.IssuedAt.AddHours(8)
            'Dim name As String = TokenInfo.Item("name")
            'Dim picture As String = TokenInfo.Item("picture")

            Dim handler As New JwtSecurityTokenHandler()
            Dim jwtIDToken = handler.ReadJwtToken(UserID)

            Dim QrySql As String = "OAUTHDEMO_DEFAULT.GETUSERINFO"
            Dim myCommand As New OracleCommand(QrySql, myConn) With {
                .CommandType = CommandType.StoredProcedure
            }
            Try
                'INPUT參數
                myCommand.Parameters.Add(New OracleParameter("P_SUBJECT", jwtIDToken.Subject))

                'OUTPUT參數
                myCommand.Parameters.Add("RETURN_ACCESSTOKEN", OracleDbType.Varchar2, 100, "", ParameterDirection.Output)
                myCommand.Parameters.Add("RETURN_NAME", OracleDbType.Varchar2, 50, "", ParameterDirection.Output)
                myCommand.Parameters.Add("RETURN_IMAGEURL", OracleDbType.Varchar2, 200, "", ParameterDirection.Output)
                myCommand.Parameters.Add("RETURN_STATUS", OracleDbType.Boolean, ParameterDirection.Output)

                myConn.Open()

                myCommand.ExecuteScalar()

                If Boolean.Parse(myCommand.Parameters("RETURN_STATUS").Value.ToString) Then
                    LINE_info.Visible = True
                    Buttom_Login.Visible = False
                    Buttom_Logout.Visible = True

                    access_token = myCommand.Parameters("RETURN_ACCESSTOKEN").Value.ToString
                    Label_name.Text = myCommand.Parameters("RETURN_NAME").Value.ToString
                    Image_picture.ImageUrl = myCommand.Parameters("RETURN_IMAGEURL").Value.ToString

                    If GetStatusAsync(access_token).Result Then
                        Button_Revoke.Visible = True
                        Button_Subscribe.Visible = False
                    Else
                        Button_Revoke.Visible = False
                        Button_Subscribe.Visible = True
                    End If
                Else
                    Buttom_Login.Visible = True
                End If

            Catch ex As Exception
                'Label1.Text = ex.Message
                Buttom_Login.Visible = True
            Finally
                myConn.Close()
            End Try
        Else
            Buttom_Login.Visible = True
        End If
    End Sub

    Protected Sub Button_Subscribe_Click(sender As Object, e As EventArgs) Handles Button_Subscribe.Click
        Try
            'https://notify-bot.line.me/oauth/authorize?response_type=code&client_id=RJZjCLa1rmICrWEQ56MXpw&state=123123&scope=notify&redirect_uri=https%3A%2F%2Foauth.pstmn.io%2Fv1%2Fcallback
            Dim response_type = "response_type=code"
            Dim client_id = "client_id=" + HttpUtility.UrlEncode(GetAppSetting("notify_client_id"))
            Dim redirect_uri = "redirect_uri=" + HttpUtility.UrlEncode(GetAppSetting("notify_redirect_uri"))
            Dim state = "state=" + HttpUtility.UrlEncode(GetAppSetting("notify_state"))
            Dim scope = "scope=notify"
            Dim sep = "&"
            Dim sb As New StringBuilder()
            sb.Append(response_type).Append(sep).Append(client_id).Append(sep)
            sb.Append(redirect_uri).Append(sep).Append(state).Append(sep).Append(scope)

            Response.Redirect("https://notify-bot.line.me/oauth/authorize?" + sb.ToString())
        Catch ex As Exception
            'Label1.Text = ex.ToString
        End Try
    End Sub

    Protected Sub Button_Revoke_Click(sender As Object, e As EventArgs) Handles Button_Revoke.Click
        If Not String.IsNullOrEmpty(access_token) Then
            If PostRevokeAsync(access_token).Result Then
                Button_Revoke.Visible = False
                Button_Subscribe.Visible = True
                ScriptManager.RegisterStartupScript(Page, Page.GetType, "Popup", "alert('成功取消訂閱', '','success');", True)
            Else
                Button_Revoke.Visible = True
                Button_Subscribe.Visible = False
                ScriptManager.RegisterStartupScript(Page, Page.GetType, "Popup", "alert('尚未訂閱或已取消訂閱', '','info');", True)
            End If
        End If
    End Sub

    Async Function GetStatusAsync(access_token As String) As Threading.Tasks.Task(Of String)
        Try
            '宣告HttpClient
            Using client As New HttpClient()
                client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", access_token)
                '宣告ResponseMessage來承接回傳的內容
                Using request As New HttpRequestMessage(HttpMethod.Get, "https://notify-api.line.me/api/status")
                    '宣告ResponseMessage來承接回傳的內容
                    Dim response = Await client.SendAsync(request).ConfigureAwait(False)
                    '確認成功
                    response.EnsureSuccessStatusCode()
                    '讀取回傳的內容
                    Dim responseContent As ResponseType = JsonConvert.DeserializeObject(Of ResponseType)(Await response.Content.ReadAsStringAsync().ConfigureAwait(False))
                    '顯示回傳的內容
                    If responseContent.status = 200 Then
                        Return True
                    Else
                        Return False
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return False
            'Label1.Text = ex.ToString
        End Try
    End Function

    Async Function PostRevokeAsync(access_token As String) As Threading.Tasks.Task(Of String)
        Try
            '宣告HttpClient
            Using client As New HttpClient()
                client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", access_token)
                '宣告ResponseMessage來承接回傳的內容
                Using request As New HttpRequestMessage(HttpMethod.Post, "https://notify-api.line.me/api/revoke")
                    '宣告ResponseMessage來承接回傳的內容
                    Dim response = Await client.SendAsync(request).ConfigureAwait(False)
                    '確認成功
                    response.EnsureSuccessStatusCode()
                    '讀取回傳的內容
                    Dim responseContent As ResponseType = JsonConvert.DeserializeObject(Of ResponseType)(Await response.Content.ReadAsStringAsync().ConfigureAwait(False))
                    '顯示回傳的內容
                    If responseContent.status = 200 Then
                        Return True
                    Else
                        Return False
                    End If
                End Using
            End Using
        Catch ex As Exception
            Return False
            'Label1.Text = ex.ToString
        End Try
    End Function

    Class ResponseType
        Public status As String
        Public message As String
    End Class
End Class