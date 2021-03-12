<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Program4c._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    
    <h2>Welcome!</h2>
    <p>CSS436: Program 4 - Stephanie Moua <br />This program uses Azure Containers and Azure Tables to load data from a URL and store it in a NoSQL database.</p>
    <br />

    <div class="row">
        <div class="col-md-9">
            <p>
                <b>Use the two buttons below to update or clear the data in the database.</b>
            <br />
                Update database&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Load Data" />
                &nbsp;</p>
            <p>
                Clear database&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Clear Data" />
            </p>
            <p>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
            </p>
            <p>
                &nbsp;</p>

            <p>
                <b>To search the database, please enter either a first or a last name or both:</b>
                    </p>
            <p>
                First Name:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
            </p>
            <p>
                Last Name:&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
                &nbsp;</p>
            <p>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;
                <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="Query" />
            </p>
            <p>
                <b>Results: </b><br />
                <asp:Label ID="Label2" runat="server" Text="Query results will be displayed here."></asp:Label>
            </p>
        </div>

    </div>

</asp:Content>
