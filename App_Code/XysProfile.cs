using skylite;
using skylite.ToolKit;
using System.Collections.Generic;
using System.Data;
using System.IO;

public class XysProfile : WebBase
{
    public XysProfile()
    {
        ViewFields.AddRange(new[] {
            new NameFlag { name = "UserEmail", flag = true },
            new NameFlag { name = "UserName" },
            new NameFlag { name = "UserPhone" },
            new NameFlag { name = "UserRole", flag = true }
        });
    }

    public override ViewPart InitialModel()
    {
        string SSQL = " select UserEmail,UserName,UserPhone,dbo.XF_RoleName(RoleId) UserRole from XysUserInfo where UserId = '" + AppKey.UserId + "'";
        string emsg = string.Empty;

        ViewPart = new ViewPart();
        ViewPart.Methods = ViewMethods();
        ViewPart.Mode = ViewMode.Edit;
        ViewPart.Data = SQLData.SQLDataTable(SSQL, ref emsg);

        for (int i = 0; i < ViewFields.Count; i++)
        {
            ViewPart.Fields.Add(new NameValue { name = ViewFields[i].name, value = ViewPart.ColunmValue(ViewFields[i].name) });
        }

        return ViewPart;
    }

    public override string InitialView()
    {
        var mnulist = SetPageMenu(new string[] { });
        var BtnWrap = SetPageButtons(new string[] { });

        var label = new Label();
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
        label.Wrap.InnerText = Translator.Format("profile");

        var filter = new FilterSection();
        filter.ModalWrap = true;
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "100px");
        filter.Wrap.SetStyle(HtmlStyles.width, "95%");
        filter.Menu = mnulist;
        filter.FilterHtml = label.HtmlText;

        var txtid = new Label();
        txtid.Wrap.InnerText = Translator.Format("email") + " " + ViewPart.Field("UserEmail").value;
        txtid.Wrap.SetStyle(HtmlStyles.fontSize, "18px");
        txtid.Wrap.SetStyle(HtmlStyles.fontWeight, "bold");
        txtid.Wrap.SetStyle(HtmlStyles.color, "#444");
        txtid.Wrap.SetStyle(HtmlStyles.marginLeft, "8px");

        var txtrole = new Label();
        txtrole.Wrap.InnerText = Translator.Format("userrole") + " " + ViewPart.Field("UserRole").value;
        txtrole.Wrap.SetStyle(HtmlStyles.fontSize, "18px");
        txtrole.Wrap.SetStyle(HtmlStyles.fontWeight, "bold");
        txtrole.Wrap.SetStyle(HtmlStyles.color, "#444");
        txtrole.Wrap.SetStyle(HtmlStyles.marginLeft, "8px");

