﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class FriendList : System.Web.UI.Page
{
	UserServies UserCommand;
	protected void Page_Load(object sender, EventArgs e)
	{
		 UserCommand = new UserServies();
		if (!Page.IsPostBack)
		{
			if (Session["User"] == null)
				Response.Redirect("login.aspx");
		}
		PopGrid();
	}
		
	public void PopGrid()
	{
		DataSet Ds = new DataSet();
		Ds = UserCommand.PopFriends(((UserDetail)Session["User"]).UserId);
		this.GridViewFriends.DataSource = Ds;
		//this.GridViewFriends.ID = Ds.Tables[0].Columns["1"].ToString();
		this.GridViewFriends.DataBind();
		
	}

	protected void GridViewFriends_RowDeleting(object sender, GridViewDeleteEventArgs e)
	{//מיחקה חבר מהfrendslist
		UserCommand.DeleteUser(((UserDetail)Session["User"]).UserId, int.Parse(this.GridViewFriends.Rows[e.RowIndex].Cells[1].Text));
		GridViewFriends.EditIndex = -1;
		LabelErr.Text = "This Member Delete from you Frendslist";
		PopGrid();
	}

	//protected void GridViewFriends_RowDataBound(object sender, GridViewRowEventArgs e)
	//{//מוסיף אזהרה  לפני מחיקת משתמש
	//	//if (e.Row.RowType != DataControlRowType.Header && e.Row.RowType != DataControlRowType.Footer && e.Row.RowType != DataControlRowType.Pager)
	//	//{
	//	//	Button Del = (Button)e.Row.Cells[3].FindControl("Button1");
	//	//	Del.Attributes["onclick"] = "javascript:return confirm('r u sure delete ?')"; ;
	//	//}
	//}

	protected void ButtonSerchFrends_Click(object sender, EventArgs e)
	{
		try
		{//מוסיף חבר חדש
			int FrendsId = UserCommand.IsUserValidName(this.TextBoxSerchFirend.Text);
			UserCommand.AddFriends(((UserDetail)Session["User"]).UserId, FrendsId);
			this.LabelErr.Text = "The User Added For Your FrendsList";
			PopGrid();
		}
		catch(Exception Err) { this.LabelErr.Text = Err.Message; }
	
	}
}