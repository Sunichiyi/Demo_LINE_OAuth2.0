Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports Newtonsoft.Json
Imports Demo_LINE_OAuth2._0.CommonUtility.General
Imports Oracle.ManagedDataAccess.Client
Imports System.IdentityModel.Tokens.Jwt

Public Class callback
    Inherits System.Web.UI.Page

    ReadOnly HomePage = "default.aspx"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        Try
            Dim Request_code As String = Request.QueryString("code")
            Dim Request_state As String = Request.QueryString("state")

            'curl -v - X POST https: //api.line.me/oauth2/v2.1/token \
            '-H 'Content-Type: application/x-www-form-urlencoded' \
            '-d 'grant_type=authorization_code' \
            '-d 'code=1234567890abcde' \
            '--data-urlencode 'redirect_uri=https://example.com/auth?key=value' \
            '-d 'client_id=1234567890' \
            '-d 'client_secret=1234567890abcdefghij1234567890ab'

            If Not String.IsNullOrEmpty(Request_code) AndAlso Request_state = GetAppSetting("state") Then
                Dim values As New Dictionary(Of String, String) From {
                    {"grant_type", "authorization_code"},
                    {"code", Request_code},
                    {"client_id", GetAppSetting("client_id")},
                    {"client_secret", GetAppSetting("client_secret")},
                    {"redirect_uri", GetAppSetting("redirect_uri")}
                }

                Dim result = JsonConvert.DeserializeObject(Of AccessToken)(GetAccessTokenAsync(values).Result)
                SetCookies(result.id_token, False, "id_token")
                MergeIDtoken(result.id_token)
                Response.Redirect(HomePage)
            Else
                Response.Redirect(HomePage)
            End If
        Catch ex As Exception
            'Response.Redirect(HomePage)
        End Try
    End Sub

    Sub MergeIDtoken(id_token As String)

        Dim TokenInfo As New Dictionary(Of String, String)
        Dim handler As New JwtSecurityTokenHandler()
        Dim jwtIDToken = handler.ReadJwtToken(id_token)
        Dim claims = jwtIDToken.Claims.ToList()
        For Each claim In claims
            TokenInfo.Add(claim.Type, claim.Value)
        Next

        Dim QrySql As String = "OAUTHDEMO_CALLBACK.MERGEIDTOKEN"
        Dim myCommand As New OracleCommand(QrySql, myConn) With {
            .CommandType = CommandType.StoredProcedure
        }
        Try
            'INPUT參數
            myCommand.Parameters.Add(New OracleParameter("P_SUBJECT", jwtIDToken.Subject))
            myCommand.Parameters.Add(New OracleParameter("P_NAME", TokenInfo.Item("name")))
            myCommand.Parameters.Add(New OracleParameter("P_IMAGEURL", TokenInfo.Item("picture")))

            'OUTPUT參數
            myCommand.Parameters.Add("RETURN_STATUS", OracleDbType.Boolean, ParameterDirection.Output)

            myConn.Open()

            'Dim dt As New DataTable
            Dim myDataReader As OracleDataReader
            myDataReader = myCommand.ExecuteReader
            If myDataReader.HasRows Then
                '---將回傳的數值放入 DataTable
                'dt.Load(myDataReader)
            End If

        Catch ex As Exception
            'lblMessage.Text = ex.Message
        Finally
            myConn.Close()
        End Try
    End Sub

    Async Function GetAccessTokenAsync(values As Dictionary(Of String, String)) As Threading.Tasks.Task(Of String)
        '宣告HttpClient
        Using client As New HttpClient()
            '宣告ResponseMessage來承接回傳的內容
            Using request As New HttpRequestMessage(HttpMethod.Post, "https://api.line.me/oauth2/v2.1/token")
                '宣告要送出的Body內容
                Using HttpContent As HttpContent = New FormUrlEncodedContent(values)
                    '設定Body內容的格式
                    HttpContent.Headers.ContentType = New MediaTypeHeaderValue("application/x-www-form-urlencoded")
                    '設定RequestMessage的Content是HttpContent
                    request.Content = HttpContent
                    '宣告ResponseMessage來承接回傳的內容
                    Dim response = Await client.SendAsync(request).ConfigureAwait(False)
                    '確認成功
                    response.EnsureSuccessStatusCode()
                    '讀取回傳的內容
                    Dim responseContent As String = Await response.Content.ReadAsStringAsync().ConfigureAwait(False)
                    '顯示回傳的內容
                    Return responseContent
                End Using
            End Using
        End Using
    End Function

    Class AccessToken
        Public access_token As String
        Public expires_in As String
        Public id_token As String
        Public refresh_token As String
        Public scope As String
        Public token_type As String
    End Class
End Class
