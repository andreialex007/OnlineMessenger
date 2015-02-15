﻿using System;
using System.Web;

namespace OnlineMessenger.MvcServer.Tools
{
    public static class HttpPostedFileBaseExtensions
    {
        public static Byte[] ToByteArray(this HttpPostedFileBase value)
        {
            if (value == null)
                return null;
            var array = new Byte[value.ContentLength];
            value.InputStream.Position = 0;
            value.InputStream.Read(array, 0, value.ContentLength);
            return array;
        }
    }
}
