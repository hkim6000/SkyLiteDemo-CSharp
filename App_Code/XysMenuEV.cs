using skylite;
using skylite.ToolKit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class XysMenuEV : WebBase
{
    public XysMenuEV()
    {
        ViewFields.AddRange(new List<NameFlag>
        {
            new NameFlag { name = "MenuId", flag = true },
            new NameFlag { name = "MenuDesc" },
            new NameFlag { name = "MenuArea" },
            new NameFlag { name = "MenuTag" },
            new NameFlag { name = "MenuMethod" },
            new NameFlag { name = "MenuParams" },
            new NameFlag { name = "MenuCtl" },
            new NameFlag { name = "MenuType" },
            new NameFlag { name = "MenuClass" },
            new NameFlag { name = "MenuOrder" },
            new NameFlag { name = "MenuUse" },
            new NameFlag { name = "PageId" }
        });
    }

    public override ViewPart InitialModel()
    {
        string SSQL = " Select MenuId,PageId,MenuDesc,MenuArea,MenuTag,MenuMethod,MenuParams,MenuCtl,MenuType,MenuClass,MenuOrder,MenuUse From XysMenu   " +
                      " where MenuId = N'" + PartialData + "'";
        string emsg = string.Empty;

        ViewPart = new ViewPart();
        ViewPart.Methods = ViewMethods();
        if (!string.IsNullOrEmpty(PartialData))
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
            ViewPart.Fields.Add(new NameValue { name = ViewFields[i].name, value = ViewPart.ColunmValue(ViewFields[i].name) });
        }

        return ViewPart;
    }

    public override string InitialView()
    {
        MenuList mnulist = SetPageMenu(new string[] {});

        Wrap BtnWrap = SetPageButtons(ViewPart.Mode == ViewMode.New ? new string[] { "save" } : new string[]{});

        Label label = new Label();
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
        label.Wrap.InnerText = ViewPart.Mode == ViewMode.New ? Translator.Format("newmenu") : Translator.Format("editmenu");

        FilterSection filter = new FilterSection();
        filter.ModalWrap = true;
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
        filter.Wrap.SetStyle(HtmlStyles.width, "90%");
        filter.Menu = mnulist;
        filter.FilterHtml = label.HtmlText;
         
        Dropdown sel1 = new Dropdown(Translator.Format("page"), ViewPart.Field("PageId").name);
        sel1.Required = true;
        sel1.SelBox.SetStyle(HtmlStyles.width, "316px");
        sel1.SelOptions = new OptionValues("sql@select PageId,PageName + case when PageName <> PageDesc then ' (' + PageDesc + ')' else '' end as PageName " +
                                           " from XysPage order by PageGroup,PageOrder,PageName", ViewPart.Field("PageId").value);
        sel1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Dropdown sel2 = new Dropdown(Translator.Format("area"), ViewPart.Field("MenuArea").name);
        sel2.Required = true;
        sel2.SelBox.SetStyle(HtmlStyles.width, "120px");
        sel2.SelOptions = new OptionValues("{X|Method}{M|Menu}{B|Button}", ViewPart.Field("MenuArea").value);
        sel2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Texts text2 = new Texts(Translator.Format("desc"), ViewPart.Field("MenuDesc").name, TextTypes.text);
        text2.Required = true;
        text2.Text.SetStyle(HtmlStyles.width, "400px");
        text2.Text.SetAttribute(HtmlAttributes.maxlength, "100");
        text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("MenuDesc").value);
        text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Texts text = new Texts(Translator.Format("tag"), ViewPart.Field("MenuTag").name, TextTypes.text);
        text.Text.SetStyle(HtmlStyles.width, "268px");
        text.Text.SetAttribute(HtmlAttributes.maxlength, "100");
        text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("MenuTag").value);
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Texts text1 = new Texts(Translator.Format("method"), ViewPart.Field("MenuMethod").name, TextTypes.text);
        text1.Required = true;
        text1.Text.SetStyle(HtmlStyles.width, "400px");
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "400");
        text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("MenuMethod").value);
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Texts text3 = new Texts(Translator.Format("params"), ViewPart.Field("MenuParams").name, TextTypes.text);
        text3.Text.SetStyle(HtmlStyles.width, "400px");
        text3.Text.SetStyle(HtmlStyles.fontSize, "14px");
        text3.Text.SetAttribute(HtmlAttributes.maxlength, "500");
        text3.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("MenuParams").value);
        text3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Texts text4 = new Texts(Translator.Format("class"), ViewPart.Field("MenuClass").name, TextTypes.text);
        text4.Text.SetStyle(HtmlStyles.width, "200px");
        text4.Text.SetAttribute(HtmlAttributes.maxlength, "100");
        text4.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("MenuClass").value);
        text4.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Dropdown sel3 = new Dropdown(Translator.Format("order"), ViewPart.Field("MenuOrder").name);
        sel3.Required = true;
        sel3.SelBox.SetStyle(HtmlStyles.width, "80px");
        sel3.SelOptions = new OptionValues("{0|0}{1|1}{2|2}{3|3}{4|4}{5|5}{6|6}{7|7}{8|8}{9|9}{10|10}{11|11}{12|12}{13|13}{14|14}{15|15}", ViewPart.Field("MenuOrder").value);
        sel3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        CheckBox chk1 = new CheckBox(Translator.Format("use"));
        if (ViewPart.Mode == ViewMode.Edit)
        {
            chk1.Checks.AddItem(ViewPart.Field("MenuUse").name, "1", string.Empty, (ValC(ViewPart.Field("MenuUse").value) == 1) ? true : false);
        }
        else
        {
            chk1.Checks.AddItem(ViewPart.Field("MenuUse").name, "1", string.Empty, true);
        }
        chk1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        HtmlElementBox elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.width, "90%");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.marginTop, "8px");
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px");

        elmBox.AddItem(sel1, 1);
        elmBox.AddItem(text2, 1);
        elmBox.AddItem(sel2);
        elmBox.AddItem(text, 20);
        elmBox.AddItem(text1, 1);
        elmBox.AddItem(text3, 1);
        elmBox.AddItem(text4, 20);
        elmBox.AddItem(sel3, 20);
        elmBox.AddItem(chk1, 50);
        elmBox.AddItem(BtnWrap, 20);

        string ViewHtml = filter.HtmlText + elmBox.HtmlText;
        return ViewHtml;
    }

    public ApiResponse SaveView()
    {
        string MenuDesc = ViewPart.Field("MenuDesc").value;
        string MenuArea = ViewPart.Field("MenuArea").value;
        string MenuTag = ViewPart.Field("MenuTag").value;
        string MenuMethod = ViewPart.Field("MenuMethod").value;
        string MenuParams = ViewPart.Field("MenuParams").value;
        string MenuClass = ViewPart.Field("MenuClass").value;
        string MenuOrder = ViewPart.Field("MenuOrder").value;
        string MenuUse = ViewPart.Field("MenuUse").value;
        string PageId = ViewPart.Field("PageId").value;

        ApiResponse _ApiResponse = new ApiResponse();
        if (string.IsNullOrEmpty(PageId) || string.IsNullOrEmpty(MenuMethod) || string.IsNullOrEmpty(MenuDesc))
        {
            DialogBox dialogBox = new DialogBox(Translator.Format("msg_required"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
        }
        else
        {
            string rlt = PutSaveView();
            if (string.IsNullOrEmpty(rlt))
            {
                DialogBox dialogBox = new DialogBox(Translator.Format("msg_saved"));
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysMenu) + ";onclick:$PopOff();");
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

        string tag = ViewPart.Field("MenuTag").value;
        string method = ViewPart.Field("MenuMethod").value;
        string @params = ViewPart.Field("MenuParams").value; // @params to avoid keyword conflict
        string styleclass = ViewPart.Field("MenuClass").value;

        switch (ViewPart.Field("MenuArea").value.ToLower())
        {
            case "x":
                ViewPart.Field("MenuCtl").value = string.Empty;
                ViewPart.Field("MenuType").value = string.Empty;
                break;
            case "m":
                string ctlObjHtmlTag = SerializeObject(MenuElement(tag, method, @params, styleclass), typeof(HtmlTag));
                ViewPart.Field("MenuCtl").value = EscQuote(ctlObjHtmlTag);
                ViewPart.Field("MenuType").value = typeof(HtmlTag).FullName;
                break;
            case "b":
                string ctlObjButton = SerializeObject(BtnElement(tag, method, @params, styleclass), typeof(Button));
                ViewPart.Field("MenuCtl").value = EscQuote(ctlObjButton);
                ViewPart.Field("MenuType").value = typeof(Button).FullName;
                break;
        }

        switch (ViewPart.Mode)
        {
            case ViewMode.New:
                ViewPart.Field("MenuId").value = NewID();
                SQL.Add(" Insert into XysMenu( MenuId,PageId,MenuDesc,MenuArea,MenuTag,MenuMethod,MenuParams,MenuCtl,MenuType,MenuClass,MenuOrder,MenuUse,SYSDTE,SYSUSR) " +
                        " values ( @MenuId, @PageId, @MenuDesc, @MenuArea, @MenuTag, @MenuMethod, @MenuParams, @MenuCtl, @MenuType, @MenuClass,@MenuOrder,@MenuUse, getdate(), @SYSUSR)");
                break;
            case ViewMode.Edit:
                SQL.Add(" Update XysMenu set " +
                        " PageId = @PageId, MenuDesc = @MenuDesc, MenuArea = @MenuArea, MenuTag = @MenuTag, MenuMethod = @MenuMethod, " +
                        " MenuParams = @MenuParams, MenuCtl = @MenuCtl,MenuType = @MenuType, MenuClass = @MenuClass, MenuOrder = @MenuOrder, MenuUse = @MenuUse, " +
                        " SYSDTE = getdate(), SYSUSR = @SYSUSR " +
                        " WHERE MenuId = @MenuId");
                break;
        }

        List<SqlParameter> SqlParams = new List<SqlParameter>();
        SqlParams.Add(new SqlParameter { ParameterName = "@MenuId", Value = ViewPart.Field("MenuId").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@PageId", Value = ViewPart.Field("PageId").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@MenuDesc", Value = ViewPart.Field("MenuDesc").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@MenuArea", Value = ViewPart.Field("MenuArea").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@MenuTag", Value = ViewPart.Field("MenuTag").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@MenuMethod", Value = ViewPart.Field("MenuMethod").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@MenuParams", Value = ViewPart.Field("MenuParams").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@MenuCtl", Value = ViewPart.Field("MenuCtl").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@MenuType", Value = ViewPart.Field("MenuType").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@MenuClass", Value = ViewPart.Field("MenuClass").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@MenuOrder", Value = Convert.ToInt32(ViewPart.Field("MenuOrder").value).ToString(), SqlDbType = SqlDbType.Int });
        SqlParams.Add(new SqlParameter { ParameterName = "@MenuUse", Value = Convert.ToInt32(ViewPart.Field("MenuUse").value).ToString(), SqlDbType = SqlDbType.Int });
        SqlParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

        return PutData(SqlWithParams(SQL, SqlParams));
    }

    public ApiResponse DeleteView()
    {
        ApiResponse _ApiResponse = new ApiResponse();

        DialogBox dialogBox = new DialogBox(Translator.Format("deletemenu"));
        dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;");
        dialogBox.AddButton(Translator.Format("yes"), string.Empty, "class:button1;onclick:" + ByPassCall("XysMenuEV/DeleteViewConfirm"));
        dialogBox.AddButton(Translator.Format("no"), string.Empty, "onclick:$PopOff();class:button;");
        _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);

        return _ApiResponse;
    }

    public ApiResponse DeleteViewConfirm()
    {
        ApiResponse _ApiResponse = new ApiResponse();
        string rlt = PutDeleteView();
        if (string.IsNullOrEmpty(rlt))
        {
            DialogBox dialogBox = new DialogBox(Translator.Format("msg_deleted"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysMenu) + "&$PopOff();");
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

    private string PutDeleteView()
    {
        List<string> SQL = new List<string>
        {
            " delete from XysRoleMenu where MenuId = @MenuId ",
            " delete from XysMenu where Menuid = @MenuId "
        };

        List<SqlParameter> SqlParams = new List<SqlParameter>();
        SqlParams.Add(new SqlParameter { ParameterName = "@MenuId", Value = ViewPart.Field("MenuId").value, SqlDbType = SqlDbType.NVarChar });

        return PutData(SqlWithParams(SQL, SqlParams));
    }

    private HtmlTag MenuElement(string tag, string method, string @params, string styleclass) // @params to avoid keyword conflict
    {
        @params = !string.IsNullOrEmpty(@params.Trim()) ? @params.Trim() : "{params}";
        HtmlTag mnu = new HtmlTag();
        mnu.InnerText = Translator.Format(tag);
        mnu.SetAttribute(HtmlAttributes.@class, styleclass);
        mnu.SetAttribute(HtmlEvents.onclick, ByPassCall(method, @params));
        return mnu;
    }

    private Button BtnElement(string tag, string method, string @params, string styleclass) // @params to avoid keyword conflict
    {
        @params = !string.IsNullOrEmpty(@params.Trim()) ? @params.Trim() : "{params}";
        Button btn = new Button(Translator.Format(tag), Button.ButtonTypes.Button);
        btn.SetAttribute(HtmlAttributes.@class, styleclass);
        btn.SetAttribute(HtmlEvents.onclick, ByPassCall(method, @params));
        btn.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");
        return btn;
    }
}