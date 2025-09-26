using skylite;
using skylite.ToolKit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class XysPageEV : WebBase
{
    public XysPageEV()
    {
        ViewFields.AddRange(new List<NameFlag>
        {
            new NameFlag { name = "PageId", flag = true },
            new NameFlag { name = "PageName" },
            new NameFlag { name = "PageGroup" },
            new NameFlag { name = "PageDesc" },
            new NameFlag { name = "PageOrder" },
            new NameFlag { name = "PageMenu" },
            new NameFlag { name = "PageUse" }
        });
    }

    public override ViewPart InitialModel()
    {
        string SSQL = " Select PageId,PageName,PageGroup,PageDesc,PageOrder,PageMenu,PageUse From XysPage " +
                      " where PageId = N'" + PartialData + "'";
        string emsg = string.Empty;

        ViewPart = new ViewPart();
        ViewPart.Methods = ViewMethods();
        if (PartialData != string.Empty)
        {
            ViewPart.Mode = ViewMode.Edit;
            ViewPart.Data = SQLData.SQLDataTable(SSQL, ref emsg);
            ViewPart.Params = PartialData;
        }
        else
        {
            ViewPart.Mode = ViewMode.New;
        }

        for (int i = 0; i < ViewFields.Count; i++)
        {
            string colValue = ViewPart.ColunmValue(ViewFields[i].name);
            ViewPart.Fields.Add(new NameValue { name = ViewFields[i].name, value = colValue });
        }

        return ViewPart;
    }

    public override string InitialView()
    {
        MenuList mnulist = SetPageMenu(new string[] { });
        Wrap BtnWrap = SetPageButtons(ViewPart.Mode == ViewMode.New ? new string[] { "save" } : new string[] { });

        Label label = new Label();
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
        label.Wrap.InnerText = ViewPart.Mode == ViewMode.New ? Translator.Format("newpage") : Translator.Format("editpage");

        FilterSection filter = new FilterSection();
        filter.ModalWrap = true;
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
        filter.Wrap.SetStyle(HtmlStyles.width, "90%");
        filter.Menu = mnulist;
        filter.FilterHtml = label.HtmlText;

        Texts text = new Texts(Translator.Format("name"), ViewPart.Field("PageName").name, TextTypes.text);
        text.Required = true;
        text.Text.SetStyle(HtmlStyles.width, "200px");
        text.Text.SetAttribute(HtmlAttributes.maxlength, "200");
        text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("PageName").value);
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Texts text1 = new Texts(Translator.Format("group"), ViewPart.Field("PageGroup").name, TextTypes.text);
        text1.Text.SetStyle(HtmlStyles.width, "200px");
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "50");
        text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("PageGroup").value);
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Texts text2 = new Texts(Translator.Format("desc"), ViewPart.Field("PageDesc").name, TextTypes.text);
        text2.Required = true;
        text2.Text.SetStyle(HtmlStyles.width, "300px");
        text2.Text.SetAttribute(HtmlAttributes.maxlength, "200");
        text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("PageDesc").value);
        text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Texts text3 = new Texts(Translator.Format("order"), ViewPart.Field("PageOrder").name, TextTypes.text);
        text3.Text.SetStyle(HtmlStyles.width, "50px");
        text3.Text.SetAttribute(HtmlAttributes.maxlength, "4");
        text3.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("PageOrder").value);
        text3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        CheckBox chk1 = new CheckBox(Translator.Format("menu"));
        chk1.Checks.AddItem(ViewPart.Field("PageMenu").name, "1", string.Empty, (ValC(ViewPart.Field("PageMenu").value) == 1) ? true : false);
        chk1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        CheckBox chk2 = new CheckBox(Translator.Format("use"));
        chk2.Checks.AddItem(ViewPart.Field("PageUse").name, "1", string.Empty,(ValC(ViewPart.Field("PageUse").value) == 1) ? true : false);
        chk2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        HtmlElementBox elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.width, "90%");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.marginTop, "8px");
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px");

        elmBox.AddItem(text, 1);
        elmBox.AddItem(text1, 1);
        elmBox.AddItem(text2, 20);
        elmBox.AddItem(text3, 20);
        elmBox.AddItem(chk1, 1);
        elmBox.AddItem(chk2, 50);
        elmBox.AddItem(BtnWrap, 20);

        string ViewHtml = filter.HtmlText + elmBox.HtmlText;
        return ViewHtml;
    }

    public ApiResponse SaveView()
    {
        string PageName = ViewPart.Field("PageName").value;
        string PageGroup = ViewPart.Field("PageGroup").value;
        string PageDesc = ViewPart.Field("PageDesc").value;
        string PageOrder = ViewPart.Field("PageOrder").value;
        string PageMenu = ViewPart.Field("PageMenu").value;
        string PageUse = ViewPart.Field("PageUse").value;

        ApiResponse _ApiResponse = new ApiResponse();
        if (PageName == string.Empty || PageDesc == string.Empty)
        {
            DialogBox dialogBox = new DialogBox(Translator.Format("msg_required"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
        }
        else
        {
            string rlt = PutSaveView();
            if (rlt == string.Empty)
            {
                DialogBox dialogBox = new DialogBox(Translator.Format("msg_saved"));
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysPage) + "&$PopOff();");
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
            }
            else
            {
                DialogBox dialogBox = new DialogBox(rlt);
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
            }
        }

        return _ApiResponse;
    }

    private string PutSaveView()
    {
        List<string> SQL = new List<string>();

        switch (ViewPart.Mode)
        {
            case ViewMode.New:
                ViewPart.Field("PageId").value = NewID();
                SQL.Add(" Insert into XysPage( PageId,PageName,PageGroup,PageDesc,PageOrder,PageMenu,PageUse,SYSDTE,SYSUSR) " +
                        " values( @PageId, @PageName, @PageGroup, @PageDesc, @PageOrder, @PageMenu,@PageUse, getdate(), @SYSUSR) ");
                break;
            case ViewMode.Edit:
                SQL.Add(" Update XysPage set " +
                        " PageName = @PageName, PageGroup = @PageGroup, PageDesc = @PageDesc, " +
                        " PageOrder = @PageOrder, PageMenu = @PageMenu, PageUse= @PageUse, SYSDTE = getdate(), SYSUSR = @SYSUSR " +
                        " WHERE PageId = @PageId");
                break;
        }

        List<SqlParameter> SqlParams = new List<SqlParameter>();
        SqlParams.Add(new SqlParameter { ParameterName = "@PageId", Value = ViewPart.Field("PageId").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@PageName", Value = ViewPart.Field("PageName").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@PageGroup", Value = ViewPart.Field("PageGroup").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@PageDesc", Value = ViewPart.Field("PageDesc").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@PageOrder", Value = ValC(ViewPart.Field("PageOrder").value).ToString(), SqlDbType = SqlDbType.Int });
        SqlParams.Add(new SqlParameter { ParameterName = "@PageMenu", Value = ValC(ViewPart.Field("PageMenu").value).ToString(), SqlDbType = SqlDbType.Int });
        SqlParams.Add(new SqlParameter { ParameterName = "@PageUse", Value = ValC(ViewPart.Field("PageUse").value).ToString(), SqlDbType = SqlDbType.Int });
        SqlParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

        return PutData(SqlWithParams(SQL, SqlParams));
    }

    public ApiResponse DeleteView()
    {
        ApiResponse _ApiResponse = new ApiResponse();

        DialogBox dialogBox = new DialogBox(Translator.Format("deletepage"));
        dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;");
        dialogBox.AddButton(Translator.Format("yes"), string.Empty, "class:button1;onclick:" + ByPassCall("XysPageEV/DeleteViewConfirm"));
        dialogBox.AddButton(Translator.Format("no"), string.Empty, "onclick:$PopOff();class:button;");
        _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);

        return _ApiResponse;
    }

    public ApiResponse DeleteViewConfirm()
    {
        ApiResponse _ApiResponse = new ApiResponse();
        string rlt = PutDeleteViewData();
        if (rlt == string.Empty)
        {
            DialogBox dialogBox = new DialogBox(Translator.Format("msg_deleted"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysPage) + "&$PopOff();");
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
        }
        else
        {
            DialogBox dialogBox = new DialogBox(rlt);
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
        }
        return _ApiResponse;
    }

    private string PutDeleteViewData()
    {
        List<string> SQL = new List<string>
        {
            " delete from XysRoleMenu where MenuId in (select MenuId from XysMenu where Pageid = @PageId) ",
            " delete from XysMenu where Pageid = @PageId ",
            " delete from XysRolePage where Pageid = @PageId ",
            " delete from XysPage where Pageid = @PageId "
        };

        List<SqlParameter> SqlParams = new List<SqlParameter>();
        SqlParams.Add(new SqlParameter { ParameterName = "@PageId", Value = ViewPart.Field("PageId").value, SqlDbType = SqlDbType.NVarChar });

        return PutData(SqlWithParams(SQL, SqlParams));
    }
}