using skylite;
using skylite.ToolKit;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Generic;
using System;

public class XysUserEV : WebBase
{
    public XysUserEV()
    {
        ViewFields.AddRange(new NameFlag[]
        {
            new NameFlag { name = "UserId", flag = true },
            new NameFlag { name = "UserPwd" },
            new NameFlag { name = "UserOTP" },
            new NameFlag { name = "UserStatus" },
            new NameFlag { name = "LevelCode" },
            new NameFlag { name = "UserEmail" },
            new NameFlag { name = "UserName" },
            new NameFlag { name = "UserPhone" },
            new NameFlag { name = "RoleId" }
        });
    }

    public override ViewPart InitialModel()
    {
        string sSQL = " select a.UserId,a.UserPwd,a.UserOTP, a.UserStatus, a.LevelCode, b.UserEmail,b.UserName,b.UserPhone,b.RoleId " +
                      " from XysUser a inner join XysUserInfo b on a.UserId = b.UserId " +
                      " where a.UserId = N'" + PartialData + "'";
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

        ViewPart.Field("UserPwd").value = Encryptor.DecryptData(ViewPart.Field("UserPwd").value);

        return ViewPart;
    }

    public override string InitialView()
    {
        MenuList mnulist = SetPageMenu(new string[] { });
        Wrap BtnWrap = SetPageButtons(ViewPart.Mode == ViewMode.New ? new string[] { "save" } : new string[] { });

        Label label = new Label();
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
        label.Wrap.InnerText = ViewPart.Mode == ViewMode.New ? Translator.Format("newuser") : Translator.Format("edituser");

        FilterSection filter = new FilterSection();
        filter.ModalWrap = true;
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
        filter.Wrap.SetStyle(HtmlStyles.width, "90%");
        filter.Menu = mnulist;
        filter.FilterHtml = label.HtmlText;

        Texts text = new Texts(Translator.Format("email"), ViewPart.Field("UserEmail").name, TextTypes.text);
        text.Required = true;
        text.Text.SetStyle(HtmlStyles.width, "300px");
        text.Text.SetAttribute(HtmlAttributes.maxlength, "200");
        text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserEmail").value);
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Texts text1 = new Texts(Translator.Format("name"), ViewPart.Field("UserName").name, TextTypes.text);
        text1.Required = true;
        text1.Text.SetStyle(HtmlStyles.width, "200px");
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "10");
        text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserName").value);
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Texts text2 = new Texts(Translator.Format("phone"), ViewPart.Field("UserPhone").name, TextTypes.text);
        text2.Text.SetStyle(HtmlStyles.width, "200px");
        text2.Text.SetAttribute(HtmlAttributes.maxlength, "16");
        text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserPhone").value);
        text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Dropdown sel1 = new Dropdown(Translator.Format("role"), ViewPart.Field("RoleId").name);
        sel1.Required = true;
        sel1.SelBox.SetStyle(HtmlStyles.width, "216px");
        sel1.SelOptions = new OptionValues("sql@select RoleId,RoleName from XysRole order by RoleOrder", ViewPart.Field("RoleId").value);
        sel1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Texts text3 = new Texts(Translator.Format("pwd"), ViewPart.Field("UserPwd").name, TextTypes.text);
        text3.Required = true;
        text3.Text.SetStyle(HtmlStyles.width, "200px");
        text3.Text.SetAttribute(HtmlAttributes.maxlength, "32");
        text3.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserPwd").value);
        text3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        CheckBox chk1 = new CheckBox(Translator.Format("mfa"));
        chk1.Checks.AddItem(ViewPart.Field("UserOTP").name, "1", string.Empty, ValC(ViewPart.Field("UserOTP").value) == 1 ? true : false);
        chk1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Dropdown sel3 = new Dropdown(Translator.Format("levelcode"), ViewPart.Field("LevelCode").name);
        sel3.SelBox.SetStyle(HtmlStyles.width, "216px");
        sel3.SelOptions = new OptionValues("SQL@select LevelCode, LevelName from XysLevel order by LevelCode", ViewPart.Field("LevelCode").value);
        sel3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        Dropdown sel2 = new Dropdown(Translator.Format("status"), ViewPart.Field("UserStatus").name);
        sel2.SelBox.SetStyle(HtmlStyles.width, "216px");
        sel2.SelOptions = new OptionValues("{0|Normal}{8|Suspended}{9|Terminated}", ViewPart.Field("UserStatus").value);
        sel2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        HtmlWrapper elmWrap = new HtmlWrapper();
        elmWrap.AddContents(text, 1);
        elmWrap.AddContents(text1, 1);
        elmWrap.AddContents(text2, 10);
        elmWrap.AddContents(sel1, 1);
        elmWrap.AddContents(text3, 10);
        elmWrap.AddContents(chk1, 10);
        elmWrap.AddContents(sel3, 26);
        elmWrap.AddContents(sel2, 46);
        elmWrap.AddContents(BtnWrap, 20);

