var userid = "";
var bodySize = [];
var clickSize = [];
var tousername;
var W = 0; //取得屏幕分辨率宽度
var H = 0; //取得屏幕分辨率高度
function getWin() {
    if (window.innerWidth) {
        W = window.innerWidth;
    }
    else if ((document.body) && (document.body.clientWidth)) {
        W = document.body.clientWidth;
    }
    // 获得窗口高度 
    if (window.innerHeight) {
        H = window.innerHeight;
    }
    else if ((document.body) && (document.body.clientHeight)) {
        H = document.body.clientHeight;
    }
    if (document.documentElement && document.documentElement.clientHeight && document.documentElement.clientWidth) {
        W = document.documentElement.clientWidth;
        H = document.documentElement.clientHeight;
    }

}
var oIframe = document.createElement('iframe');
var open_Div = document.createElement('div');
var data_div = document.createElement('div');
var content;
if ($(".popup_peo").length > 0) {
    content = $('.popup_peo')[0].outerHTML;
}
function pointerXY(event) {
    if (event.pageX || event.pageY) {
        bodySize[0] = event.pageX;
        bodySize[1] = event.pageY;
        clickSize[0] = event.clientX;
        clickSize[1] = event.clientX;
    }
    else {
        bodySize[0] = event.clientX + document.documentElement.scrollLeft;
        bodySize[1] = event.clientY + document.documentElement.scrollTop;
        clickSize[0] = event.clientX;
        clickSize[1] = event.clientY;
    }
}
function getBodySize() {
    return bodySize;
}
function getClickSize() {
    return clickSize;
}
function change() {
    var bodySize = getBodySize();
    var clickSize = getClickSize();
    var wi = W - clickSize[0];
    var hi = H - clickSize[1];
    if (wi < 352) {
        wi = bodySize[0] + wi - 352 + 50;
    }
    else {
        wi = bodySize[0] + 50;
    }
    if (hi < 220) {
        hi = bodySize[1] + hi - 220;
    }
    else {
        hi = bodySize[1];
    }

    showIframe(open_Div, hi, wi);
}
function showIframe(div, hi, wi) {
    div.style.width = 353 + "px";
    div.style.height = 216 + "px";
    div.style.left = wi + "px";
    div.style.top = hi + "px";
    div.style.display = 'block';
    div.style.position = "absolute";
    div.style.border = "none";
    div.innerHTML = "";
    div.id = "tempDiv"
    div.style.zIndex = 10;
    oIframe.id = 'HelpFrame';
    oIframe.frameborder = 1;
    oIframe.style.position = 'absolute';
    oIframe.style.zIndex = 9;

    oIframe.style.width = 360 + "px";
    oIframe.style.height = 216 + "px";
    oIframe.style.top = 'auto';
    oIframe.style.left = 'auto';
    oIframe.style.display = 'block';

    //div.appendChild(oIframe);
    data_div.innerHTML = $('.popup_peo')[0].innerHTML;;
    data_div.className = "popup_peo";
    data_div.style.zIndex = 10;
    data_div.style.display = "block";

    div.appendChild(data_div);
    document.forms[0].appendChild(div);
}
function openPerson() {
    $.ajax({
        url: "/AjaxHander/GetUserInfo.ashx",
        type: "GET",
        data: { "UserID": userid },
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        async: false,
        success: function (data) {
            var sex = data.Gender;
            $("#pName").html(data.Name);
            $("#pMobliePhone").html(data.Phone);
            $("#pPhone").html(data.OfficeTel);
            $("#pEmail").html(data.OfficeEmail);
            $("#pOrg").html(data.Department);
            $("#pSex").html(data.Gender == "男" ? "男" : "女");
            $("#pPosition").html(data.JobTitle);
            $("#pOrg").attr("title", data.unitName);
            $("#pPosition").attr("title", data.JobTitle);
            if (sex == "男")
            {
                $("#pImg").attr("src","/images/men.png");
            }
            else{
                $("#pImg").attr("src","/images/women.png");
            }
        },
        error: function (error) {
            alert("调用出错" + error.responseText);
        }
    });
}
//打开DIV层
function openUserInfo(tempuserid) {
    userid = tempuserid;
    getWin();
    openPerson();
    change();
    void (0);
}

//关闭div
function closediv() {
    $("#tempDiv")[0].style.display = "none";
    //$("#HelpFrame")[0].style.display = "none";
    void (0);
}

//function TextChange(ob) {
//    var spanRed = $(ob).parent().children(".color_red");
//    if (spanRed.length > 0) {
//        if ($(ob).val().length > 0) {
//            spanRed.hide();
//        }
//        else {
//            spanRed.show();
//        }
//    }
//}

function TextChangeForUserControl(ob) {
    var spanRed = $(ob).parent().parent().parent().children(".color_red");
    if (spanRed.length > 0) {
        if ($(ob).val().length > 0) {
            spanRed.hide();
        }
        else {
            spanRed.show();
        }
    }
}