using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient; // Using for SqlParameter
using skylite;
using skylite.ToolKit;

public class XysBulletinEV : WebBase
{
    public XysBulletinEV()
    {
        ViewFields.AddRange(new List<NameFlag>
        {
            new NameFlag { name = "BltnId", flag = true },
            new NameFlag { name = "BltnTitle" },
            new NameFlag { name = "BltnMemo" },
            new NameFlag { name = "CreatedBy" },
            new NameFlag { name = "Files", flag = true },
            new NameFlag { name = "FileRefId", flag = true }
        });
    }

    public override ViewPart InitialModel()
    {
        string SSQL = " Select BltnId, BltnTitle, BltnMemo, CreatedBy, dbo.XF_FileList(FileRefId) Files, FileRefId, SYSDTE, SYSUSR " +
                      " from XysBulletin where BltnId = " + PartialData;
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
        MenuList mnulist = SetPageMenu(new string[] { });
        Wrap BtnWrap = SetPageButtons(ViewPart.Mode == ViewMode.New ? new string[] { "save" } : new string[] { });

        Label label = new Label();
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
        label.Wrap.InnerText = ViewPart.Mode == ViewMode.New ? Translator.Format("newnotice") : Translator.Format("editnotice");

        FilterSection filter = new FilterSection();
        filter.ModalWrap = true;
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
        filter.Wrap.SetStyle(HtmlStyles.width, "90%");
        filter.Menu = mnulist;
        filter.FilterHtml = label.HtmlText;

        // Corrected Texts control instantiation as per SkyLite framework description
        Texts text = new Texts(Translator.Format("title"), TextTypes.text);
        text.Required = true;
        text.Text.SetAttribute(HtmlAttributes.name, ViewPart.Field("BltnTitle").name); // Name attribute explicitly set
        text.Text.SetStyle(HtmlStyles.width, "60%");
        text.Text.SetAttribute(HtmlAttributes.maxlength, "200");
        text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("BltnTitle").value);
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");
        text.Wrap.SetStyle(HtmlStyles.display, "block");