        Wrap col = new Wrap();
        col.SetStyle(HtmlStyles.marginLeft, "40px");
        col.InnerText = elmWrap.HtmlText;

        string imgfile = PhysicalFolder + "photos\\" + ViewPart.Field("UserId").value + ".jpg";
        if (!File.Exists(imgfile))
        {
            imgfile = ImagePath + "img_fakeuser.jpg";
        }
        else
        {
            imgfile = GetPhotoData(imgfile);
        }
        HtmlTag img = new HtmlTag(HtmlTags.img, HtmlTag.Types.Empty);
        img.SetAttribute(HtmlAttributes.id, "UserPic");
        img.SetAttribute(HtmlAttributes.src, imgfile);
        img.SetStyle(HtmlStyles.borderRadius, "6px");
        img.SetStyle(HtmlStyles.width, "220px");
        img.SetStyle(HtmlStyles.height, "250px");
        img.SetStyle(HtmlStyles.objectFit, "cover");

        HtmlTag imginput = new HtmlTag(HtmlTags.input, HtmlTag.Types.Empty);
        imginput.SetAttribute(HtmlAttributes.id, "UserFile");
        imginput.SetAttribute(HtmlAttributes.type, "file");
        imginput.SetAttribute(HtmlEvents.onchange, "UpdatePhoto('UserPic','UserFile','" + EncryptString(ViewPart.Field("UserId").value) + "')");
        imginput.SetStyles("left: 0px; top: 0px; width: 100%; height: 100%; color: transparent; position: absolute; cursor: pointer; opacity: 0;");
        HtmlTag imgbtnwrap = new HtmlTag();
        imgbtnwrap.SetStyles("overflow: hidden; display: inline-block; border-radius:4px; width:36px; height:36px; position:absolute; bottom:0px;right:-2px;background-repeat: no-repeat;background-size: contain;");
        imgbtnwrap.SetStyle(HtmlStyles.backgroundImage, "url('" + ImagePath + "changephoto.jpg')");
        imgbtnwrap.InnerText = imginput.HtmlText;

        Wrap imgWrap = new Wrap();
        imgWrap.SetStyle(HtmlStyles.position, "relative");
        imgWrap.SetStyle(HtmlStyles.width, "220px");
        imgWrap.SetStyle(HtmlStyles.height, "250px");
        imgWrap.InnerText = img.HtmlText + imgbtnwrap.HtmlText;

        Wrap col1 = new Wrap();
        col1.SetStyle(HtmlStyles.padding, "20px");
        col1.InnerText = imgWrap.HtmlText;

        Wrap colWrap = new Wrap();
        colWrap.SetStyle(HtmlStyles.width, "90%");
        colWrap.SetStyle(HtmlStyles.margin, "auto");
        colWrap.SetStyle(HtmlStyles.marginTop, "10px");
        colWrap.SetStyle(HtmlStyles.marginBottom, "80px");
        colWrap.SetStyle(HtmlStyles.borderRadius, "2px");
        colWrap.SetStyle(HtmlStyles.border, "1px solid #ddd");
        colWrap.SetStyle(HtmlStyles.boxShadow, "3px 4px 6px 1px rgba(0, 0, 0, 0.15)");
        colWrap.SetStyle(HtmlStyles.boxSizing, "border-box");
        colWrap.SetStyle(HtmlStyles.padding, "30px");
        colWrap.SetStyle(HtmlStyles.display, "flex");
        colWrap.InnerText = (ViewPart.Mode == ViewMode.New ? string.Empty : col1.HtmlText) + col.HtmlText;

