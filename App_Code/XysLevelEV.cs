using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using skylite;
using skylite.ToolKit;

public class XysLevelEV : WebBase
{
    public XysLevelEV()
    {
        ViewFields.AddRange(new List<NameFlag>
        {
            new NameFlag { name = "LevelCode" },
            new NameFlag { name = "LevelName" },
            new NameFlag { name = "LevelDesc" },
            new NameFlag { name = "LevelFlag" }
        });
    }

    public override ViewPart InitialModel()
    {
        string sSQL = @" Select LevelCode,LevelName,LevelDesc,LevelFlag From XysLevel where LevelCode = N'" + PartialData+ "'";
        string emsg = string.Empty;

        ViewPart = new ViewPart();
        ViewPart.Methods = ViewMethods();
        if (PartialData != string.Empty)
        {
            ViewPart.Mode = ViewMode.Edit;
            ViewPart.Data = SQLData.SQLDataTable(sSQL, ref emsg);
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
        MenuList mnulist = SetPageMenu(new string[]{});
        Wrap btnWrap = SetPageButtons(ViewPart.Mode == ViewMode.New ? new string[] { "save" } : new string[]{});

        Label label = new Label();
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
        label.Wrap.InnerText = ViewPart.Mode == ViewMode.New ? Translator.Format("newlevel") : Translator.Format("editlevel");

        FilterSection filter = new FilterSection();
        filter.ModalWrap = true;
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
        filter.Wrap.SetStyle(HtmlStyles.width, "90%");
        filter.Menu = mnulist;
        filter.FilterHtml = label.HtmlText;

        Texts text = new Texts(Translator.Format("code"), TextTypes.text);
        text.Text.SetAttribute(HtmlAttributes.name, ViewPart.Field("LevelCode").name); // Enforcing mandatory name attribute on Text property
        text.Required = true;
        text.Text.SetStyle(HtmlStyles.width, "200px");
        text.Text.SetAttribute(HtmlAttributes.maxlength, "200");
        text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("LevelCode").value);
        if (ViewPart.Mode == ViewMode.Edit)
        {
            text.Text.SetAttribute(HtmlAttributes.disabled, "disabled");
        }
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Texts text1 = new Texts(Translator.Format("name"), TextTypes.text);
        text1.Text.SetAttribute(HtmlAttributes.name, ViewPart.Field("LevelName").name); // Enforcing mandatory name attribute on Text property
        text1.Required = true;
        text1.Text.SetStyle(HtmlStyles.width, "200px");
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "50");
        text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("LevelName").value);
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Texts text2 = new Texts(Translator.Format("desc"), TextTypes.text);
        text2.Text.SetAttribute(HtmlAttributes.name, ViewPart.Field("LevelDesc").name); // Enforcing mandatory name attribute on Text property
        text2.Text.SetStyle(HtmlStyles.width, "300px");
        text2.Text.SetAttribute(HtmlAttributes.maxlength, "200");
        text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("LevelDesc").value);
        text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");
         
        CheckBox chk1 = new CheckBox(Translator.Format("flag"));
        chk1.Checks.AddItem(ViewPart.Field("LevelFlag").name, "1", string.Empty, (ValC(ViewPart.Field("LevelFlag").value) == 1) ? true : false);
        chk1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");
         
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
        elmBox.AddItem(chk1, 50);
        elmBox.AddItem(btnWrap, 20);

        string viewHtml = filter.HtmlText + elmBox.HtmlText;
        return viewHtml;
    }

    public ApiResponse SaveView()
    {
        string levelCode = ViewPart.Field("LevelCode").value;
        string levelName = ViewPart.Field("LevelName").value;
        string levelDesc = ViewPart.Field("LevelDesc").value;
        string levelFlag = ViewPart.Field("LevelFlag").value;

        ApiResponse apiResponse = new ApiResponse();
        if (string.IsNullOrEmpty(levelCode) || string.IsNullOrEmpty(levelName))
        {
            DialogBox dialogBox = new DialogBox(Translator.Format("msg_required"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            apiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
        }
        else
        {
            string rlt = PutSaveView();
            if (rlt == string.Empty)
            {
                DialogBox dialogBox = new DialogBox(Translator.Format("msg_saved"));
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysLevel) + "&$PopOff();");
                apiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
            }
            else
            {
                DialogBox dialogBox = new DialogBox(rlt);
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                apiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
            }
        }

        return apiResponse;
    }

    private string PutSaveView()
    {
        List<string> sql = new List<string>();

        switch (ViewPart.Mode)
        {
            case ViewMode.New:
                sql.Add(" Insert into XysLevel( LevelCode,LevelName,LevelDesc,LevelFlag ,SYSDTE,SYSUSR) " +
                        " values( @LevelCode, @LevelName, @LevelDesc, @LevelFlag, getdate(), @SYSUSR) ");
                break;
            case ViewMode.Edit:
                sql.Add(" Update XysLevel set " +
                        " LevelName = @LevelName, LevelDesc = @LevelDesc, LevelFlag = @LevelFlag, " +
                        " SYSDTE = getdate(), SYSUSR = @SYSUSR " +
                        " WHERE LevelCode = @LevelCode");
                break;
        }

        List<SqlParameter> sqlParams = new List<SqlParameter>();
        sqlParams.Add(new SqlParameter { ParameterName = "@LevelCode", Value = ViewPart.Field("LevelCode").value, SqlDbType = SqlDbType.NVarChar });
        sqlParams.Add(new SqlParameter { ParameterName = "@LevelName", Value = ViewPart.Field("LevelName").value, SqlDbType = SqlDbType.NVarChar });
        sqlParams.Add(new SqlParameter { ParameterName = "@LevelDesc", Value = ViewPart.Field("LevelDesc").value, SqlDbType = SqlDbType.NVarChar });
        sqlParams.Add(new SqlParameter { ParameterName = "@LevelFlag", Value = ValC(ViewPart.Field("LevelFlag").value).ToString(), SqlDbType = SqlDbType.Int });
        sqlParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

        return PutData(SqlWithParams(sql, sqlParams));
    }

    public ApiResponse DeleteView()
    {
        ApiResponse apiResponse = new ApiResponse();

        DialogBox dialogBox = new DialogBox(Translator.Format("deleteitem"));
        dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;");
        dialogBox.AddButton(Translator.Format("yes"), string.Empty, "class:button1;onclick:" + ByPassCall("XysLevelEV/DeleteViewConfirm"));
        dialogBox.AddButton(Translator.Format("no"), string.Empty, "onclick:$PopOff();class:button;");
        apiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);

        return apiResponse;
    }

    public ApiResponse DeleteViewConfirm()
    {
        ApiResponse apiResponse = new ApiResponse();
        string rlt = PutDeleteViewData();
        if (rlt == string.Empty)
        {
            DialogBox dialogBox = new DialogBox(Translator.Format("msg_deleted"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysLevel) + "&$PopOff();");
            apiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
        }
        else
        {
            DialogBox dialogBox = new DialogBox(rlt);
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            apiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
        }
        return apiResponse;
    }

    private string PutDeleteViewData()
    {
        List<string> sql = new List<string>
        {
            " delete from XysLevel where LevelCode = @LevelCode "
        };

        List<SqlParameter> sqlParams = new List<SqlParameter>();
        sqlParams.Add(new SqlParameter { ParameterName = "@LevelCode", Value = ViewPart.Field("LevelCode").value, SqlDbType = SqlDbType.NVarChar });

        return PutData(SqlWithParams(sql, sqlParams));
    }
}