        TextArea text1 = new TextArea(Translator.Format("memo"));
        text1.Required = true;
        text1.Text.SetAttribute(HtmlAttributes.name, ViewPart.Field("BltnMemo").name); // Ensure TextArea also has a name
        text1.Text.SetStyle(HtmlStyles.width, "90%");
        text1.Text.SetStyle(HtmlStyles.height, "300px");
        text1.Text.SetAttribute(HtmlAttributes.id, ViewPart.Field("BltnMemo").name);
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "500");
        text1.Text.InnerText = ViewPart.Field("BltnMemo").value;
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        // Corrected Texts control instantiation as per SkyLite framework description
        Texts text2 = new Texts(Translator.Format("createdby"), TextTypes.text);
        text2.Required = true;
        text2.Text.SetAttribute(HtmlAttributes.name, ViewPart.Field("CreatedBy").name); // Name attribute explicitly set
        text2.Text.SetStyle(HtmlStyles.width, "50%");
        text2.Text.SetAttribute(HtmlAttributes.maxlength, "200");
        text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Mode == ViewMode.New ? AppKey.UserName : ViewPart.Field("CreatedBy").value);
        text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");
        text2.Wrap.SetStyle(HtmlStyles.display, "block");

        FileUpload fileRef = new FileUpload();
        fileRef.File.SetAttribute(HtmlAttributes.name, ViewPart.Field("Files").name); // Set the name for the file input
        fileRef.File.SetAttribute(HtmlAttributes.id, ViewPart.Field("Files").name);
        fileRef.Wrap.SetAttribute(HtmlAttributes.@class, "__filebtn");
        fileRef.Button.SetStyles("padding: 8px; border-radius: 4px; border: 1px solid rgb(68, 68, 68); border-image: none; color: rgb(68, 68, 68); font-size: 12px; cursor: pointer; background-color: rgb(255, 255, 255);");
        fileRef.Label.SetStyle(HtmlStyles.paddingLeft, "10px");
        fileRef.Label.InnerText = ViewPart.Field("Files").value;

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
        elmBox.AddItem(fileRef, 20);
        elmBox.AddItem(BtnWrap, 20);

        string ViewHtml = filter.HtmlText + elmBox.HtmlText;
        return ViewHtml;
    }

    public ApiResponse SaveView()
    {
        string BltnTitle = ViewPart.Field("BltnTitle").value;
        string BltnMemo = ViewPart.Field("BltnMemo").value;
        string CreatedBy = ViewPart.Field("CreatedBy").value;

        if (ViewPart.Mode == ViewMode.New) ViewPart.Field("FileRefId").value = NewID(1);

        ApiResponse _ApiResponse = new ApiResponse();
        if (string.IsNullOrEmpty(BltnTitle) || string.IsNullOrEmpty(BltnMemo) || string.IsNullOrEmpty(CreatedBy))
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
                rlt = UploadFile(ViewPart.Field("Files").name, ViewPart.Field("FileRefId").value);
                if (string.IsNullOrEmpty(rlt))
                {
                    DialogBox dialogBox = new DialogBox(Translator.Format("msg_saved"));
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                    dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysBulletinMV) + "&$PopOff();");
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
                }
                else
                {
                    DialogBox dialogBox = new DialogBox(rlt);
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
                }
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
                SQL.Add(" declare @seq int " +
                        " exec dbo.XP_NextSeq N'XysBulletin',N'BltnId',@seq out " +
                        " insert into XysBulletin(BltnId, BltnTitle, BltnMemo, CreatedBy, FileRefId, SYSDTE, SYSUSR) " +
                        " values(@seq,@BltnTitle,@BltnMemo,@CreatedBy,@FileRefId,getdate(),@SYSUSR)    ");
                break;
            case ViewMode.Edit:
                SQL.Add(" Update XysBulletin set " +
                        "   BltnTitle = @BltnTitle, BltnMemo = @BltnMemo, CreatedBy = @CreatedBy, " +
                        "   FileRefId = (case when @FileRefId='' then FileRefId else @FileRefId end), " +
                        "   SYSDTE = getdate(), SYSUSR = @SYSUSR " +
                        " Where  BltnId = @BltnId  ");
                break;
        }

        List<SqlParameter> SqlParams = new List<SqlParameter>();
        SqlParams.Add(new SqlParameter { ParameterName = "@BltnId", Value = ValC(ViewPart.Field("BltnId").value), SqlDbType = SqlDbType.Int });
        SqlParams.Add(new SqlParameter { ParameterName = "@BltnTitle", Value = ViewPart.Field("BltnTitle").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@BltnMemo", Value = ViewPart.Field("BltnMemo").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@CreatedBy", Value = ViewPart.Field("CreatedBy").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@FileRefId", Value = ViewPart.Field("FileRefId").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

        return PutData(SqlWithParams(SQL, SqlParams));
    }

    public ApiResponse DeleteView()
    {
        ApiResponse _ApiResponse = new ApiResponse();

        DialogBox dialogBox = new DialogBox(Translator.Format("deletepage"));
        dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;");
        dialogBox.AddButton(Translator.Format("yes"), string.Empty, "class:button1;onclick:" + ByPassCall("XysBulletinEV/DeleteViewConfirm"));
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
            dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysBulletinMV) + "&$PopOff();");
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
            " delete from XysBulletin where BltnId = @BltnId "
        };

        List<SqlParameter> SqlParams = new List<SqlParameter>();
        SqlParams.Add(new SqlParameter { ParameterName = "@BltnId", Value = ValC(ViewPart.Field("BltnId").value), SqlDbType = SqlDbType.Int });

        return PutData(SqlWithParams(SQL, SqlParams));
    }
}