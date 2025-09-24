using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using skylite;
using skylite.ToolKit;

public class XysDictEV : WebBase
{
    public XysDictEV()
    {
        ViewFields.AddRange(new List<NameFlag>
        {
            new NameFlag(){ name = "Target", flag = true },
            new NameFlag() { name = "IsoCode", flag = true },
            new NameFlag() { name = "KeyWord", flag = true },
            new NameFlag() { name = "Translated" }
        });
    }

    public override ViewPart InitialModel()
    {
        string sSQL = @" Select Target,IsoCode,KeyWord,Translated from XysDict where Target+IsoCode+KeyWord = N'{PartialData}'";
        string rlt = string.Empty;

        ViewPart = new ViewPart();
        ViewPart.Methods = ViewMethods();
        if (!string.IsNullOrEmpty(PartialData))
        {
            ViewPart.Mode = ViewMode.Edit;
            ViewPart.Data = (DataTable)SQLData.SQLDataTable(sSQL, ref rlt);
            ViewPart.Params = PartialData;
        }
        else
        {
            ViewPart.Mode = ViewMode.New;
        }

        foreach (var field in ViewFields)
        {
            ViewPart.Fields.Add(new NameValue { name = field.name, value = ViewPart.ColunmValue(field.name) });
        }

        return ViewPart;
    }

    public override string InitialView()
    {
        MenuList mnulist = SetPageMenu(new string[] { });
        Wrap btnWrap = SetPageButtons(ViewPart.Mode == ViewMode.New ? new string[] { "save" } : new string[] { });

        Label label = new Label();
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
        label.Wrap.InnerText = ViewPart.Mode == ViewMode.New ? Translator.Format("newdict") : Translator.Format("editdict");

        FilterSection filter = new FilterSection();
        filter.ModalWrap = true;
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
        filter.Wrap.SetStyle(HtmlStyles.width, "90%");
        filter.Menu = mnulist;
        filter.FilterHtml = label.HtmlText;

        Label lbl = new Label(Translator.Format("target"));
        lbl.Wrap.SetStyles("padding-left:8px;color:#777;");
        Label vlbl = new Label(ViewPart.Field("Target").value);
        vlbl.Wrap.SetStyles("padding-left:8px;font-weight:bold;font-size:20px;");

        Label lbl1 = new Label(Translator.Format("isocode"));
        lbl1.Wrap.SetStyles("padding-left:8px;color:#777;");
        Label vlbl1 = new Label(ViewPart.Field("IsoCode").value);
        vlbl1.Wrap.SetStyles("padding-left:8px;font-weight:bold;font-size:20px;");

        Label lbl2 = new Label(Translator.Format("keyword"));
        lbl2.Wrap.SetStyles("padding-left:8px;color:#777;");
        Label vlbl2 = new Label(ViewPart.Field("KeyWord").value);
        vlbl2.Wrap.SetStyles("padding-left:8px;font-weight:bold;font-size:20px;");

        Texts text = new Texts(Translator.Format("translated"), ViewPart.Field("Translated").name, TextTypes.text);
        text.Required = true;
        text.Text.SetStyle(HtmlStyles.width, "200px");
        text.Text.SetAttribute(HtmlAttributes.maxlength, "200");
        text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("Translated").value);
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        HtmlElementBox elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.width, "90%");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.marginTop, "8px");
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px");

        elmBox.AddItem(lbl, 10);
        elmBox.AddItem(vlbl, 20);
        elmBox.AddItem(lbl1, 10);
        elmBox.AddItem(vlbl1, 20);
        elmBox.AddItem(lbl2, 10);
        elmBox.AddItem(vlbl2, 20);
        elmBox.AddItem(text, 40);
        elmBox.AddItem(btnWrap, 20);

        string viewHtml = filter.HtmlText + elmBox.HtmlText;
        return viewHtml;
    }

    public ApiResponse SaveView()
    {
        string translated = ViewPart.Field("Translated").value;

        ApiResponse apiResponse = new ApiResponse();
        if (string.IsNullOrEmpty(translated))
        {
            DialogBox dialogBox = new DialogBox(Translator.Format("msg_required"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            apiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
        }
        else
        {
            string rlt = PutSaveView();
            if (string.IsNullOrEmpty(rlt))
            {
                DialogBox dialogBox = new DialogBox(Translator.Format("msg_saved"));
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysDict) + "&$PopOff();");
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
                ViewPart.Field("PageId").value = NewID();
                sql.Add(" Insert into XysDict(Target,IsoCode,KeyWord,Translated) " +
                        " values( @Target,@IsoCode,@KeyWord,@Translated) ");
                break;
            case ViewMode.Edit:
                sql.Add(" Update XysDict set " +
                        " Translated = @Translated " +
                        " WHERE Target+IsoCode+KeyWord = @KeyData");
                break;
        }

        List<SqlParameter> sqlParams = new List<SqlParameter>
        {
            new SqlParameter { ParameterName = "@KeyData", Value = ViewPart.Params, SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@IsoCode", Value = ViewPart.Field("IsoCode").value, SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@KeyWord", Value = ViewPart.Field("KeyWord").value, SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@Translated", Value = ViewPart.Field("Translated").value, SqlDbType = SqlDbType.NVarChar }
        };

        return PutData(SqlWithParams(sql, sqlParams));
    }

    public ApiResponse DeleteView()
    {
        ApiResponse apiResponse = new ApiResponse();

        DialogBox dialogBox = new DialogBox(Translator.Format("deletepage"));
        dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;");
        dialogBox.AddButton(Translator.Format("yes"), string.Empty, "class:button1;onclick:" + ByPassCall("XysDictEV/DeleteViewConfirm"));
        dialogBox.AddButton(Translator.Format("no"), string.Empty, "onclick:$PopOff();class:button;");
        apiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);

        return apiResponse;
    }

    public ApiResponse DeleteViewConfirm()
    {
        ApiResponse apiResponse = new ApiResponse();
        string rlt = PutDeleteViewData();
        if (string.IsNullOrEmpty(rlt))
        {
            DialogBox dialogBox = new DialogBox(Translator.Format("msg_deleted"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysDict) + "&$PopOff();");
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
            " delete from XysDict where Target+IsoCode+KeyWord = @KeyData "
        };

        List<SqlParameter> sqlParams = new List<SqlParameter>
        {
            new SqlParameter { ParameterName = "@KeyData", Value = ViewPart.Params, SqlDbType = SqlDbType.NVarChar }
        };

        return PutData(SqlWithParams(sql, sqlParams));
    }
}