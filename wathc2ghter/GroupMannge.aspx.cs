﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class GroupMannge : System.Web.UI.Page
{
	GroupServies GP;
	DataSet Ds = new DataSet();
    protected void Page_Load(object sender, EventArgs e)
    {
            if (!Page.IsPostBack)
            {
                if (Session["User"] == null)
                    Response.Redirect("HomePage.aspx");
			this.LabelFrendlist.Visible = false;
			this.LabelPeople.Visible = false;
			this.TablePageInterFace.Visible = false;
			popListGroup();
			Session["Group"] = new Group();
		}
		

		GP = new GroupServies();
	}
    protected void popListGroup()
    {//מחזיר הקבוצות שאתה יכול לנהל 
            GroupServies Gr = new GroupServies();
            Ds = new DataSet();
            Ds = Gr.GetGroupCanMannge(((UserDetail)Session["User"]).UserId);
		if (Ds.Tables[0].Rows.Count < 1)
		{
			LabelFrendlist.Visible = true;
			LabelFrendlist.Text = "You Not Mannge any group";
		}
		else
		{
			
			this.RadioButtonListGroup.DataSource = Ds.Tables[0];
			RadioButtonListGroup.DataTextField = Ds.Tables[0].Columns["GroupName"].ToString();
			RadioButtonListGroup.DataValueField = Ds.Tables[0].Columns["GroupId"].ToString();
			RadioButtonListGroup.DataBind();
		}
    }
	public void PopGrid(int id)
	{
        GroupServies Gr = new GroupServies();
        Ds= Gr.GetPoepleInGroupDb(id);
		int num = ((UserDetail) Session["User"]).UserId;
		UserServies UserCommand = new UserServies();
        DataSet DsFirends = new DataSet();
        DsFirends = UserCommand.PopFriends(num);
        for (int d = 0; d < Ds.Tables[0].Rows.Count; d++)
        {
            for (int i = 0; i < DsFirends.Tables[0].Rows.Count; i++)
            {
                
                    if (Ds.Tables[0].Rows[d]["UserId"].ToString() == DsFirends.Tables[0].Rows[i]["FriendId"].ToString())
                        DsFirends.Tables[0].Rows.RemoveAt(i);
            }
        }
        this.GridViewFrends.DataSource = DsFirends;
		GridViewFrends.DataBind();
	}
	protected void RadioButtonListGroup_SelectedIndexChanged(object sender, EventArgs e)
	{//מציג את נתוני הקבוצה
		LabelFrendlist.Visible = true;
		LabelPeople.Visible = true;
		this.TablePageInterFace.Visible = true;
		Session["Group"] = GP.GetGroupToWatch(int.Parse(this.RadioButtonListGroup.SelectedValue), 0);
		this.TextBoxGroupName.Text = ((Group)Session["Group"]).GroupeName;
		DropDownListKindGourp.DataSource = GP.GetKindGroup();
		DropDownListKindGourp.DataTextField = "Kind";
		DropDownListKindGourp.DataValueField = "KindId";
		DropDownListKindGourp.DataBind();
		DropDownListKindGourp.Items.FindByValue(((Group)Session["Group"]).KindGroup.ToString()).Selected = true;
        PopGrid(((Group)Session["Group"]).GroupId);
		PopGroupMember(((Group)Session["Group"]).GroupId);

	}
	public void PopGroupMember(int id)
	{//מציג את חברי הקבוצה
		GroupServies Gr = new GroupServies();
		this.GridViewPeopleOnGroup.DataSource = Gr.GetPoepleInGroupDb(id);
		this.GridViewPeopleOnGroup.DataBind();
	}

	protected void GridViewFrends_RowCommand(object sender, GridViewCommandEventArgs e)
	{
		if (e.CommandName == "Add")
		{
			int MemeberID = int.Parse(((GridView)sender).Rows[Convert.ToInt32(e.CommandArgument.ToString())].Cells[1].Text);
			try
			{
				((Group)Session["Group"]).AddUser(MemeberID);
				GP.AddMemberToGroup(MemeberID, ((Group)Session["Group"]).GroupId, false);
			}
			catch(Exception Err)
			{ LabelErr.Text = "המשתמש קיים במערכת"; }
            PopGrid(((Group)Session["Group"]).GroupId);
			PopGroupMember(((Group)Session["Group"]).GroupId);
		}
	}

	protected void GridViewPeopleOnGroup_RowCommand(object sender, GridViewCommandEventArgs e)
	{
		if (e.CommandName == "invite")
		{
			int MemeberID = int.Parse(((GridView)sender).Rows[Convert.ToInt32(e.CommandArgument.ToString())].Cells[1].Text);
			GP.UpdateToWating(((Group)Session["Group"]).GroupId, MemeberID,5);
			PopGroupMember(((Group)Session["Group"]).GroupId);
		}
		if (e.CommandName == "Kick")
		{
			int MemeberID = int.Parse(((GridView)sender).Rows[Convert.ToInt32(e.CommandArgument.ToString())].Cells[1].Text);
			GP.UpdateToWating(((Group)Session["Group"]).GroupId, MemeberID, 10);
			PopGroupMember(((Group)Session["Group"]).GroupId);
		}
		
		}

	protected void GridViewPeopleOnGroup_RowDataBound(object sender, GridViewRowEventArgs e)
	{//מטפל בכפתורים של לעיף בן אדם המקבוצה או לאשיר אותו
		if (e.Row.RowType != DataControlRowType.Header && e.Row.RowType != DataControlRowType.Footer && e.Row.RowType != DataControlRowType.Pager)
		{
			if (e.Row.Cells[3].Text == "Online")
				{
				e.Row.Cells[4].Enabled = false;
				e.Row.Cells[5].Enabled = true;
			}
			else if(e.Row.Cells[3].Text == "Waiting")
			{
				e.Row.Cells[4].Enabled = true;
				e.Row.Cells[5].Enabled = true;
			}
			else
			{
				e.Row.Cells[4].Enabled = true;
				e.Row.Cells[5].Enabled = false;
			}

		}
	}

	protected void ButtonStatus_Click(object sender, EventArgs e)
	{
		GP.DelGroup(((Group)Session["Group"]).GroupId);
	}

	protected void ButtonUpdate_Click(object sender, EventArgs e)
	{
		GP.UpdateGroupGroup(((Group)Session["Group"]).GroupId,this.TextBoxGroupName.Text,int.Parse(this.DropDownListKindGourp.SelectedValue));
		LabelErr.Text = "Fixed :D";
	}
}