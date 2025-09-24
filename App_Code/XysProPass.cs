using skylite;
using skylite.ToolKit;
using System.Collections.Generic;
using System.Data;
using System.IO;

public class XysProPass : WebBase
{
    public override string InitialView()
    {
        var mnulist = SetPageMenu(new string[] { });
        var BtnWrap = SetPageButtons(new string[] { });

        var label = new Label();
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
        label.Wrap.InnerText = Translator.Format("changepwd");

        var filter = new FilterSection();
        filter.ModalWrap = true;
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "100px");
        filter.Wrap.SetStyle(HtmlStyles.width, "95%");
        filter.Menu = mnulist;
        filter.FilterHtml = label.HtmlText;

        var lbl1 = new Label(Translator.Format("enterpwd"));
        lbl1.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px");
        lbl1.Wrap.SetStyle(HtmlStyles.color, "#444");

        var text = new Texts(Translator.Format("pwd"), "pwd", TextTypes.password);
        text.Text.SetAttribute(HtmlAttributes.name, "pwd");
        text.Required = true;
        text.Text.SetStyle(HtmlStyles.width, "180px");
        text.Text.SetAttribute(HtmlAttributes.maxlength, "15");
        text.Text.SetAttribute(HtmlAttributes.autocomplete, "off");

        var text1 = new Texts(Translator.Format("pwd1"), TextTypes.password);
        text1.Text.SetAttribute(HtmlAttributes.name, "pwd1");
        text1.Required = true;
        text1.Text.SetStyle(HtmlStyles.width, "180px");
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "15");
        text1.Text.SetAttribute(HtmlAttributes.autocomplete, "off");

        var elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.width, "95%");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.marginTop, "10px");
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "40px 10px 40px 40px");

        elmBox.AddItem(lbl1, 34);
        elmBox.AddItem(text, 4);
        elmBox.AddItem(text1, 50);
        elmBox.AddItem(BtnWrap);

        string ViewHtml = filter.HtmlText + elmBox.HtmlText;
        return ViewHtml;
    }

    public ApiResponse PassSave()
    {
        string pwd = GetDataValue("pwd");
        string pwd1 = GetDataValue("pwd1");

        var _ApiResponse = new ApiResponse();

        bool Valid = true;
        string DialogMsg = string.Empty;

        if (string.IsNullOrEmpty(pwd) || string.IsNullOrEmpty(pwd1))
        {
            Valid = false;
        }

        if (Valid == false)
        {
            DialogMsg = Translator.Format("msg_required");
        }
        else
        {
            if (pwd != pwd1)
            {
                DialogMsg = Translator.Format("msg_pwdconfirm");
            }
            else
            {
                string rlt = ValidatePassword(pwd);
            }
        }

        if (DialogMsg != string.Empty)
        {
            var dialogBox = new DialogBox(DialogMsg);
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
            _ApiResponse.ExecuteScript("XysProPassShowButtons();");
        }
        else
        {
            string MailFile = HtmlFolder + References.Htmls.Email_PassChanged + "_" + ClientLanguage + ".html";
            if (!File.Exists(MailFile))
            {
                MailFile = References.Htmls.Email_PassChanged;
            }
            else
            {
                MailFile = References.Htmls.Email_PassChanged + "_" + ClientLanguage;
            }

            string rlt = SaveData(pwd);
            if (string.IsNullOrEmpty(rlt))
            {
                string Subject = HtmlTranslator.Value("msg_email");
                string bodyHtml = ReadHtmlFile(MailFile)
                                         .Replace("{username}", AppKey.UserName)
                                         .Replace("{useremail}", AppKey.UserEmail);
                string[] ToAddr = { AppKey.UserEmail };

                string rltmail = SendEmail(Subject, bodyHtml, ToAddr);
                if (string.IsNullOrEmpty(rltmail))
                {
                    _ApiResponse.Navigate(References.Pages.XysSignin);
                }
                else
                {
                    var dialogBox = new DialogBox(rltmail);
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
                    _ApiResponse.ExecuteScript("XysProPassShowButtons();");
                }
            }
            else
            {
                var dialogBox = new DialogBox(rlt);
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
                _ApiResponse.ExecuteScript("XysProPassShowButtons();");
            }
        }

        return _ApiResponse;
    }

    private string SaveData(string UserPwd)
    {
        List<string> SQL = new List<string>();
        SQL.Add(" update XysUser set UserPwd=@UserPwd, SYSDTE = getdate(),SYSUSR=@UserId where UserId = @UserId ");

         List<System.Data.SqlClient.SqlParameter> SqlParams = new List<System.Data.SqlClient.SqlParameter>()
        {
            new System.Data.SqlClient.SqlParameter { ParameterName = "@UserId", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar },
            new System.Data.SqlClient.SqlParameter { ParameterName = "@UserPwd", Value = Encryptor.EncryptData(UserPwd), SqlDbType = SqlDbType.NVarChar }
        };

        return PutData(SqlWithParams(SQL, SqlParams));
    }
}