        string ViewHtml = filter.HtmlText + colWrap.HtmlText;
        return ViewHtml;
    }

    public ApiResponse SaveView()
    {
        string UserOTP = ViewPart.Field("UserOTP").value;
        string UserPwd = ViewPart.Field("UserPwd").value;
        string UserEmail = ViewPart.Field("UserEmail").value;
        string UserName = ViewPart.Field("UserName").value;
        string UserPhone = ViewPart.Field("UserPhone").value;
        string LevelCode = ViewPart.Field("LevelCode").value;
        string RoleId = ViewPart.Field("RoleId").value;

        ApiResponse _ApiResponse = new ApiResponse();
        if (UserPwd == string.Empty || UserEmail == string.Empty || UserName == string.Empty || RoleId == string.Empty)
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
                dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysUser) + "&$PopOff();");
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
                ViewPart.Field("UserId").value = NewID();
                SQL.Add(" Insert into XysUser( UserId,UserPwd,UserOTP,UserStatus,LevelCode,PassChanged,Created,Closed,SYSDTE,SYSUSR) " +
                        " values ( @UserId, @UserPwd, @UserOTP, @UserStatus,@LevelCode, getdate(), getdate(), getdate(), getdate(), @SYSUSR) ");
                SQL.Add(" Insert into XysUserInfo( UserId,UserName,UserDesc,UserEmail,UserPhone,UserPic,RoleId,UserRef) " +
                        " values ( @UserId, @UserName, N'', @UserEmail, @UserPhone, N'', @RoleId, N'') ");
                break;
            case ViewMode.Edit:
                SQL.Add(" update XysUserInfo set UserEmail = @UserEmail,UserName = @UserName,UserPhone=@UserPhone,RoleId=@RoleId where UserId = @UserId ");
                SQL.Add(" update XysUser set UserPwd=@UserPwd,UserOTP=@UserOTP,UserStatus=@UserStatus,LevelCode=@LevelCode, SYSDTE = getdate(),SYSUSR=@SYSUSR where UserId = @UserId ");
                break;
        }

        List<System.Data.SqlClient.SqlParameter> SqlParams = new List<System.Data.SqlClient.SqlParameter>();
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@UserId", Value = ViewPart.Field("UserId").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@UserPwd", Value = Encryptor.EncryptData(ViewPart.Field("UserPwd").value), SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@UserOTP", Value = ValC(ViewPart.Field("UserOTP").value), SqlDbType = SqlDbType.Int });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@UserStatus", Value = ValC(ViewPart.Field("UserStatus").value), SqlDbType = SqlDbType.Int });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@LevelCode", Value = ValC(ViewPart.Field("LevelCode").value), SqlDbType = SqlDbType.Int });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@UserEmail", Value = ViewPart.Field("UserEmail").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@UserName", Value = ViewPart.Field("UserName").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@UserPhone", Value = ViewPart.Field("UserPhone").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@RoleId", Value = ViewPart.Field("RoleId").value, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

        return PutData(SqlWithParams(SQL, SqlParams));
    }

    public ApiResponse DeleteView()
    {
        ApiResponse _ApiResponse = new ApiResponse();

        DialogBox dialogBox = new DialogBox(Translator.Format("deleteuser"));
        dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;");
        dialogBox.AddButton(Translator.Format("yes"), string.Empty, "class:button1;onclick:" + ByPassCall("XysUserEV/DeleteViewConfirm"));
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
            dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysUser) + "&$PopOff();");
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
            " delete from XysUserReset where UserId = @UserId ",
            " delete from XysUserInfo where UserId = @UserId ",
            " delete from XysUser where UserId = @UserId "
        };

        List<System.Data.SqlClient.SqlParameter> SqlParams = new List<System.Data.SqlClient.SqlParameter>();
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@UserId", Value = ViewPart.Field("UserId").value, SqlDbType = SqlDbType.NVarChar });

        return PutData(SqlWithParams(SQL, SqlParams));
    }
}