using skylite;
using skylite.ToolKit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class XysLangEV : WebBase
{
    public XysLangEV()
    {
        ViewFields.AddRange(new List<NameFlag>
        {
            new NameFlag { name = "CODE", flag = false },
            new NameFlag { name = "SNO" },
            new NameFlag { name = "SD01" },
            new NameFlag { name = "SD02" },
            new NameFlag { name = "SD03" },
            new NameFlag { name = "SD04" },
            new NameFlag { name = "SD05" },
            new NameFlag { name = "SD06" },
            new NameFlag { name = "SD07" }
        });
    }

    public override ViewPart InitialModel()
    {
        string SSQL = " Select CODE, SNO, SD01, SD02, SD03, SD04, SD05, SD06, SD07  From XysOption   " +
                      " where CODE+convert(varchar(18), SNO) = N'" + PartialData + "'";
        string emsg = string.Empty; 
        
        ViewPart = new ViewPart();
        ViewPart.Methods = ViewMethods();
        if (PartialData != string.Empty)
        {
            ViewPart.Mode = ViewMode.Edit;
            ViewPart.Data = SQLData.SQLDataTable(SSQL,ref emsg);
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
        label.Wrap.InnerText = ViewPart.Mode == ViewMode.New ? Translator.Format("newlang") : Translator.Format("editlang");

        FilterSection filter = new FilterSection();
        filter.ModalWrap = true;
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
        filter.Wrap.SetStyle(HtmlStyles.width, "90%");
        filter.Menu = mnulist;
        filter.FilterHtml = label.HtmlText;

        Texts text = new Texts(Translator.Format("code"), ViewPart.Field("CODE").name, TextTypes.text);
        text.Required = true;
        text.Text.SetStyle(HtmlStyles.width, "100px");
        text.Text.SetAttribute(HtmlAttributes.maxlength, "10");
        text.Text.SetAttribute(HtmlAttributes.value, "ISO639");
        text.Text.SetAttribute(HtmlAttributes.disabled, "disabled");
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Texts text1 = new Texts(Translator.Format("no."), ViewPart.Field("SNO").name, TextTypes.text);
        text1.Required = true;
        text1.Text.SetStyle(HtmlStyles.width, "100px");
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "10");
        text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SNO").value);
        if (ViewPart.Mode == ViewMode.Edit) text1.Text.SetAttribute(HtmlAttributes.disabled, "disabled");
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Texts text2 = new Texts(Translator.Format("iso"), ViewPart.Field("SD01").name, TextTypes.text);
        text2.Required = true;
        text2.Text.SetStyle(HtmlStyles.width, "100px");
        text2.Text.SetAttribute(HtmlAttributes.maxlength, "10");
        text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SD01").value);
        text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Texts text3 = new Texts(Translator.Format("locale"), ViewPart.Field("SD02").name, TextTypes.text);
        text3.Required = true;
        text3.Text.SetStyle(HtmlStyles.width, "200px");
        text3.Text.SetAttribute(HtmlAttributes.maxlength, "500");
        text3.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SD02").value);
        text3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        CheckBox chk1 = new CheckBox(Translator.Format("use"));
        if (ViewPart.Mode == ViewMode.Edit)
        {
            chk1.Checks.AddItem(ViewPart.Field("SD03").name, "1", string.Empty, ValC(ViewPart.Field("SD03").value) == 1 ? true : false);
        }
        else
        {
            chk1.Checks.AddItem(ViewPart.Field("SD03").name, "1", string.Empty, true);
        }
        chk1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        HtmlElementBox elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.width, "90%");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.marginTop, "8px");
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px");

        elmBox.AddItem(text);
        elmBox.AddItem(text1, 20);
        elmBox.AddItem(text2, 1);
        elmBox.AddItem(text3, 10);
        elmBox.AddItem(chk1, 50);
        elmBox.AddItem(BtnWrap, 20);

        string ViewHtml = filter.HtmlText + elmBox.HtmlText;
        return ViewHtml;
    }

    public ApiResponse SaveView()
    {
        string CODE = ViewPart.Field("CODE").value;
        string SNO = ViewPart.Field("SNO").value;
        string SD01 = ViewPart.Field("SD01").value;
        string SD02 = ViewPart.Field("SD02").value;
        string SD03 = ViewPart.Field("SD03").value;

        ApiResponse _ApiResponse = new ApiResponse();
        if (string.IsNullOrEmpty(CODE) || string.IsNullOrEmpty(SNO) || string.IsNullOrEmpty(SD01) || string.IsNullOrEmpty(SD02) || string.IsNullOrEmpty(SD03))
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
                dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysLang) + "&$PopOff();");
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
                SQL.Add(" insert into XysOption(CODE, SNO, SD01, SD02, SD03, SD04, SD05, SD06, SD07) " +
                        " values(@CODE,@SNO,@SD01,@SD02,@SD03,@SD04,@SD05,@SD06,@SD07) ");
                break;
            case ViewMode.Edit:
                SQL.Add(" Update XysOption set " +
                        "   SD01 = @SD01, " +
                        "   SD02 = @SD02, " +
                        "   SD03 = @SD03, " +
                        "   SD04 = @SD04, " +
                        "   SD05 = @SD05, " +
                        "   SD06 = @SD06, " +
                        "   SD07 = @SD07 " +
                        " Where  CODE = @CODE and SNO = @SNO  ");
                break;
        }

        List<System.Data.SqlClient.SqlParameter> SqlParams = new List<System.Data.SqlClient.SqlParameter>();
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@CODE", Value = ViewPart.Field("CODE").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@SNO", Value = ViewPart.Field("SNO").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@SD01", Value = ViewPart.Field("SD01").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@SD02", Value = ViewPart.Field("SD02").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@SD03", Value = ViewPart.Field("SD03").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@SD04", Value = ViewPart.Field("SD04").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@SD05", Value = ViewPart.Field("SD05").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@SD06", Value = ViewPart.Field("SD06").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@SD07", Value = ViewPart.Field("SD07").value, SqlDbType = SqlDbType.NVarChar });

        List<string> lst = SqlWithParams(SQL, SqlParams);
        return PutData(lst);
    }

    public ApiResponse DeleteView()
    {
        ApiResponse _ApiResponse = new ApiResponse();

        DialogBox dialogBox = new DialogBox(Translator.Format("deletepage"));
        dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;");
        dialogBox.AddButton(Translator.Format("yes"), string.Empty, "class:button1;onclick:" + ByPassCall("XysLangEv/DeleteViewConfirm"));
        dialogBox.AddButton(Translator.Format("no"), string.Empty, "onclick:$PopOff();class:button;");
        _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);

        return _ApiResponse;
    }

    public ApiResponse DeleteViewConfirm()
    {
        ApiResponse _ApiResponse = new ApiResponse();
        string rlt = PutDeleteViewData();
        if (string.IsNullOrEmpty(rlt))
        {
            DialogBox dialogBox = new DialogBox(Translator.Format("msg_deleted"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysLang) + "&$PopOff();");
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
            " delete from XysOption where  CODE = @CODE and SNO = @SNO  "
        };

        List<System.Data.SqlClient.SqlParameter> SqlParams = new List<System.Data.SqlClient.SqlParameter>();
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@CODE", Value = ViewPart.Field("CODE").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@SNO", Value = ViewPart.Field("SNO").value, SqlDbType = SqlDbType.NVarChar });

        List<string> lst = SqlWithParams(SQL, SqlParams);
        return PutData(lst);
    }
}