        var text = new Texts(Translator.Format("name"), "UserName", TextTypes.text);
        text.Text.SetAttribute(HtmlAttributes.name, ViewPart.Field("UserName").name);
        text.Required = true;
        text.Text.SetStyle(HtmlStyles.width, "200px");
        text.Text.SetAttribute(HtmlAttributes.maxlength, "100");
        text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserName").value);
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        var text1 = new Texts(Translator.Format("phone"), TextTypes.text);
        text1.Text.SetAttribute(HtmlAttributes.name, ViewPart.Field("UserPhone").name);
        text1.Text.SetStyle(HtmlStyles.width, "200px");
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "15");
        text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserPhone").value);
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

        var elmWrap = new HtmlWrapper();
        elmWrap.AddContents(txtrole, 10);
        elmWrap.AddContents(txtid, 30);
        elmWrap.AddContents(text, 1);
        elmWrap.AddContents(text1, 46);
        elmWrap.AddContents(BtnWrap);

        var col = new Wrap();
        col.SetStyle(HtmlStyles.marginLeft, "40px");
        col.InnerText = elmWrap.HtmlText;

        string imgfile = PhysicalFolder + "photos\\" + AppKey.UserId + ".jpg";
        if (!File.Exists(imgfile))
        {
            imgfile = ImagePath + "img_fakeuser.jpg";
        }
        else
        {
            imgfile = GetPhotoData(imgfile);
        }
        var img = new HtmlTag(HtmlTags.img, HtmlTag.Types.Empty);
        img.SetAttribute(HtmlAttributes.id, "UserPic");
        img.SetAttribute(HtmlAttributes.src, imgfile);
        img.SetStyle(HtmlStyles.borderRadius, "6px");
        img.SetStyle(HtmlStyles.width, "220px");
        img.SetStyle(HtmlStyles.height, "250px");
        img.SetStyle(HtmlStyles.objectFit, "cover");

        var imginput = new HtmlTag(HtmlTags.input, HtmlTag.Types.Empty);
        imginput.SetAttribute(HtmlAttributes.id, "UserFile");
        imginput.SetAttribute(HtmlAttributes.type, "file");
        imginput.SetAttribute(HtmlEvents.onchange, "UpdatePhoto('UserPic','UserFile','" + EncryptString(AppKey.UserId) + "')");
        imginput.SetStyles("left: 0px; top: 0px; width: 100%; height: 100%; color: transparent; position: absolute; cursor: pointer; opacity: 0;");
        var imgbtnwrap = new HtmlTag();
        imgbtnwrap.SetStyles("overflow: hidden; display: inline-block; border-radius:4px; width:36px; height:36px; position:absolute; bottom:0px;right:-2px;background-repeat: no-repeat;background-size: contain;");
        imgbtnwrap.SetStyle(HtmlStyles.backgroundImage, "url('" + ImagePath + "changephoto.jpg')");
        imgbtnwrap.InnerText = imginput.HtmlText;

        var imgWrap = new Wrap();
        imgWrap.SetStyle(HtmlStyles.position, "relative");
        imgWrap.SetStyle(HtmlStyles.width, "220px");
        imgWrap.SetStyle(HtmlStyles.height, "250px");
        imgWrap.InnerText = img.HtmlText + imgbtnwrap.HtmlText;

        var col1 = new Wrap();
        col1.SetStyle(HtmlStyles.padding, "20px");
        col1.InnerText = imgWrap.HtmlText;

        var colWrap = new Wrap();
        colWrap.SetStyle(HtmlStyles.width, "95%");
        colWrap.SetStyle(HtmlStyles.margin, "auto");
        colWrap.SetStyle(HtmlStyles.marginTop, "10px");
        colWrap.SetStyle(HtmlStyles.marginBottom, "80px");
        colWrap.SetStyle(HtmlStyles.borderRadius, "2px");
        colWrap.SetStyle(HtmlStyles.border, "1px solid #ddd");
        colWrap.SetStyle(HtmlStyles.boxShadow, "3px 4px 6px 1px rgba(0, 0, 0, 0.15)");
        colWrap.SetStyle(HtmlStyles.boxSizing, "border-box");
        colWrap.SetStyle(HtmlStyles.padding, "30px");
        colWrap.SetStyle(HtmlStyles.display, "flex");
        colWrap.InnerText = col1.HtmlText + col.HtmlText;

        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.UserIconScript);
        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TimerScript);
        HtmlDoc.InitialScripts.ExecuteScript("$StartTimer('tmr')");

        TitleSection2 PageLayout = PageTitle();
        PageLayout.ContentWrap.SetAttribute(HtmlAttributes.id, References.Elements.PageContents);
        PageLayout.ContentWrap.InnerText = filter.HtmlText + colWrap.HtmlText;

        return PageLayout.HtmlText;
    }

    public ApiResponse MenuClick()
    {
        string m = GetDataValue("m");
        string t = GetDataValue("t");

        var _ApiResponse = new ApiResponse();
        _ApiResponse.SetElementContents(References.Elements.PageContents, PartialDocument(m, t));
        _ApiResponse.ExecuteScript("$ScrollToTop()");
        return _ApiResponse;
    }

    public ApiResponse NaviClick()
    {
        string m = GetDataValue("m");
        var _ApiResponse = new ApiResponse();
        _ApiResponse.Navigate(m);
        return _ApiResponse;
    }

    public ApiResponse SaveView()
    {
        string UserName = ViewPart.Field("UserName").value;
        string UserPhone = ViewPart.Field("UserPhone").value;

        var _ApiResponse = new ApiResponse();
        if (string.IsNullOrEmpty(UserName))
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
                AppKey.UserName = UserName;
                AppKey.UserPhone = UserPhone;
                string SerializedString = SerializeObjectEnc(AppKey, typeof(AppKey));
                _ApiResponse.SetCookie(References.Keys.AppKey, SerializedString, 0);

                var dialogBox = new DialogBox(Translator.Format("msg_saved"));
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
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
        List<string> SQL = new List<string>()
        {
            " update XysUserInfo set UserName = @UserName,UserPhone=@UserPhone where UserId = @UserId ",
            " update XysUser set SYSDTE = getdate(),SYSUSR=@UserId where UserId = @UserId "
        };

        List<System.Data.SqlClient.SqlParameter> SqlParams = new List<System.Data.SqlClient.SqlParameter>()
        {
            new System.Data.SqlClient.SqlParameter { ParameterName = "@UserId", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar },
            new System.Data.SqlClient.SqlParameter { ParameterName = "@UserName", Value = ViewPart.Field("UserName").value, SqlDbType = SqlDbType.NVarChar },
            new System.Data.SqlClient.SqlParameter { ParameterName = "@UserPhone", Value = ViewPart.Field("UserPhone").value, SqlDbType = SqlDbType.NVarChar }
        };

        return PutData(SqlWithParams(SQL, SqlParams));
    }
}