<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="CrawlerWeb.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:DropDownList ID="ddlTask" runat="server" AutoPostBack="true" OnTextChanged="ddlTask_TextChanged"></asp:DropDownList>


            <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="false" DataKeyNames="Id"
                CssClass="table table-hover">
                <Columns>
                     <asp:BoundField HeaderText="Title" DataField="Title" />
                    <asp:BoundField HeaderText="Url" DataField="CrawUrl" />
                    <asp:BoundField HeaderText="Depth" DataField="Depth" />
                    <asp:BoundField HeaderText="CreationTime" DataField="CreationTime" />
                    <asp:TemplateField HeaderText="操作" ShowHeader="False">
                        <ItemTemplate>
                            <a href='Detail.aspx?id=<%#Eval("Id") %>'>查看</a>

                            &nbsp;
                        </ItemTemplate>

                        <ItemStyle Width="8%" HorizontalAlign="Center" Wrap="false" />
                    </asp:TemplateField>
                </Columns>

            </asp:GridView>
        </div>
    </form>
</body>
</html>
