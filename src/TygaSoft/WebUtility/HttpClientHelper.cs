﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using System.IO;
using TygaSoft.Converter;

namespace TygaSoft.WebUtility
{
    public class HttpClientHelper
    {
        public static string GetClientIp(HttpContext context)
        {
            string ip = "";

            var request = context.Request;
            var nvc = request.ServerVariables;

            //发出请求的远程主机的ip地址是否为空，不为空则获取
            //否则判断是否使用设置代理，使用则获取代理的服务器Ip地址，否则直接获取客户端IP
            if (!string.IsNullOrWhiteSpace(nvc["REMOTE_ADDR"]))
            {
                ip = request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            else if (nvc["HTTP_VIA"] != null)
            {
                if (!string.IsNullOrWhiteSpace(nvc["HTTP_X_FORWARDED_FOR"]))
                {
                    ip = nvc["HTTP_X_FORWARDED_FOR"];
                }
                else
                {
                    ip = request.UserHostAddress;
                }
            }
            else
            {
                ip = request.UserHostAddress;
            }

            return ip;
        }

        public static void Export(HttpContext context, DataTable dt)
        {
            using (var stream = new MemoryStream())
            {
                ExcelHelper.Export(stream, dt);
                context.Response.Buffer = true;
                context.Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")));
                context.Response.ContentType = "application/ms-excel";
                context.Response.BinaryWrite(stream.ToArray());
                context.Response.Flush();
            }
        }
    }
}
