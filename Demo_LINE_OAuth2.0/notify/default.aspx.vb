Imports System.Net.Http
Imports System.Net.Http.Headers
Imports Newtonsoft.Json
Imports Oracle.ManagedDataAccess.Client
Imports Demo_LINE_OAuth2._0.CommonUtility.General
Imports System.Net

Public Class _default1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        If Not IsPostBack Then
            copyright.Text = "Copyright &copy; " & Today.Year.ToString & " Chang Gung University."
        End If

        LoadHistory()
    End Sub

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not String.IsNullOrEmpty(TextBox1.Text.Trim) Then
            Dim values As New Dictionary(Of String, String) From {
                {"message", TextBox1.Text}
            }
            GetThenPostNotify(values)
            LoadHistory()
        Else
            ScriptManager.RegisterStartupScript(Page, Page.GetType, "Popup", "alert('請輸入訊息內容', '','warning');", True)
        End If
    End Sub

    Sub LoadHistory()
        Dim QrySql As String = "OAUTHDEMO_NOTIFYDEFAULT.SELECTHISTORY"
        Dim myCommand As New OracleCommand(QrySql, myConn) With {
            .CommandType = CommandType.StoredProcedure
        }
        Try
            'INPUT參數

            'OUTPUT參數
            myCommand.Parameters.Add("RETURN_DATA", OracleDbType.RefCursor, ParameterDirection.Output)

            myConn.Open()

            Dim dt As New DataTable
            Dim myDataReader As OracleDataReader
            myDataReader = myCommand.ExecuteReader
            If myDataReader.HasRows Then
                '---將回傳的數值放入 DataTable
                dt.Load(myDataReader)

                gvData.DataSource = dt
                gvData.DataBind()
            End If
        Catch ex As Exception
            'Label1.Text = ex.ToString
        Finally
            myConn.Close()
        End Try
    End Sub

    Sub GetThenPostNotify(values As Dictionary(Of String, String))
        Dim QrySql As String = "OAUTHDEMO_NOTIFYDEFAULT.SELECTACCESSTOKEN"
        Dim myCommand As New OracleCommand(QrySql, myConn) With {
            .CommandType = CommandType.StoredProcedure
        }
        Try
            'INPUT參數
            myCommand.Parameters.Add(New OracleParameter("P_MESSAGE", values.Item("message")))

            'OUTPUT參數
            myCommand.Parameters.Add("RETURN_DATA", OracleDbType.RefCursor, ParameterDirection.Output)
            myCommand.Parameters.Add("RETURN_MESSAGEID", OracleDbType.Int16, ParameterDirection.Output)

            myConn.Open()

            Dim dt As New DataTable
            Dim myDataReader As OracleDataReader
            myDataReader = myCommand.ExecuteReader
            If myDataReader.HasRows Then
                '---將回傳的數值放入 DataTable
                dt.Load(myDataReader)

                ScriptManager.RegisterStartupScript(Page, Page.GetType, "Popup", "alert('完成傳送', '','success');", True)
            Else
                ScriptManager.RegisterStartupScript(Page, Page.GetType, "Popup", "alert('沒有人訂閱', '','info');", True)
            End If

            myConn.Close()

            For Each row As DataRow In dt.Rows
                InsertPostNotifyResult(row.Item(0), myCommand.Parameters("RETURN_MESSAGEID").Value.ToString, PostNotifyAsync(row.Item(0), values).Result.ToString)
            Next
        Catch ex As Exception
            'Label1.Text = ex.ToString
            ScriptManager.RegisterStartupScript(Page, Page.GetType, "Popup", "alert('發生錯誤', '','error');", True)
        Finally
            myConn.Close()
        End Try
    End Sub

    Async Function PostNotifyAsync(access_token As String, values As Dictionary(Of String, String)) As Threading.Tasks.Task(Of Short)
        Try
            '宣告HttpClient
            Using client As New HttpClient()
                client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", access_token)
                '宣告ResponseMessage來承接回傳的內容
                Using request As New HttpRequestMessage(HttpMethod.Post, "https://notify-api.line.me/api/notify")
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
                        Dim responseContent As ResponseNotifyType = JsonConvert.DeserializeObject(Of ResponseNotifyType)(Await response.Content.ReadAsStringAsync().ConfigureAwait(False))
                        '顯示回傳的內容
                        If responseContent.status = 200 Then
                            Return 1
                        Else
                            Return 0
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Page, Page.GetType, "Popup", "alert('發生錯誤', '','error');", True)
            Return 0
        End Try
    End Function

    Sub InsertPostNotifyResult(access_token As String, message_id As String, status As String)
        Dim QrySql As String = "OAUTHDEMO_NOTIFYDEFAULT.INSERTRESULT"
        Dim myCommand As New OracleCommand(QrySql, myConn) With {
            .CommandType = CommandType.StoredProcedure
        }
        Try
            'INPUT參數
            myCommand.Parameters.Add(New OracleParameter("P_ACCESSTOKEN", access_token))
            myCommand.Parameters.Add(New OracleParameter("P_MESSAGEID", Convert.ToInt16(message_id)))
            myCommand.Parameters.Add(New OracleParameter("P_STATUS", status))

            'OUTPUT參數
            myCommand.Parameters.Add("RETURN_STATUS", OracleDbType.Boolean, ParameterDirection.Output)

            myConn.Open()

            myCommand.ExecuteScalar()

        Catch ex As Exception
            'Label1.Text = status + "   " + ex.ToString
        Finally
            myConn.Close()
        End Try
    End Sub

    Class ResponseNotifyType
        Public status As String
        Public message As String
    End Class
End Class