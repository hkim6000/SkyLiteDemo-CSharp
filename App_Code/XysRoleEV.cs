using skylite;
using skylite.ToolKit;
using System.Collections.Generic;
using System.Data;
using System.Net;

public class XysRoleEV : WebBase
{
    public XysRoleEV()
    {
        ViewFields.AddRange(new[] {
            new NameFlag { name = "RoleId", flag = true },
            new NameFlag { name = "RoleName" },
            new NameFlag { name = "RoleAlias" },
            new NameFlag { name = "RoleOrder" }
        });
    }

    public override ViewPart InitialModel()
    {
        string emsg = string.Empty;

        ViewPart = new ViewPart();
        ViewPart.Methods = ViewMethods();
        if (!string.IsNullOrEmpty(PartialData))
        {
            ViewPart.Mode = ViewMode.Edit;
            ViewPart.Data = SQLData.SQLDataTable(@" Select RoleId,RoleName,RoleAlias,RoleOrder,SYSDTE,SYSUSR From XysRole Where  RoleId = '" + PartialData + "'",ref emsg);
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
        var mnulist = SetPageMenu(ViewPart.Mode == ViewMode.New ? new[] { "xysrole" } : new string[] { });
        var BtnWrap = SetPageButtons(ViewPart.Mode == ViewMode.New ? new[] { "save" } : new string[] { });

        var label = new Label();
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
        label.Wrap.InnerText = ViewPart.Mode == ViewMode.New ? Translator.Format("newrole") : Translator.Format("editrole");

        var filter = new skylite.ToolKit.FilterSection();
        filter.ModalWrap = true;
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
        filter.Wrap.SetStyle(HtmlStyles.width, "90%");
        filter.Menu = mnulist;
        filter.FilterHtml = label.HtmlText;

        var text = new Texts(Translator.Format("name"), ViewPart.Field("RoleName").name, TextTypes.text);
        text.Required = true;
        text.Text.SetStyle(HtmlStyles.width, "200px");
        text.Text.SetAttribute(HtmlAttributes.maxlength, "100");
        text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("RoleName").value);
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        var text1 = new Texts(Translator.Format("alias"), ViewPart.Field("RoleAlias").name, TextTypes.text);
        text1.Required = true;
        text1.Text.SetStyle(HtmlStyles.width, "200px");
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "10");
        text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("RoleAlias").value);
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        var sel1 = new Dropdown(Translator.Format("order"), ViewPart.Field("RoleOrder").name);
        sel1.Required = true;
        sel1.SelBox.SetStyle(HtmlStyles.width, "216px");
        sel1.SelOptions = new OptionValues("{0|0}{1|1}{2|2}{3|3}{4|4}{5|5}{6|6}{7|7}{8|8}{9|9}{10|10}", ViewPart.Field("RoleOrder").value);
        sel1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        var elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.width, "90%");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.marginTop, "8px");
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px");

        elmBox.AddItem(text, 1);
        elmBox.AddItem(text1, 20);
        elmBox.AddItem(sel1, 40);
        elmBox.AddItem(BtnWrap, 20);

        string ViewHtml = filter.HtmlText + elmBox.HtmlText;
        return ViewHtml;
    }

    public ApiResponse SaveView()
    {
        string rolename = ViewPart.Field("RoleName").value;
        string rolealias = ViewPart.Field("RoleAlias").value;
        string roleorder = ViewPart.Field("RoleOrder").value;

        var _ApiResponse = new ApiResponse();
        if (string.IsNullOrEmpty(rolename) || string.IsNullOrEmpty(rolealias) || string.IsNullOrEmpty(roleorder))
        {
            var dialogBox = new DialogBox(Translator.Format("msg_required"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
        }
        else
        {
            string rlt = PutSaveView();
            if (string.IsNullOrEmpty(rlt))
            {
                var dialogBox = new DialogBox(Translator.Format("msg_saved"));
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m={References.Pages.XysRole}") + "&$PopOff();");
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
            }
            else
            {
                var dialogBox = new DialogBox(rlt);
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
                ViewPart.Field("RoleId").value = NewID();
                SQL.Add(" Insert into XysRole( RoleId,RoleName,RoleAlias,RoleOrder,SYSDTE,SYSUSR) " +
                        " values ( @RoleId, @RoleName, @RoleAlias, @RoleOrder, getdate(), @SYSUSR) ");
                break;
            case ViewMode.Edit:
                SQL.Add(" Update XysRole set " +
                        " RoleName = @RoleName, RoleAlias = @RoleAlias, RoleOrder = @RoleOrder, " +
                        " SYSDTE = getdate(), SYSUSR = @SYSUSR Where RoleId = @RoleId ");
                break;
        }

        List<System.Data.SqlClient.SqlParameter> SqlParams = new List<System.Data.SqlClient.SqlParameter>();
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@RoleId", Value = ViewPart.Field("RoleId").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@RoleName", Value = ViewPart.Field("RoleName").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@RoleAlias", Value = ViewPart.Field("RoleAlias").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@RoleOrder", Value = ViewPart.Field("RoleOrder").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

        return PutData(SqlWithParams(SQL, SqlParams));
    }

    public ApiResponse DeleteView()
    {
        var _ApiResponse = new ApiResponse();

        if (IfExists())
        {
            var dialogBox = new DialogBox(Translator.Format("msg_exists"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
        }
        else
        {
            var dialogBox = new DialogBox(Translator.Format("deleterole"));
            dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;");
            dialogBox.AddButton(Translator.Format("yes"), string.Empty, "class:button1;onclick:" + ByPassCall("XysRoleEV/DeleteViewConfirm"));
            dialogBox.AddButton(Translator.Format("no"), string.Empty, "onclick:$PopOff();class:button;");
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
        }
        return _ApiResponse;
    }

    private bool IfExists()
    {
        bool rtnvlu = false;
        string emsg = string.Empty;
        DataTable dt = SQLData.SQLDataTable(" select count(*) from XysUserInfo where RoleId = '" + ViewPart.Field("RoleId").value + "'", ref emsg);
        if (dt != null && dt.Rows.Count > 0)
        {
            if (ValC(dt.Rows[0][0].ToString()) > 0)
            {
                rtnvlu = true;
            }
        }
        return rtnvlu;
    }
    public ApiResponse DeleteViewConfirm()
    {
        var _ApiResponse = new ApiResponse();
        string rlt = PutDeleteView();
        if (string.IsNullOrEmpty(rlt))
        {
            var dialogBox = new DialogBox(Translator.Format("msg_deleted"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m={References.Pages.XysRole}") + "&$PopOff();");
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
        }
        else
        {
            var dialogBox = new DialogBox(rlt);
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
        }
        return _ApiResponse;
    }

    private string PutDeleteView()
    {
        List<string> SQL = new List<string>()
            {
            " delete from XysRoleMenu where RoleId = @RoleId ",
            " delete from XysRolePage where RoleId = @RoleId ",
            " delete from XysRole where RoleId = @RoleId "
            };

        List<System.Data.SqlClient.SqlParameter> SqlParams = new List<System.Data.SqlClient.SqlParameter>();
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@RoleId", Value = ViewPart.Field("RoleId").value, SqlDbType = SqlDbType.NVarChar });

        return PutData(SqlWithParams(SQL, SqlParams));
    }
}