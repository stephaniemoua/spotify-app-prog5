<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="Program4c.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <p>
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
&nbsp;+
        <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
&nbsp;=
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    </p>
    <p>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" />
    </p>
</asp:Content>
