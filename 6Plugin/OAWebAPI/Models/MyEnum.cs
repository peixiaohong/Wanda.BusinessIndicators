using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plugin.OAMessage
{
    public class MyEnum
    {
    }
    public enum RequestType
    {
        ToDo = 0,
        Done = 2,
        Over = 4,
        Cancel = 9,
        DirectOver=99
    }
    public enum ResCode
    {
        success=0,
        oaerror=100,
        parameter=200,
        error=400,
        exception=500
    }
}