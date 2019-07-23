using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

[JsonObject(MemberSerialization.OptIn)]
public partial class APIReturn : ContentResult
{
    /// <summary>
    /// 错误代码
    /// </summary>
    [JsonProperty("code")] public int Code { get; protected set; }
    /// <summary>
    /// 错误消息
    /// </summary>
    [JsonProperty("message")] public string Message { get; protected set; }

    public APIReturn() { }
    public APIReturn(int code, string message) => this.SetCode(code);

    public virtual APIReturn SetCode(int value) { this.Code = value; return this; }
    public virtual APIReturn SetCode(Enum value) { this.Code = Convert.ToInt32(value); this.Message = value.ToString(); return this; }
    public virtual APIReturn SetMessage(string value) { this.Message = value; return this; }

    #region form 表单 target=iframe 提交回调处理
    protected void Jsonp(ActionContext context)
    {
        string __callback = context.HttpContext.Request.HasFormContentType ? context.HttpContext.Request.Form["__callback"].ToString() : null;
        if (string.IsNullOrEmpty(__callback))
        {
            this.ContentType = "text/json;charset=utf-8;";
            this.Content = JsonConvert.SerializeObject(this);
        }
        else
        {
            this.ContentType = "text/html;charset=utf-8";
            this.Content = $"<script>top.{__callback}({GlobalExtensions.Json(null, this)});</script>";
        }
    }
    public override void ExecuteResult(ActionContext context)
    {
        Jsonp(context);
        base.ExecuteResult(context);
    }
    public override Task ExecuteResultAsync(ActionContext context)
    {
        Jsonp(context);
        return base.ExecuteResultAsync(context);
    }
    #endregion

    public static APIReturn Success => new APIReturn(0, "成功");
    public static APIReturn Failed => new APIReturn(99, "失败");
}

[JsonObject(MemberSerialization.OptIn)]
public partial class APIReturn<T> : APIReturn
{
    [JsonProperty("data")] public T Data { get; protected set; }

    public APIReturn() { }
    public APIReturn(int code) => this.SetCode(code);
    public APIReturn(string message) => this.SetMessage(message);
    public APIReturn(int code, string message) => this.SetCode(code).SetMessage(message);

    new public APIReturn<T> SetCode(int value) { this.Code = value; return this; }
    new public APIReturn<T> SetCode(Enum value) { this.Code = Convert.ToInt32(value); this.Message = value.ToString(); return this; }
    new public APIReturn<T> SetMessage(string value) { this.Message = value; return this; }
    public APIReturn<T> SetData(T value) { this.Data = value; return this; }

    new public static APIReturn<T> Success => new APIReturn<T>(0, "成功");
}