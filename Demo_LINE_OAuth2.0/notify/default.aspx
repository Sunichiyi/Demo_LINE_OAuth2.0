<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="default.aspx.vb" Inherits="Demo_LINE_OAuth2._0._default1" %>

<!DOCTYPE html>

<html lang="en" class="h-100">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <!-- Required meta tags -->
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <webopt:bundlereference runat="server" path="~/bundles/css" />

    <title>Demo LINE Notify Dashboard</title>
    <link rel="icon" href="images/cgu_icon.svg" />

    <script src="//cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script type="text/javascript">
        function alert(msg, type) {
            Swal.fire({
                title: msg,
                icon: type
            });
        }
    </script>
</head>
<body class="d-flex flex-column h-100">
    <header style="border-top: solid 10px #fb7832;">
        <div class="container">
            <!-- Fixed navbar -->
            <nav class="navbar navbar-expand navbar-light" runat="server" id="navbar">
                <a class="navbar-brand" href="Default.aspx">
                    Demo LINE Notify Dashboard
                </a>
            </nav>
        </div>
    </header>
    
    <!-- Begin page content -->
    <main role="main" class="flex-shrink-0">
        <div class="container">
            <div class="row">
                <div class="col">
                    <form runat="server">
                        <asp:ScriptManager runat="server">
                            <Scripts>
                                <%--To learn more about bundling scripts in ScriptManager see https://go.microsoft.com/fwlink/?LinkID=301884 --%>
                                <%--Framework Scripts--%>
                                <asp:ScriptReference Name="jquery" />
                                <asp:ScriptReference Name="bootstrap" />
                                <%--Site Scripts--%>
                            </Scripts>
                        </asp:ScriptManager>
                        <div class="row justify-content-center py-3" style="border-top: 5px solid #fb7832;">
                            <div class="col">
                                <div class="form-group">
                                    <asp:TextBox ID="TextBox1" CssClass="form-control" runat="server" Rows="8" TextMode="MultiLine"></asp:TextBox>
                                </div>
                                <div class="d-flex justify-content-end">
                                    <asp:Button ID="Button1" CssClass="btn btn-success" runat="server" Text="傳送訊息" />
                                </div>
                            </div>
                        </div>
                        <div class="row justify-content-center">
                            <div class="col-auto table-responsive">
                                <asp:GridView ID="gvData" runat="server" CssClass="table table-striped table-bordered table-sm text-center">
                                </asp:GridView>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </main>

    <footer class="footer mt-auto py-3">
        <div class="container text-center">
            <asp:Label ID="copyright" runat="server"></asp:Label>
        </div>
    </footer>
</body>
</html>
